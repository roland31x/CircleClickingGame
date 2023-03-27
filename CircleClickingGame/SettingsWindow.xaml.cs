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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        int isReadingKey = 0;
        public SettingsWindow()
        {
            InitializeComponent();
            btn1.Content = Engine.key1.ToString();
            btn2.Content = Engine.key2.ToString();
            KeyDown += SettingsWindow_KeyDown;
            if(Engine.OsuSongsPath != string.Empty)
            {
                pathfilebox.Content = Engine.OsuSongsPath;
            }

        }

        private void SettingsWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(isReadingKey > 0)
            {
                bool ok = true;
                if(e.Key == Key.Escape || e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.Capital)
                {
                    ok = false;
                }
                if(isReadingKey == 1)
                {
                    if(e.Key == Engine.key2)
                    {
                        ok = false;
                    }
                    if (ok)
                    {
                        Engine.key1 = e.Key;
                    }
                        
                    btn1.Content = Engine.key1.ToString();
                }
                else
                {
                    if(e.Key == Engine.key1)
                    {
                        ok = false;
                    }
                    if(ok)
                    {
                        Engine.key2 = e.Key;
                    }

                    btn2.Content = Engine.key2.ToString();
                }
                isReadingKey = 0;
            }
        }

        private void PathFileSet(object sender, RoutedEventArgs e)
        {

        }

        private void Key1Click(object sender, RoutedEventArgs e)
        {
            btn1.Content = Engine.key1.ToString();
            btn2.Content = Engine.key2.ToString();
            if(isReadingKey == 0)
            {
                btn1.Content = "PRESS ANY KEY";
                isReadingKey = 1;
            }
        }

        private void Key2Click(object sender, RoutedEventArgs e)
        {
            btn1.Content = Engine.key1.ToString();
            btn2.Content = Engine.key2.ToString();
            if (isReadingKey == 0)
            {
                btn2.Content = "PRESS ANY KEY";
                isReadingKey = 2;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).Content = "Saved!";
            KeyConverter kc = new KeyConverter();

            await Task.Delay(1000);
            (sender as Button).Content = "Save";
            
        }
    }
}
