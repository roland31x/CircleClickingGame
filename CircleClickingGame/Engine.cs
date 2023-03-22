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
        public static int CS = 109 - (9 * 5);
        public static int AR = 8;
        public static int Preempt;
        public static int FadeIn;
        public static int HitWindow;
        public static int OD = 5;
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
                Opacity = 0,
                
            };
            MainCircle.MouseDown += Circle_ClickCheck;
            MainWindow.PlayArea.Children.Add(MainCircle);
            Canvas.SetTop(MainCircle, y);
            Canvas.SetLeft(MainCircle, x);

            Ellipse circle = new Ellipse()
            {
                Height = CS * 6,
                Width = CS * 6,
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 4,
                Opacity = 0
                
            };
            MainWindow.PlayArea.Children.Add(circle);
            Canvas.SetTop(circle, y + CS / 2);
            Canvas.SetLeft(circle, x + CS / 2);
            Canvas.SetZIndex(MainCircle, HitObjects.Count + 10 - ClickableCircle.Circles.Count);
            Canvas.SetZIndex(circle, HitObjects.Count + 10 - ClickableCircle.Circles.Count);
            Stopwatch approach = new Stopwatch();

            ClickableCircle circ = new ClickableCircle(MainCircle, circle, approach);
            approach.Start();
            while(approach.ElapsedMilliseconds < Preempt && circ.isAlive)
            {
                if(approach.ElapsedMilliseconds < FadeIn)
                {
                    MainCircle.Opacity = (double)(approach.ElapsedMilliseconds / (double)(FadeIn));
                    circle.Opacity = (double)(approach.ElapsedMilliseconds / (double)(FadeIn));
                }             
                //MessageBox.Show(circle.Opacity.ToString());
                circle.Height = 5 * CS * (1 - (double)(approach.ElapsedMilliseconds / (double)Preempt)) + CS;
                circle.Width = 5 * CS * (1 - (double)(approach.ElapsedMilliseconds / (double)Preempt)) + CS;
                Canvas.SetTop(circle, y + (CS / 2) - circle.Height / 2);
                Canvas.SetLeft(circle, x + (CS / 2) - circle.Width / 2);

                await Task.Delay(1);
            }
            approach.Stop();
            MainWindow.PlayArea.Children.Remove(circle);
            approach.Start();
            if (circ.isAlive)
            {
                while(approach.ElapsedMilliseconds < Preempt + HitWindow && circ.isAlive)
                {
                    if (circ.isAlive)
                    {
                        await Task.Delay(1);
                    }
                    else break;
                   
                }               
            }
            approach.Stop();
            approach = null;
            if (circ.isAlive)
            {
                //MessageBox.Show("miss");
            }
            circ.isAlive = false;
            
            MainWindow.PlayArea.Children.Remove(MainCircle);
            //ClickableCircle.Circles.Remove(circ.ID);
            MainCircle = null;
            circle = null;
            circ = null;

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
                if (Stopwatch.ElapsedMilliseconds + HitObjects[0].Time - 500 >= HitObjects[j].Time)
                {
                    if ((HitObjects[j].Type & 1 ) > 0)
                    {
                        SpawnCircle((int)HitObjects[j].coords.X, (int)HitObjects[j].coords.Y, j);
                    }
                    else if((HitObjects[j].Type & 2) > 0)
                    {
                        //SpawnCircle((int)HitObjects[j].coords.X, (int)HitObjects[j].coords.Y, j);
                    }
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
                    int type = int.Parse(properties[3]);
                    string[] pars = new string[3];
                    if((type & 2) > 0)
                    {
                        pars[0] = properties[5]; // curvee
                        pars[1] = properties[6]; // slides 
                        pars[2] = properties[7]; // length
                    }
                    HitObjects.Add(new HitObjectEvent(x, y, time,type,pars));
                }
            }
            Preempt = 1250 - 750 * ((AR - 5) / 5);
            FadeIn = 800 - 500 * ((AR - 5) / 5);
            HitWindow = 140 - 8 * OD;
        }
    }
    class ClickableCircle
    {
        public static Dictionary<int,ClickableCircle> Circles = new Dictionary<int, ClickableCircle>();
        public int ID { get; set; }
        public Ellipse parent { get; set; }
        public Ellipse ApproachCircle { get; set; }
        public bool isAlive { get; set; }
        public Stopwatch sw { get; set; }
        public ClickableCircle(Ellipse main, Ellipse approach, Stopwatch timer) 
        {
            ID = Convert.ToInt32(main.Tag);
            parent = main;
            ApproachCircle = approach;
            isAlive = true;
            Circles.Add(ID,this);
            sw = timer;
        }
        public static void ClickCheck(int ID)
        {
            ClickableCircle check = Circles[ID];
            if (check.isAlive)
            {
                if(check.sw.ElapsedMilliseconds + Engine.HitWindow >= Engine.Preempt && check.sw.ElapsedMilliseconds <= Engine.Preempt + Engine.HitWindow )
                {
                    check.isAlive = false;
                    return;
                }               
                else
                {
                    //ShakeAnimation() - todo
                }
            }
        }
    }
    class HitObjectEvent
    {
        public Point coords { get; set; }
        public int Time { get; set; }
        public int Type { get; set; }
        public string[] Props { get; set; }
        public HitObjectEvent(int x, int y, int time, int type, string[] pars)
        {
            coords = new Point(x, y);
            Time = time;
            Type = type;
            Props = pars;
        }
    }
}
