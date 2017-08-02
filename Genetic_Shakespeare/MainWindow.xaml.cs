using System;
using System.Linq;
using System.Windows;
using Genetic_Shakespeare.Genetic;
using System.Windows.Threading;

namespace Genetic_Shakespeare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string targetString = "No, I'm not from Compton. England is my city!";

        string validChars = "1234567890QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnmēūīāšģķļžčņ,./!@#$%^&*()?><)_+-=' ";
        int populationSize = 200;
        float mutationRate = 0.01f;

        bool isValidString;

        Algorithm<char> ga;
        Random random = new Random();

        DispatcherTimer Updater;


        public MainWindow()
        {
            InitializeComponent();

            Updater = new DispatcherTimer();
            Updater.Interval = TimeSpan.FromMilliseconds(16);
            Updater.Tick += Updater_Tick;

            //Check, if validChars contains all characters from targetString
            isValidString = !targetString.AsEnumerable().Except(validChars.AsEnumerable()).Any();
            if(!isValidString)
            {
                Console.WriteLine("WARNING! Target string has unsupported characters!");
            }

            TargetPhrase.Text = $"Target: {targetString}";
            ga = new Algorithm<char>(populationSize, targetString.Length, random, GetRandomChar, FitnessFunction, mutationRate);
            StartUpdater(true);
        }

        void StartUpdater(bool s)
        {
            if(s == true)
            {
                Updater.Start();
                PlayButton.Content = "Stop";
            }
            else
            {
                Updater.Stop();
                PlayButton.Content = "Start";
            }
            
        }

        private void Updater_Tick(object sender, EventArgs e)
        {
            ga.NewGeneration();
            UpdateText();
            UpdateMonitor();
            if (ga.BestFitness == 1)
            {
                StartUpdater(false);
                return;
            }

        }

        void UpdateMonitor()
        {
            InfoMon.Text = $"" +
                $"Generation: {ga.Generation}, " +
                $"Fitness: {Math.Round(ga.BestFitness * 100)}";
        }

        private void UpdateText()
        {
            string line = "";

            foreach(char c in ga.BestGenes)
            {
                line += c;
            }

            if(line == targetString)
            {
                StartUpdater(false);
            }

            GeneratedList.Items.Add(line);
            ListScroller.ScrollToBottom();
        }

        private char GetRandomChar()
        {
            int i = random.Next(validChars.Length);
            return validChars[i];
        }

        private float FitnessFunction(int index)
        {
            float score = 0;
            DNA<char> dna = ga.Population[index];

            for (int i = 0; i < dna.Genes.Length; i++)
            {
                if(dna.Genes[i] == targetString[i])
                {
                    score += 1;
                }
            }

            score /= targetString.Length;
            return score;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(PlayButton.Content.ToString() == "Start")
            {
                StartUpdater(true);
            }
            else
            {
                StartUpdater(false);
            }
        }
    }
}
