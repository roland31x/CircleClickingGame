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
            Combobox.Content = Engine.player.MaxCombo.ToString() + " / " + Engine.HitObjects.Count;
            Accuracybox.Content = "Accuracy: " + (Math.Round(Engine.player.Accuracy, 2) * 100).ToString() + "%";
            ScoreBox.Content = Engine.player.Score.ToString();
        }
    }
}
