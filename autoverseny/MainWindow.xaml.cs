using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DotRace
{
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();
        private int finishedCount = 0;
        private string winner = null;
        private readonly double startX = 10;
        private readonly double finishX = 840;

        private DispatcherTimer countdownTimer;
        private DispatcherTimer raceTimer;
        private DateTime raceStartTime;
        private List<RaceResult> results = new List<RaceResult>();

        public class RaceResult
        {
            public string Color { get; set; }
            public int Place { get; set; }
            public double TimeSeconds { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimers();
        }

        private void InitializeTimers()
        {
          
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTimer_Tick;

            raceTimer = new DispatcherTimer();
            raceTimer.Interval = TimeSpan.FromMilliseconds(100);
            raceTimer.Tick += RaceTimer_Tick;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartCountdown();
        }

        private void StartCountdown()
        {
            StartButton.IsEnabled = false;
            StatusText.Text = "";
            TimesText.Text = "";
            CountdownText.Text = "3";
            CountdownText.Visibility = System.Windows.Visibility.Visible;

            countdownTimer.Tag = 3; // Kezdőérték
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            int count = (int)countdownTimer.Tag;
            if (count > 1)
            {
                CountdownText.Text = (count - 1).ToString();
                countdownTimer.Tag = count - 1;
            }
            else
            {
              
                countdownTimer.Stop();
                CountdownText.Visibility = System.Windows.Visibility.Hidden;
                StartRace();
            }
        }

        private void StartRace()
        {
            StatusText.Text = "FUT A VERSENY...";
            finishedCount = 0;
            winner = null;
            results.Clear();

            raceStartTime = DateTime.Now;
            raceTimer.Start();

            ResetDots();
            CreateAnimation(RedDot, "Piros");
            CreateAnimation(WhiteDot, "Fehér");
            CreateAnimation(GreenDot, "Zöld");
        }

        private void RaceTimer_Tick(object sender, EventArgs e)
        {
            
        }

        private void ResetDots()
        {
            Canvas.SetLeft(RedDot, startX);
            Canvas.SetLeft(WhiteDot, startX);
            Canvas.SetLeft(GreenDot, startX);
        }

        private void CreateAnimation(Ellipse dot, string colorName)
        {
            double duration = 2.5 + rnd.NextDouble() * 3.5;
            var animation = new DoubleAnimation
            {
                From = startX,
                To = finishX,
                Duration = TimeSpan.FromSeconds(duration),
                AccelerationRatio = rnd.NextDouble() * 0.4,
                DecelerationRatio = rnd.NextDouble() * 0.4
            };

            animation.Completed += (s, args) => DotFinished(dot, colorName);
            dot.BeginAnimation(Canvas.LeftProperty, animation);
        }

        private void DotFinished(Ellipse dot, string colorName)
        {
            finishedCount++;
            double finishTime = (DateTime.Now - raceStartTime).TotalSeconds;

            results.Add(new RaceResult
            {
                Color = colorName,
                Place = finishedCount,
                TimeSeconds = Math.Round(finishTime, 2)
            });

            if (winner == null)
                winner = colorName;

            if (finishedCount == 3)
            {
                raceTimer.Stop();
                ShowResults();
                SaveResultsToFile();
                StartButton.IsEnabled = true;

                Canvas.SetLeft(RedDot, finishX);
                Canvas.SetLeft(WhiteDot, finishX);
                Canvas.SetLeft(GreenDot, finishX);
            }
        }

        private void ShowResults()
        {
            results = results.OrderBy(r => r.Place).ToList();
            string times = "";
            for (int i = 0; i < results.Count; i++)
            {
                times += $"{results[i].Place}. {results[i].Color}: {results[i].TimeSeconds}s\n";
            }
            StatusText.Text = $"{winner} NYERT!";
            TimesText.Text = $"Eredmények:\n{times}";
        }

        private void SaveResultsToFile()
        {
            try
            {
                results = results.OrderBy(r => r.Place).ToList();
                string content = $"Verseny eredménye - {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
                content += "Helyezés | Szín | Idő\n";
                content += "------------------------\n";

                foreach (var result in results)
                {
                    content += $"{result.Place}.         | {result.Color,-6} | {result.TimeSeconds,6:F2}s\n";
                }
                content += "\n------------------------\n";

                File.WriteAllText("adatok.txt", content);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fájl hiba: {ex.Message}");
            }
        }
    }
}
