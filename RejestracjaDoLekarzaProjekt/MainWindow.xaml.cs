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
        string day = "";
        string month = "";
        string hour = "";
        string minutes = "";
        Person person = null;
        Specjalization specjalization = null;
        List<Doctor> doctors = null;
        Person doctor = null;
        List<Visit> freeVisitsOfDoctor = null;

        List<Label> pesel_cells = new List<Label>();

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

            /*calendar.SelectedDates.Add(new DateTime(2022, 1, 5));
            calendar.SelectedDates.Add(new DateTime(2022, 1, 6));
            calendar.SelectedDates.Add(new DateTime(2022, 1, 7));
            calendar.SelectedDates.Add(new DateTime(2022, 1, 8));

            Console.WriteLine("=========================================");
            Console.WriteLine(calendar.SelectedDates);
            Console.WriteLine("=========================================");*/

            pesel_cells.Add(pesel_cell_1);
            pesel_cells.Add(pesel_cell_2);
            pesel_cells.Add(pesel_cell_3);
            pesel_cells.Add(pesel_cell_4);
            pesel_cells.Add(pesel_cell_5);
            pesel_cells.Add(pesel_cell_6);
            pesel_cells.Add(pesel_cell_7);
            pesel_cells.Add(pesel_cell_8);
            pesel_cells.Add(pesel_cell_9);
            pesel_cells.Add(pesel_cell_10);
            pesel_cells.Add(pesel_cell_11);

            ss.SpeakAsync("Czy jesteś naszym pacjentem? Odpowiedz tak lub nie");
            
            existing_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
            new_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");

            phase_1_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
        }

        private void reset_pesel_labels()
        {
            for(int i=0; i<11; i++)
            {
                pesel_cells[i].Content = "";
                pesel_cells[i].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
            }
            pesel_cells[0].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
        }

        private void complete_phase_1()
        {
            phase_1_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            phase_2_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
        }

        private void complete_phase_2()
        {
            phase_2_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            phase_3_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
        }

        private void mark_specialization(string specialization)
        {
            if(specialization == "Dentysta")
            {
                dentysta.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if(specialization == "Internista")
            {
                internista.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if(specialization == "Ginekolog")
            {
                ginekolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if(specialization == "Dermatolog")
            {
                dermatolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if(specialization == "Alergolog")
            {
                alergolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if (specialization == "Endokrynolog")
            {
                endokrynolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if (specialization == "Okulista")
            {
                okulista.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if (specialization == "Laryngolog")
            {
                laryngolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if (specialization == "Ortopeda")
            {
                ortopeda.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if (specialization == "Chirurg")
            {
                chirurg.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if (specialization == "Pediatra")
            {
                pediatra.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            else if (specialization == "Psycholog")
            {
                psycholog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
        }

        private void back_to_phase_2()
        {
                dentysta.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                internista.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                ginekolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                dermatolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                alergolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                endokrynolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                okulista.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                laryngolog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                ortopeda.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                chirurg.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                pediatra.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                psycholog.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");

            phase_2_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
            phase_3_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
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
                    try { 
                        isPatient = e.Result.Semantics["decision"].Value.ToString(); 
                        if(isPatient == "tak")
                        {
                            existing_account.IsChecked = true;
                            existing_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
                            new_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                        }
                        else
                        {
                            new_account.IsChecked = true;
                            existing_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                            new_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
                        }
                    }
                    catch (KeyNotFoundException) { }
                }

                if (isPatient == "tak" && pesel == "" && !peselSaid)
                {
                    ss.SpeakAsync("Podaj numer pe-sel, podawaj kolejne cyfry pojedynczo");
                    peselSaid = true;
                    pesel_cells[0].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
                } else
                if (isPatient == "tak" && pesel.Length != 11)
                {
                    try { 
                        pesel += e.Result.Semantics["number"].Value.ToString();
                        pesel_cells[pesel.Length - 1].Content = pesel.Last();

                        pesel_cells[pesel.Length - 1].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
                        if(pesel.Length < 11)
                        {
                            pesel_cells[pesel.Length].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
                        }
                    }
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
                        {
                            ss.SpeakAsync("Witaj " + person.Name + " " + person.Surname + " Do jakiego lekarza chcesz się u-mówić?");
                            this.complete_phase_1();
                        }
                        else
                        {
                            ss.SpeakAsync("Brak pacjenta o takim numerze pe-sel w naszej bazie danych, musisz najpierw złożyc deklaracje, " +
                                "aby móc korzystać z usług naszych wykwalifikowanych specjalistów");
                            isPatient = "nie";
                            pesel = "";
                            newPeselSaid = false;

                            existing_account.IsChecked = false;
                            new_account.IsChecked = true;
                            this.reset_pesel_labels();
                            existing_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
                            new_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
                        }
                    }
                    else
                    if (isPeselCorrect == "nie")
                    {
                        pesel = "";
                        isPeselCorrect = "";
                        this.reset_pesel_labels();
                        ss.SpeakAsync("Podaj ponownie swój pe-sel, podawaj kolejne cyfry pojedynczo");
                    }

                }

                if (isPatient == "nie" && pesel == "" && !newPeselSaid)
                {
                    ss.SpeakAsync("Złóż deklaracje podaj swój numer pe-sel, podawaj kolejne cyfry pojedynczo");
                    pesel_cells[0].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
                    newPeselSaid = true;
                }
                else
                if (isPatient == "nie" && pesel.Length != 11)
                {
                    try {
                        pesel += e.Result.Semantics["number"].Value.ToString();
                        pesel_cells[pesel.Length - 1].Content = pesel.Last();

                        pesel_cells[pesel.Length - 1].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
                        if (pesel.Length < 11)
                        {
                            pesel_cells[pesel.Length].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
                        }
                    }
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
                            this.complete_phase_1();
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
                        this.reset_pesel_labels();
                        ss.SpeakAsync("Złóż ponownie deklaracje podaj swój numer pe-sel, podawaj kolejne cyfry pojedynczo");
                    }

                }

                if (person != null )
                {
                    if (specjalization == null || doctor == null)
                    {
                        string specString = "";
                        try { 
                            specString = e.Result.Semantics["specjalization"].Value.ToString();
                            this.mark_specialization(specString);
                            this.complete_phase_2();
                        }
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

                            /*=================================================
                             TODO: dodać funkcję, która znajdzie specjalizację podanego lekarza*/
                            this.mark_specialization("Internista");
                            /*=================================================*/
                            this.complete_phase_2();
                            if (doctor != null)
                            {
                                freeVisitsOfDoctor = repo.GetFreeVisitsForDoctor(doctor.Doctors.First().Id);
                                if (freeVisitsOfDoctor == null || freeVisitsOfDoctor.Count == 0)
                                {
                                    ss.SpeakAsync("Doktor " + doctor.Name + " " + doctor.Surname + " nie ma już wolnych terminów. Wybierz innego lekarza");
                                    doctor = null;
                                    freeVisitsOfDoctor = null;
                                    specjalization = null;
                                    this.back_to_phase_2();
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
                        
                    } 
                    else
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
                                    this.back_to_phase_2();
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
                        } 
                        else
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
                                if (day == "")
                                {
                                    try { day = e.Result.Semantics["day"].Value.ToString(); }
                                    catch (KeyNotFoundException) { }
                                }
                                if (month == "")
                                {
                                    try { month = e.Result.Semantics["month"].Value.ToString(); }
                                    catch (KeyNotFoundException) { }
                                }
                                if (hour == "")
                                {
                                    try { hour = e.Result.Semantics["hour"].Value.ToString(); }
                                    catch (KeyNotFoundException) { }
                                }

                                if (day == "")
                                {
                                    ss.SpeakAsync("Powtórz dzień");
                                }
                                if (month == "")
                                {
                                    ss.SpeakAsync("Powtórz miesiąc");
                                }
                                if (hour == "")
                                {
                                    ss.SpeakAsync("Powtórz godzinę");
                                }

                                if (day != "" && month != "" && hour != "")
                                {
                                    var date = day + "." + month + ".2022 " + hour;
                                    var visit = freeVisitsOfDoctor.Where(x => x.Date.ToString() == date).First();
                                    repo.BookVisit(visit.Id, person.Patients.First().Id);
                                    ss.SpeakAsync("Zapisałam wizytę na " + visit.Date.Day + " " + visit.Date.Month + " "
                                            + visit.Date.Hour + ":" + visit.Date.Minute + " zdrówka wariacie!");
                                }
                            }

                        }
                    }
                   
                }

            }
        }
    }
}
