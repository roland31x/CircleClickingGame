using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public static List<HitObjectEvent> HitObjects;
        public static Stopwatch Stopwatch = new Stopwatch();
        public static int CS = 60;
        public static int AR = 1;
        public static int OD = 8;
        public static int HP = 8;
        public static void Init(MainWindow m)
        {
            MainWindow = m;
            rng = new Random();
            Circles = new List<ClickableCircle>();
            Timer = new DispatcherTimer();
            HitObjects = new List<HitObjectEvent>();
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0,0,0,0,milliseconds: 1);
            m.PlayArea.Background = new SolidColorBrush(Colors.Black);
        }

        private static void Timer_Tick(object? sender, EventArgs e)
        {
            //(sender as DispatcherTimer).
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
            Canvas.SetTop(MainCircle, y);
            Canvas.SetLeft(MainCircle, x);

            Ellipse circle = new Ellipse()
            {
                Height = CS*6,
                Width = CS*6,
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 2,
                Opacity = 0
                
            };
            MainWindow.PlayArea.Children.Add(circle);
            Canvas.SetTop(circle, y + CS / 2);
            Canvas.SetLeft(circle, x + CS / 2);
            Canvas.SetZIndex(MainCircle, HitObjects.Count + 10 - ClickableCircle.Circles.Count);
            Canvas.SetZIndex(circle, HitObjects.Count + 10 - ClickableCircle.Circles.Count);
            ClickableCircle circ = new ClickableCircle(MainCircle, circle);

            while(circle.Height > MainCircle.Height && circ.isAlive)
            {
                Canvas.SetTop(circle, y + (CS / 2) - circle.Height / 2 );
                
                Canvas.SetLeft(circle, x + (CS / 2) - circle.Width / 2 );
                circle.Opacity = (6*CS - circle.Width) / (4 * CS);
                //MessageBox.Show(circle.Opacity.ToString());
                circle.Height -= 1;
                circle.Width -= 1;
                await Task.Delay(AR);
            }
            MainWindow.PlayArea.Children.Remove(circle);
            if (circ.isAlive)
            {
                for(int i = 0; i < 3; i++)
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
            
            MainWindow.PlayArea.Children.Remove(MainCircle);
            //ClickableCircle.Circles.Remove(circ.ID);
            MainCircle = null;
            circle = null;

        }

        private static void Circle_ClickCheck(object sender, MouseButtonEventArgs e)
        {
            ClickableCircle.ClickCheck(Convert.ToInt32((sender as Ellipse).Tag));
        }
        public static async void Run()
        {
            int j = 0;
            Stopwatch.Start();
            while(j < HitObjects.Count)
            {
                if (Stopwatch.ElapsedMilliseconds >= HitObjects[j].Time)
                {
                    SpawnCircle((int)HitObjects[j].coords.X, (int)HitObjects[j].coords.Y, j);
                    j++;
                }
                else await Task.Delay(1);
            }
        }
        public static void LoadMap()
        {
            if(MapPath == null || MapPath == string.Empty)
            {
                return;
            }
            StreamReader sr = new StreamReader(MapPath);
            bool begin = false;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if(!begin && line == "[HitObjects]")
                {
                    begin = true;
                    continue;
                }
                if (begin)
                {
                    string[] properties = line.Split(',');
                    int x = int.Parse(properties[0]);
                    int y = int.Parse(properties[1]);
                    int time = int.Parse(properties[2]);
                    HitObjects.Add(new HitObjectEvent(x, y, time));
                }
            }
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
                    //ShakeAnimation() - todo
                    return;
                }
                //Engine.MainWindow.label1.Content = ID.ToString();
                
                check.isAlive = false;
            }
        }
    }
    class HitObjectEvent
    {
        public Point coords { get; set; }
        public int Time { get; set; }

        public HitObjectEvent(int x, int y, int time)
        {
            coords = new Point(x, y);
            Time = time;
        }
    }
}
