using Microsoft.Win32;
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

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainWindow_Loaded(sender, e);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double Scale = ActualWidth / ButtonCanvas.Width;
            PlayArea.Background = Brushes.Beige;
            PlayArea.Height = 0.8 * ActualHeight;
            PlayArea.Width = (4d / 3d) * PlayArea.Height;
            Canvas.SetTop(PlayArea, (ActualHeight - PlayArea.Height) / 2);
            Canvas.SetLeft(PlayArea, (ActualWidth - PlayArea.Width) / 2);


            ResizeButtons(Scale);
            ResizeHud(Scale);


            Engine.SoftReset();
        }
        void ResizeButtons(double Scale)
        {            
            ButtonCanvas.Height = ActualHeight;
            ButtonCanvas.Width = ActualWidth;
            foreach(Button b in ButtonCanvas.Children.OfType<Button>())
            {
                Canvas.SetTop(b, Canvas.GetTop(b) * Scale);
                Canvas.SetLeft(b, Canvas.GetLeft(b) * Scale);
                b.Width = b.Width * Scale;
                b.Height = b.Height * Scale;
                b.FontSize = b.FontSize * Scale;
            }
        }
        void ResizeHud(double Scale)
        {            
            foreach (Label b in PlayerCanvas.Children.OfType<Label>())
            {
                b.FontSize = b.FontSize * Scale;

                Canvas.SetTop(b, Canvas.GetTop(b) * Scale);
                if (Canvas.GetLeft(b) != 0)
                {                   
                    Canvas.SetLeft(b, Canvas.GetLeft(b) * Scale);
                }
                else
                {
                    Canvas.SetRight(b, Canvas.GetRight(b) * Scale);
                }               

            }
            Canvas.SetTop(label1, Canvas.GetTop(label1) * Scale);
            Canvas.SetLeft(label1, Canvas.GetLeft(label1) * Scale);

            label1.Width = label1.Width * Scale;
            label1.Height = label1.Height * Scale;
            label1.FontSize = label1.FontSize * Scale;

            MKey1Label.Width *= Scale;
            MKey2Label.Width *= Scale;
            Key1Label.Width *= Scale;
            Key2Label.Width *= Scale;
            Key2Label.Height *= Scale;
            Key1Label.Height *= Scale;
            MKey2Label.Height *= Scale;
            MKey1Label.Height *= Scale;


            PlayerCanvas.Height = ActualHeight;
            PlayerCanvas.Width = ActualWidth;
        }

        void MainInit()
        {
            Loaded += MainWindow_Loaded;
            SizeChanged += MainWindow_SizeChanged;
            StartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;
            MouseWheel += MainWindow_MouseWheel;
            //this.Cursor = Cursors.None;
            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;
            this.Cursor = new Cursor("Assets/RedCursor.cur", true);
            Engine.MainInit(this);
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                Engine.MButton1IsHeld = false;
                Engine.MainWindow.MKey1Label.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (e.RightButton == MouseButtonState.Released)
            {
                Engine.MButton2IsHeld = false;
                Engine.MainWindow.MKey2Label.Background = new SolidColorBrush(Colors.Transparent);
            }

        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Engine.MButton1IsHeld = true;
                Engine.MainWindow.MKey1Label.Background = new SolidColorBrush(Colors.LightPink);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Engine.MButton2IsHeld = true;
                Engine.MainWindow.MKey2Label.Background = new SolidColorBrush(Colors.LightPink);
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Engine.key1)
            {
                Engine.Button1IsHeld = false;
                Key1Label.Background = new SolidColorBrush(Colors.Transparent);
            }
            else if(e.Key == Engine.key2)
            {
                Engine.Button2IsHeld = false;
                Key2Label.Background = new SolidColorBrush(Colors.Transparent);
            }          
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
            if(e.Key == Key.Escape)
            {
                if(MessageBox.Show("You sure you want to quit?", "Quit?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.Close();
                }
                else { return; }
            }
            if (e.IsRepeat)
            {
                return;
            }
            if(e.Key == Engine.key1 || e.Key == Engine.key2)
            {
                if(e.Key == Engine.key1)
                {
                    Engine.Button1IsHeld = true;
                    Key1Label.Background = new SolidColorBrush(Colors.Yellow);
                }
                else
                {
                    Engine.Button2IsHeld = true;
                    Key2Label.Background = new SolidColorBrush(Colors.Yellow);
                }

                foreach (Ellipse v in PlayArea.Children.OfType<Ellipse>())
                {
                    if (v.IsMouseDirectlyOver && v.Tag is Clickable)
                    {
                        (v.Tag as Clickable).Click();
                    }
                }
            }           
        }

        private async void BeatMap_Click(object sender, RoutedEventArgs e)
        {
            Engine.Default();
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select the beatmap",
                InitialDirectory = Engine.OsuSongsPath,
                Filter = "osu! files (*.osu)|*.osu",
                FilterIndex = 1,
                RestoreDirectory = false,
            };          
            openFileDialog.ShowDialog();
            Engine.MapPath = openFileDialog.FileName;
            Engine.MapName = openFileDialog.FileName.Split(@"\").Last().Split('.').First();
            if (Engine.LoadMap())
            {
                StartButton.Visibility = Visibility.Visible;
            }
            else
            {
                StartButton.Visibility = Visibility.Collapsed;
                Engine.player.Hide();
            }
            await Task.Delay(200);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Engine.Abort = false;

            Engine.MediaPlayer.Play();

            Engine.Run();

            Engine.player.Show();

            StartButton.Visibility = Visibility.Collapsed;

            PauseButton.Visibility = Visibility.Visible;

            BeatmapButton.Visibility = Visibility.Collapsed;

            SettingsButton.Visibility = Visibility.Collapsed;

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Engine.Abort = true;
            PauseButton.IsEnabled = false;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow setwin = new SettingsWindow();
            setwin.ShowDialog();
            //testwindow t = new testwindow();
            //t.ShowDialog();
        }

    }
}
