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
        //int test1 = 0;
        public MainWindow()
        {
            InitializeComponent();
            Engine.Init(this);
        }

        private async void BeatMap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "osu! files (*.osu)|*.osu";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowDialog();
            Engine.MapPath = openFileDialog.FileName;
            Engine.MapName = openFileDialog.FileName.Split(@"\").Last().Split('.').First();
            label1.Content = Engine.MapName;
            Engine.LoadMap();
            //for(int i = 0; i < 50; i++)
            //{
            //    int x = Engine.rng.Next(100, (int)Width - 100);
            //    int y = Engine.rng.Next(100, (int)Height - 100);
            //    Engine.SpawnCircle(x,y,i);
            //    await Task.Delay(400);
            //}
            StartButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            Engine.isPaused = false;
            PauseButton.Content = "STOP";
            SongButton.IsEnabled = true;
        }
        private void Song_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Engine.MapPath;
            openFileDialog.Filter = "mp3 files (*.mp3)|*.mp3";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowDialog();
            if(openFileDialog.FileName != null && openFileDialog.FileName != string.Empty)
            {
                Engine.MediaPlayer.Open(new Uri(openFileDialog.FileName));
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
