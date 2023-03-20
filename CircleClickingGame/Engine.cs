using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CircleClickingGame
{
    static class Engine
    {
        public static string MapPath;
        public static Random rng;
        public static string MapName;
        public static DispatcherTimer Timer;
        public static List<ClickableCircle> Circles;
        public static MainWindow MainWindow;
        public static int CS = 100;
        public static void Init(MainWindow m)
        {
            MainWindow = m;
            rng = new Random();
            Circles = new List<ClickableCircle>();
            Timer = new DispatcherTimer();
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0,0,0,0,milliseconds: 100);
            m.PlayArea.Background = new SolidColorBrush(Colors.Black);
        }

        private static void Timer_Tick(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        async public static void SpawnCircle(int x, int y, int order)
        {
            Ellipse MainCircle = new Ellipse()
            {
                Height = CS,
                Width = CS,
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 4,
                Tag = order,
                Fill = new SolidColorBrush(Colors.DarkBlue),
                Opacity = 0.6,
                
            };
            MainCircle.MouseDown += Circle_ClickCheck;
            MainWindow.PlayArea.Children.Add(MainCircle);
            Canvas.SetTop(MainCircle, y - MainCircle.Height / 2);
            Canvas.SetLeft(MainCircle, x - MainCircle.Width / 2);

            Ellipse circle = new Ellipse()
            {
                Height = CS*4,
                Width = CS*4,
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 2,
                Opacity = 0
                
            };
            MainWindow.PlayArea.Children.Add(circle);
            Canvas.SetTop(circle, y);
            Canvas.SetLeft(circle, x);
            Canvas.SetZIndex(MainCircle, 1000 - ClickableCircle.Circles.Count);
            Canvas.SetZIndex(circle, 1000 - ClickableCircle.Circles.Count);
            ClickableCircle circ = new ClickableCircle(MainCircle, circle);

            while(circle.Height > MainCircle.Height && circ.isAlive)
            {
                Canvas.SetTop(circle, y - circle.Height/2);
                
                Canvas.SetLeft(circle, x - circle.Width/2);
                circle.Opacity = Math.Pow((circle.Width - CS) / 3 * CS , 4) ;
                circle.Height -= 1;
                circle.Width -= 1;
                await Task.Delay(1);
            }

            if (circ.isAlive)
            {
                for(int i = 0; i < 25; i++)
                {
                    if (circ.isAlive)
                    {
                        await Task.Delay(20);
                    }
                    else break;
                   
                }
                
            }
            if (circ.isAlive)
            {
                //MessageBox.Show("miss");
            }
            circ.isAlive = false;
            MainWindow.PlayArea.Children.Remove(circle);
            MainWindow.PlayArea.Children.Remove(MainCircle);
            MainCircle = null;
            circle = null;

        }

        private static void Circle_ClickCheck(object sender, MouseButtonEventArgs e)
        {
            ClickableCircle.ClickCheck(Convert.ToInt32((sender as Ellipse).Tag));
        }
    }
    class ClickableCircle
    {
        public static Dictionary<int,ClickableCircle> Circles = new Dictionary<int, ClickableCircle>();
        public int ID { get; set; }
        public Ellipse parent { get; set; }
        public Ellipse ApproachCircle { get; set; }
        public bool isAlive { get; set; }
        public ClickableCircle(Ellipse main, Ellipse approach) 
        {
            ID = Convert.ToInt32(main.Tag);
            parent = main;
            ApproachCircle = approach;
            isAlive = true;
            Circles.Add(ID,this);
        }
        public static void ClickCheck(int ID)
        {
            ClickableCircle check = Circles[ID];
            if (check.isAlive)
            {
                if(check.ApproachCircle.Width - check.parent.Width > 100)
                {
                    //MessageBox.Show("100");
                    //ShakeAnimation() - todo
                    return;
                }
                Engine.MainWindow.label1.Content = ID.ToString();
                check.isAlive = false;
            }
        }
    }
}
