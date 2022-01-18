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

        List<AccessText> doctors_cells = new List<AccessText>();
        List<Label> doctors_cells_background = new List<Label>();

        List<AccessText> visits_cells = new List<AccessText>();
        List<Label> visits_cells_background = new List<Label>();

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

            doctors_cells.Add(doc1_label);
            doctors_cells.Add(doc2_label);
            doctors_cells.Add(doc3_label);
            doctors_cells.Add(doc4_label);
            doctors_cells.Add(doc5_label);
            doctors_cells.Add(doc6_label);
            doctors_cells.Add(doc7_label);
            doctors_cells.Add(doc8_label);

            doctors_cells_background.Add(doc1_background);
            doctors_cells_background.Add(doc2_background);
            doctors_cells_background.Add(doc3_background);
            doctors_cells_background.Add(doc4_background);
            doctors_cells_background.Add(doc5_background);
            doctors_cells_background.Add(doc6_background);
            doctors_cells_background.Add(doc7_background);
            doctors_cells_background.Add(doc8_background);

            visits_cells.Add(vis1_label);
            visits_cells.Add(vis2_label);
            visits_cells.Add(vis3_label);
            visits_cells.Add(vis4_label);
            visits_cells.Add(vis5_label);
            visits_cells.Add(vis6_label);
            visits_cells.Add(vis7_label);
            visits_cells.Add(vis8_label);

            visits_cells_background.Add(vis1_background);
            visits_cells_background.Add(vis2_background);
            visits_cells_background.Add(vis3_background);
            visits_cells_background.Add(vis4_background);
            visits_cells_background.Add(vis5_background);
            visits_cells_background.Add(vis6_background);
            visits_cells_background.Add(vis7_background);
            visits_cells_background.Add(vis8_background);


            ss.SpeakAsync("Czy jesteś naszym pacjentem? Odpowiedz tak lub nie");
            
            existing_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
            new_account_background.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");

            phase_1_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
        }

        private void load_doctors(Specjalization specialization)
        {
            doctors = repo.GetDoctorsBySpecjalizaton(specjalization.Id);
            try
            {
                for (int i = 0; i < doctors.Count(); i++)
                {
                    doctors_cells[i].Text = doctors[i].Person.Name + " " + doctors[i].Person.Surname;
                }
            }
            catch (IndexOutOfRangeException) { }
        }

        private String parse_visit_string(DateTime date)
        {
            String hour;
            String minutes;
            String day;
            String month;
            
            if(date.Hour < 10)
            {
                hour = "0" + date.Hour;
            }
            else
            {
                hour = date.Hour.ToString();
            }

            if(date.Minute < 10)
            {
                minutes = "0" + date.Minute;
            }
            else
            {
                minutes = date.Minute.ToString();
            }

            if(date.Day < 10)
            {
                day = "0" + date.Day;
            }
            else
            {
                day = date.Day.ToString();
            }

            if(date.Month < 10)
            {
                month = "0" + date.Month;
            }
            else
            {
                month = date.Month.ToString();
            }

            return hour + ":" + minutes + " " + day + "-" + month + "-" + date.Year;
        }

        private void load_visits(List<Visit> visits)
        {
            try
            {
                for (int i = 0; i < visits.Count(); i++)
                {
                    visits_cells[i].Text = this.parse_visit_string(visits[i].Date);
                }
            }
            catch (IndexOutOfRangeException) { }
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

        private void complete_phase_3()
        {
            phase_3_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            phase_4_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
        }

        private void complete_phase_4()
        {
            phase_4_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
        }

        private void mark_specialization(string specialization)
        {
            Console.WriteLine("===========================");
            Console.WriteLine(specialization);
            Console.WriteLine("===========================");


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

        private void mark_doctor(Person doctor)
        {
            try
            {
                int index = this.doctors_cells.FindIndex((x => (doctor.Name + " " + doctor.Surname) == x.Text));
                doctors_cells_background[index].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
            }
            catch (IndexOutOfRangeException) { }
        }

        private void mark_visit(Visit visit)
        {
            int index = this.visits_cells.FindIndex((x => this.parse_visit_string(visit.Date) == x.Text));
            visits_cells_background[index].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightGreen");
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

            for(int i=0; i < 8; i++)
            {
                this.doctors_cells[i].Text = "";
            }

            phase_2_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("LightBlue");
            phase_3_label.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("Transparent");
        }

        private string get_visit_date_string(DateTime date)
        {
            string hour;
            string minutes;

            if(date.Hour == 1)
            {
                hour = "pierwszą";
            }
            else if(date.Hour == 2)
            {
                hour = "drugą";
            }
            else if (date.Hour == 3)
            {
                hour = "trzecią";
            }
            else if (date.Hour == 4)
            {
                hour = "czwartą";
            }
            else if (date.Hour == 5)
            {
                hour = "piątą";
            }
            else if (date.Hour == 6)
            {
                hour = "szóstą";
            }
            else if (date.Hour == 7)
            {
                hour = "siódmą";
            }
            else if (date.Hour == 8)
            {
                hour = "ósmą";
            }
            else if (date.Hour == 9)
            {
                hour = "dziewiątą";
            }
            else if (date.Hour == 10)
            {
                hour = "dziesiątą";
            }
            else if (date.Hour == 11)
            {
                hour = "jedenastą";
            }
            else if (date.Hour == 12)
            {
                hour = "dwunastą";
            }
            else if (date.Hour == 13)
            {
                hour = "trzynastą";
            }
            else if (date.Hour == 14)
            {
                hour = "czternastą";
            }
            else if (date.Hour == 15)
            {
                hour = "piętnastą";
            }
            else if (date.Hour == 16)
            {
                hour = "szesnastą";
            }
            else if (date.Hour == 17)
            {
                hour = "siedemnastą";
            }
            else if (date.Hour == 18)
            {
                hour = "osiemnastą";
            }
            else if (date.Hour == 19)
            {
                hour = "dziewiętnastą";
            }
            else if (date.Hour == 20)
            {
                hour = "dwudziestą";
            }
            else if (date.Hour == 21)
            {
                hour = "dwudziestą pierwszą";
            }
            else if (date.Hour == 22)
            {
                hour = "dwudziestą drugą";
            }
            else
            {
                hour = "dwudziestą trzecią";
            }

            if(date.Minute == 0)
            {
                minutes = "zero zero";
            }
            else if(date.Minute == 15)
            {
                minutes = "piętnaście";
            }
            else if(date.Minute == 30)
            {
                minutes = "trzydzieści";
            }
            else
            {
                minutes = "czterdzieści pięć";
            }

            return hour + " " + minutes;
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
                                load_doctors(specjalization);
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
                            specjalization = repo.GetSpecjalization(doctor.Doctors.First().Specjalization.Name);

                            /*=================================================
                             TODO: dodać funkcję, która znajdzie specjalizację podanego lekarza*/
                            this.mark_specialization(specjalization.Name);
                            this.load_doctors(specjalization);
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
                                    complete_phase_3();
                                    mark_doctor(doctor);
                                    load_visits(freeVisitsOfDoctor);
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
                                    complete_phase_3();
                                    mark_doctor(doctor);
                                    load_visits(freeVisitsOfDoctor);
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
                                this.complete_phase_4();
                                this.mark_visit(freeVisitsOfDoctor.First());
                                ss.Speak("Zapisałam wizytę na " + freeVisitsOfDoctor.First().Date.Day + " " + freeVisitsOfDoctor.First().Date.Month + " "
                                        + this.get_visit_date_string(freeVisitsOfDoctor.First().Date) + " zdrówka wariacie!");
                            }
                            else
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
                                    this.complete_phase_4();
                                    this.mark_visit(visit);
                                    ss.SpeakAsync("Zapisałam wizytę na " + visit.Date.Day + " " + visit.Date.Month + " "
                                            + this.get_visit_date_string(visit.Date) + " zdrówka wariacie!");

                                }
                            }

                        }
                    }
                   
                }

            }
        }
    }
}
