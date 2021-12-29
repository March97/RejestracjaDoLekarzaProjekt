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
        public MainWindow()
        {
            InitializeComponent();
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
            float confidence = e.Result.Confidence;
            if (confidence <= 0.3)
            {
                ss.SpeakAsync("Proszę powtórzyć");
            }
            else
            {
                int number = Convert.ToInt32(e.Result.Semantics["number"].Value);
                string decision = e.Result.Semantics["decision"].Value.ToString();
                if (decision == "tak")
                {
                    ss.SpeakAsync("Podaj numer pe-sel");
                }
                else
                if (decision == "nie")
                {
                    ss.SpeakAsync("Złóż deklaracje");
                }
                else
                {
                    ss.SpeakAsync("Proszę powtórzyć");
                }
            }
        }
    }
}
