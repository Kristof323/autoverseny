using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DotRace
{
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();
        private int finishedCount = 0;
        private string winner = null;
        private readonly double startX = 10;
        private readonly double finishX = 805;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartRace();
        }

        private void StartRace()
        {
            StartButton.IsEnabled = false;
            StatusText.Text = "FUT A VERSENY...";
            finishedCount = 0;
            winner = null;

            ResetDots();
            CreateAnimation(RedDot, "Piros");
            CreateAnimation(WhiteDot, "Fehér");
            CreateAnimation(GreenDot, "Zöld");
        }

        private void ResetDots()
        {
            Canvas.SetLeft(RedDot, startX);
            Canvas.SetLeft(WhiteDot, startX);
            Canvas.SetLeft(GreenDot, startX);
        }

        private void CreateAnimation(Ellipse dot, string colorName)
        {
            double duration = 2.0 + rnd.NextDouble() * 3.0; // 2-5 mp
            var animation = new DoubleAnimation
            {
                From = startX,
                To = finishX,
                Duration = TimeSpan.FromSeconds(duration),
                AccelerationRatio = rnd.NextDouble() * 0.3,
                DecelerationRatio = rnd.NextDouble() * 0.3
            };

            animation.Completed += (s, args) => DotFinished(dot, colorName);
            dot.BeginAnimation(Canvas.LeftProperty, animation);
        }

        private void DotFinished(Ellipse dot, string colorName)
        {
            finishedCount++;

            if (winner == null)
                winner = colorName;

            if (finishedCount == 3)
            {
                StatusText.Text = $"{winner} NYERT!";
                StartButton.IsEnabled = true;

                // Pöttyök a célban maradnak
                Canvas.SetLeft(RedDot, finishX);
                Canvas.SetLeft(WhiteDot, finishX);
                Canvas.SetLeft(GreenDot, finishX);
            }
        }
    }
}
