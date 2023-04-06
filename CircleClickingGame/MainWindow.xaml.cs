﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
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
            MainInit();
        }
        void MainInit()
        {
            StartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;
            MouseWheel += MainWindow_MouseWheel;
            //this.Cursor = Cursors.None;
            this.KeyDown += MainWindow_KeyDown;
            Engine.MainInit(this);
        }

        async private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(Engine.MediaPlayer != null)
            {
                if (e.Delta > 0)
                {
                    Engine.MediaPlayer.Volume += 0.05;
                }
                else if (e.Delta < 0)
                {
                    Engine.MediaPlayer.Volume -= 0.05;
                }
                VolumeLabel.Visibility = Visibility.Visible;
                VolumeLabel.Content = "Volume: " + Math.Round(Engine.MediaPlayer.Volume * 100, 2).ToString() + "%";
                await Task.Delay(500);
                VolumeLabel.Visibility = Visibility.Collapsed;
            }          
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }
            if(e.Key == Engine.key1 || e.Key == Engine.key2)
            {
                foreach (Ellipse v in PlayArea.Children.OfType<Ellipse>())
                {
                    if (v.IsMouseDirectlyOver && (v.Tag is ClickableCircle || v.Tag is ClickableSlider))
                    {
                        if (v.Tag is ClickableSlider)
                        {
                            (v.Tag as ClickableSlider).Click();
                        }
                        else
                        {
                            (v.Tag as ClickableCircle).Click();
                        }                       
                    }
                }
            }           
        }

        private async void BeatMap_Click(object sender, RoutedEventArgs e)
        {
            Engine.Default();           
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select the beatmap";
            openFileDialog.InitialDirectory = Engine.OsuSongsPath;        
            openFileDialog.Filter = "osu! files (*.osu)|*.osu";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = false;
            openFileDialog.ShowDialog();
            Engine.MapPath = openFileDialog.FileName;
            Engine.MapName = openFileDialog.FileName.Split(@"\").Last().Split('.').First();
            if (Engine.LoadMap())
            {
                StartButton.Visibility = Visibility.Visible;
                PauseButton.Content = "STOP";
            }
            else
            {
                StartButton.Visibility = Visibility.Collapsed;
                Engine.UpdatePlayerLabel(true);
            }
            await Task.Delay(200);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Engine.Abort = false;

            Engine.MediaPlayer.Play();

            Engine.Run();

            StartButton.Visibility = Visibility.Collapsed;

            PauseButton.Visibility = Visibility.Visible;

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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //SettingsWindow setwin = new SettingsWindow();
            //setwin.ShowDialog();
            testwindow t = new testwindow();
            t.ShowDialog();
        }

    }
}
