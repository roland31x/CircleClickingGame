using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace CircleClickingGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartButton.IsEnabled = false;
            PauseButton.IsEnabled = false;
            Engine.MainInit(this);
        }

        private async void BeatMap_Click(object sender, RoutedEventArgs e)
        {
            Engine.Default();
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "osu! files (*.osu)|*.osu";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowDialog();
            Engine.MapPath = openFileDialog.FileName;
            Engine.MapName = openFileDialog.FileName.Split(@"\").Last().Split('.').First();
            if (Engine.LoadMap())
            {
                StartButton.IsEnabled = true;
                PauseButton.IsEnabled = false;
                PauseButton.Content = "STOP";
                Engine.Abort = false;
            }
            else
            {
                StartButton.IsEnabled = false;
                PauseButton.IsEnabled = false;
                Engine.UpdatePlayerLabel(true);
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Engine.Abort = false;

            Engine.MediaPlayer.Play();

            Engine.Run();
            (sender as Button).IsEnabled = false;
            PauseButton.IsEnabled = true;
            //Engine.SpawnCircle(400, 400, test1++);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Engine.isPaused)
            {
                Engine.MediaPlayer.Play();

                Engine.Stopwatch.Start();
                Engine.isPaused = false;
                (sender as Button).Content = "STOP";
            }
            else
            {
                Engine.MediaPlayer.Pause();

                Engine.Stopwatch.Stop();
                Engine.isPaused = true;
                (sender as Button).Content = "RESUME";
            }
        }
    }
}
