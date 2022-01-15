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
        string pesel = "";
        bool peselSaid = false;
        
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
                        ss.SpeakAsync("Twój pe-sel to: " + pesel[0] + ", " + pesel[1] + ", " + pesel[2] + ", " + pesel[3]
                            + ", " + pesel[4] + ", " + pesel[5] + ", " + pesel[6] + ", " + pesel[7] + ", " + pesel[8]
                            + ", " + pesel[9] + ", " + pesel[10]);
                    }
                }

                if (isPatient == "nie")
                {
                    ss.SpeakAsync("Złóż deklaracje");
                }

            }
        }
    }
}
