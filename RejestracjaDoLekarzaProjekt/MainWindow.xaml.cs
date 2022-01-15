using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;

namespace RejestracjaDoLekarzaProjekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SpeechSynthesizer ss = new SpeechSynthesizer();
        static SpeechRecognitionEngine sre;
        static DataRepository repo = new DataRepository();
        string isPatient = "";
        string isPeselCorrect = "";
        string pesel = "";
        bool peselSaid = false;
        bool newPeselSaid = false;
        Person person = null;
        Specjalization specjalization = null;
        List<Doctor> doctors = null;
        Person doctor = null;
        List<Visit> freeVisitsOfDoctor = null;

        public MainWindow()
        {
            InitializeComponent();

            var db = new DataContext();

            ss.SetOutputToDefaultAudioDevice();
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized;

            Grammar grammar = new Grammar(".\\Grammars\\Grammar.xml");
            grammar.Enabled = true;
            sre.LoadGrammar(grammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            ss.SpeakAsync("Witaj w naszej przychodni");
            ss.SpeakAsync("Czy jesteś naszym pacjentem? Odpowiedz tak lub nie");
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            lbl1.Content = "";
            float confidence = e.Result.Confidence;
            lbl2.Content = e.Result.Text + "        Pewność: " + confidence;
            if (confidence <= 0.6)
            {
                ss.SpeakAsync("Proszę powtórzyć");
            }
            else
            {
                if (isPatient == "")
                {
                    try { isPatient = e.Result.Semantics["decision"].Value.ToString(); }
                    catch (KeyNotFoundException) { }
                }

                if (isPatient == "tak" && pesel == "" && !peselSaid)
                {
                    ss.SpeakAsync("Podaj numer pe-sel, podawaj kolejne cyfry pojedynczo");
                    peselSaid = true;
                } else
                if (isPatient == "tak" && pesel.Length != 11)
                {
                    try { pesel += e.Result.Semantics["number"].Value.ToString(); }
                    catch (KeyNotFoundException) { }
                    if (pesel.Length == 11)
                    {
                        ss.SpeakAsync("Czy twój pe-sel to: " + pesel[0] + ", " + pesel[1] + ", " + pesel[2] + ", " + pesel[3]
                            + ", " + pesel[4] + ", " + pesel[5] + ", " + pesel[6] + ", " + pesel[7] + ", " + pesel[8]
                            + ", " + pesel[9] + ", " + pesel[10] + "? Odpowiedz tak lub nie");
                    }
                } else
                if (isPatient == "tak" && pesel.Length == 11 && isPeselCorrect == "")
                {
                    try { isPeselCorrect = e.Result.Semantics["decision"].Value.ToString(); }
                    catch (KeyNotFoundException) { }
                    if (isPeselCorrect == "tak")
                    {
                        person = repo.GetPatient(pesel);
                        if (person != null)
                            ss.SpeakAsync("Witaj " + person.Name + " " + person.Surname + " Do jakiego lekarza chcesz się u-mówić?");
                        else
                        {
                            ss.SpeakAsync("Brak pacjenta o takim numerze pe-sel w naszej bazie danych, musisz najpierw złożyc deklaracje, " +
                                "aby móc korzystać z usług naszych wykwalifikowanych specjalistów");
                            isPatient = "nie";
                            pesel = "";
                            newPeselSaid = false;
                        }
                    }
                    else
                    if (isPeselCorrect == "nie")
                    {
                        pesel = "";
                        isPeselCorrect = "";
                        ss.SpeakAsync("Podaj ponownie swój pe-sel, podawaj kolejne cyfry pojedynczo");
                    }

                }

                if (isPatient == "nie" && pesel == "" && !newPeselSaid)
                {
                    ss.SpeakAsync("Złóż deklaracje podaj swój numer pe-sel, podawaj kolejne cyfry pojedynczo");
                    newPeselSaid = true;
                }
                else
                if (isPatient == "nie" && pesel.Length != 11)
                {
                    try { pesel += e.Result.Semantics["number"].Value.ToString(); }
                    catch (KeyNotFoundException) { }
                    if (pesel.Length == 11)
                    {
                        ss.SpeakAsync("Czy twój pe-sel to: " + pesel[0] + ", " + pesel[1] + ", " + pesel[2] + ", " + pesel[3]
                            + ", " + pesel[4] + ", " + pesel[5] + ", " + pesel[6] + ", " + pesel[7] + ", " + pesel[8]
                            + ", " + pesel[9] + ", " + pesel[10] + "? Odpowiedz tak lub nie");
                    }
                } else 
                if (isPatient == "nie" && pesel.Length == 11 && isPeselCorrect =="")
                {
                    try { isPeselCorrect = e.Result.Semantics["decision"].Value.ToString(); }
                    catch (KeyNotFoundException) { }
                    if (isPeselCorrect == "tak")
                    {
                        repo.AddPatient("", "", pesel, "");
                        person = repo.GetPatient(pesel);
                        if (person != null)
                        {
                            ss.SpeakAsync("Zostałeś poprawnie zarejestrowany w naszej przychodni. " +
                            "Teraz możesz u-mówić się na wizytę. Podaj do jakiej specjalności lekarza chcesz się u-mówić");
                        } else
                        {
                            pesel = "";
                            isPeselCorrect = "";
                            ss.SpeakAsync("Nie zostałeś zarejestrowany");
                        }
                        
                    } else
                    if (isPeselCorrect == "nie")
                    {
                        pesel = "";
                        isPeselCorrect = "";
                        ss.SpeakAsync("Złóż ponownie deklaracje podaj swój numer pe-sel, podawaj kolejne cyfry pojedynczo");
                    }

                }

                if (person != null )
                {
                    if (specjalization == null || doctor == null)
                    {
                        string specString = "";
                        try { specString = e.Result.Semantics["specjalization"].Value.ToString(); }
                        catch (KeyNotFoundException) { }

                        string doctorSurname = "";
                        try { doctorSurname = e.Result.Semantics["doctorSurname"].Value.ToString(); }
                        catch (KeyNotFoundException) { }

                        string doctorName = "";
                        try { doctorName = e.Result.Semantics["doctorName"].Value.ToString(); }
                        catch (KeyNotFoundException) { }

                        if (specString != "")
                        {
                            specjalization = repo.GetSpecjalization(specString);
                            if (specjalization != null)
                            {
                                ss.Speak("Wybrałeś lekarza o specjalizacji " + specjalization.Name);
                                doctors = repo.GetDoctorsBySpecjalizaton(specjalization.Id);
                                ss.Speak("Dostępni lekarze to");
                                foreach (Doctor d in doctors)
                                {
                                    ss.Speak(d.Person.Name + " " + d.Person.Surname);
                                }
                                ss.SpeakAsync("Powiedz imię i nazwisko lekarza do którego chcesz się zapisać");
                            } else
                            {
                                ss.SpeakAsync("Brak lekarza o takiej specjalizacji u nas w przychodni. Wybierz innego lekarza");
                            }
                            
                        }

                        if (doctorSurname != "")
                        {
                            doctor = repo.GetDoctorByName(doctorName, doctorSurname);
                            specjalization = repo.GetSpecjalization("Internista");
                            if (doctor != null)
                            {
                                freeVisitsOfDoctor = repo.GetFreeVisitsForDoctor(doctor.Doctors.First().Id);
                                if (freeVisitsOfDoctor == null || freeVisitsOfDoctor.Count == 0)
                                {
                                    ss.SpeakAsync("Doktor " + doctor.Name + " " + doctor.Surname + " nie ma już wolnych terminów. Wybierz innego lekarza");
                                    doctor = null;
                                    freeVisitsOfDoctor = null;
                                    specjalization = null;
                                }
                                else
                                {
                                    ss.SpeakAsync("Wybrałeś doktora " + doctor.Name + " " + doctor.Surname + " Najbliższy termin tego specjalisty to: "
                                        + freeVisitsOfDoctor.First().Date.Day + " " + freeVisitsOfDoctor.First().Date.Month + " " 
                                        + freeVisitsOfDoctor.First().Date.Hour + ":" + freeVisitsOfDoctor.First().Date.Minute
                                        + " Jeśli chcesz potwierdzić ten termin powiedz tak. Jeśli chcesz " +
                                        "potwierdzić inny termin powiedz datę i godzinę wizyty Inne terminy zostały" +
                                        "wypisane na ekranie");
                                }
                            }
                        }
                        
                    } else
                    {
                        if (doctor == null)
                        {
                            string doctorSurname = "";
                            try { doctorSurname = e.Result.Semantics["doctorSurname"].Value.ToString(); }
                            catch (KeyNotFoundException) { }

                            string doctorName = "";
                            try { doctorName = e.Result.Semantics["doctorName"].Value.ToString(); }
                            catch (KeyNotFoundException) { }

                            doctor = repo.GetDoctorByName(doctorName, doctorSurname);
                            if (doctor != null)
                            {
                                freeVisitsOfDoctor = repo.GetFreeVisitsForDoctor(doctor.Doctors.First().Id);
                                if(freeVisitsOfDoctor == null || freeVisitsOfDoctor.Count == 0)
                                {
                                    ss.SpeakAsync("Doktor " + doctor.Name + " " + doctor.Surname + " nie ma już wolnych terminów. Wybierz innego lekarza");
                                    doctor = null;
                                    freeVisitsOfDoctor = null;
                                    specjalization = null;
                                } else
                                {
                                    ss.SpeakAsync("Wybrałeś doktora " + doctor.Name + " " + doctor.Surname + " Najbliższy termin tego specjalisty to: "
                                        + freeVisitsOfDoctor.First().Date.Day + " " + freeVisitsOfDoctor.First().Date.Month + " "
                                        + freeVisitsOfDoctor.First().Date.Hour + ":" + freeVisitsOfDoctor.First().Date.Minute
                                        + " Jeśli chcesz potwierdzić ten termin powiedz tak. Jeśli chcesz " +
                                        "potwierdzić inny termin powiedz datę i godzinę wizyty Inne terminy zostały" +
                                        "wypisane na ekranie");
                                }
                            }
                        } else
                        {
                            string decision = "";
                            try { decision = e.Result.Semantics["decision"].Value.ToString(); }
                            catch (KeyNotFoundException) { }
                            if (decision == "tak")
                            {
                                repo.BookVisit(freeVisitsOfDoctor.First().Id, person.Patients.First().Id);
                                ss.SpeakAsync("Zapisałam wizytę na " + freeVisitsOfDoctor.First().Date.Day + " " + freeVisitsOfDoctor.First().Date.Month + " "
                                        + freeVisitsOfDoctor.First().Date.Hour + ":" + freeVisitsOfDoctor.First().Date.Minute + " zdrówka wariacie!");
                            } else
                            {
                                try { decision = e.Result.Semantics["decision"].Value.ToString(); }
                                catch (KeyNotFoundException) { }
                            }

                        }
                    }
                   
                }

            }
        }
    }
}
