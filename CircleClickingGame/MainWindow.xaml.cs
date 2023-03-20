using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        int test1 = 0;
        public MainWindow()
        {
            InitializeComponent();
            Engine.Init(this);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            test1++;
            //OpenFileDialog openFileDialog = new OpenFileDialog();

            //openFileDialog.InitialDirectory = "c:\\";
            //openFileDialog.Filter = "osu! files (*.txt)|*.txt";
            //openFileDialog.FilterIndex = 2;
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.ShowDialog();
            //Engine.MapPath = openFileDialog.FileName;
            //label1.Content = Engine.MapPath;
            for(int i = 0; i < 50; i++)
            {
                int x = Engine.rng.Next(100, (int)Width - 100);
                int y = Engine.rng.Next(100, (int)Height - 100);
                Engine.SpawnCircle(x,y,i);
                await Task.Delay(400);
            }
            
        }
    }
}
