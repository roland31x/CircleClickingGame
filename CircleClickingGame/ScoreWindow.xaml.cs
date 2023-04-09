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
using System.Windows.Shapes;

namespace CircleClickingGame
{
    /// <summary>
    /// Interaction logic for ScoreWindow.xaml
    /// </summary>
    public partial class ScoreWindow : Window
    {
        public ScoreWindow()
        {
            InitializeComponent();
            MapNamebox.Content = Engine.MapName;
            X300sbox.Content = Engine.player.ObjectsHit300.ToString() + "x";
            X100sbox.Content = Engine.player.ObjectsHit100.ToString() + "x";
            X50sbox.Content = Engine.player.ObjectsHit50.ToString() + "x";
            Missesbox.Content = Engine.player.ObjectsMiss.ToString() + "x";
            Combobox.Content = "Max Combo: " + Engine.player.MaxCombo.ToString();
            Accuracybox.Content = "Accuracy: " + Engine.player.Accuracy;
            ScoreBox.Content = Engine.player.Score.ToString();
            if (Engine.player.Acc >= 90 && Engine.player.ObjectsMiss == 0)
            {
                    ResultScore.Content = 'S';
                    ResultScore.Foreground = Brushes.Orange;
            }
            else if (Engine.player.Acc >= 90 || (Engine.player.Acc >= 80 && Engine.player.ObjectsMiss == 0))
            {
                ResultScore.Content = 'A';
                ResultScore.Foreground = Brushes.Green;
            }
            else if (Engine.player.Acc >= 80 || (Engine.player.Acc >= 70 && Engine.player.ObjectsMiss == 0))
            {
                ResultScore.Content = 'B';
                ResultScore.Foreground = Brushes.Blue;
            }
            else if (Engine.player.Acc >= 60)
            {
                ResultScore.Content = 'C';
                ResultScore.Foreground = Brushes.Pink;
            }
            else
            {
                ResultScore.Content = 'D';
                ResultScore.Foreground = Brushes.Red;
            }
            if (Engine.player.HasFailed)
            {
                ResultScore.Content = 'F';
                ResultScore.Foreground = Brushes.DarkRed;
            }
        }
    }
}
