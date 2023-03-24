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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.Windows.Shapes.Path;

namespace CircleClickingGame
{
    static class Engine
    {
        public static MainWindow MainWindow;       
        public static Random rng;
        
        //public static DispatcherTimer Timer; // wip
        //public static List<ClickableCircle> Circles;        
        public static List<HitObjectEvent> HitObjects;
        
        
        
        public static Stopwatch Stopwatch;
        public static MediaPlayer MediaPlayer;

        public static string MapPath;
        public static string MapName;
        public static string MapAudio;
        public static bool UseOsuSongsFolder;
        public static string OsuSongsPath = string.Empty;
        public static string PathFile = "PathFile.txt";

        public static double CS;
        public static double CircSize;
        public static double AR;
        public static double Preempt;
        public static double FadeIn;
        public static double HitWindow300;
        public static double HitWindow100;
        public static double HitWindow50;
        public static double OD;
        public static double HP;
        public static double FadeOutTime = 300;
        public static Key key1 = Key.Z;
        public static Key key2 = Key.X;

        public static bool isPaused;
        public static bool Abort;

        public static PlayerStats player;

        public static void MainInit(MainWindow m)
        {           
            MainWindow = m;
            UseOsuSongsFolder = TryLoadOsuPath();
        }
        static bool TryLoadOsuPath()
        {
            if (File.Exists(PathFile))
            {
                string toCheck;
                using (StreamReader sr = new StreamReader(PathFile))
                {
                    toCheck = sr.ReadLine();
                };
                if (File.Exists(toCheck))
                {
                    OsuSongsPath = toCheck;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public static void Default()
        {
            Abort = true;
            isPaused = false;
            rng = new Random();
            ClickableCircle.Circles = new Dictionary<int, ClickableCircle>();
            //Timer = new DispatcherTimer();
            HitObjects = new List<HitObjectEvent>();
            //Timer.Tick += Timer_Tick;
            Stopwatch = new Stopwatch();
            //Timer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: 1);
            MainWindow.PlayArea.Background = new SolidColorBrush(Colors.Black);
            MainWindow.PauseButton.Visibility = Visibility.Collapsed;
            MediaPlayer = new MediaPlayer();
            player = new PlayerStats(HitObjects.Count);
            MapPath = string.Empty;
            MapName = "No Beatmap Loaded";
            MapAudio = string.Empty;
            StatsUpdate(false);
            UpdatePlayerLabel(true);

        }
        public static void SoftReset()
        {
            Abort = true;
            isPaused = false;
            rng = new Random();
            ClickableCircle.Circles = new Dictionary<int, ClickableCircle>();
            //Timer = new DispatcherTimer();
            HitObjects = new List<HitObjectEvent>();
            //Timer.Tick += Timer_Tick;
            Stopwatch = new Stopwatch();
            //Timer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: 1);
            MainWindow.PlayArea.Background = new SolidColorBrush(Colors.Black);
            MainWindow.PauseButton.Visibility = Visibility.Collapsed;
            MediaPlayer = new MediaPlayer();
            player = new PlayerStats(HitObjects.Count);

            LoadMap();
            UpdatePlayerLabel(true);

        }
        public static void StatsUpdate(bool OK)
        {
            if (OK)
            {
                MainWindow.StatsLabel.Content = $"CS : {CircSize} {Environment.NewLine}AR : {AR} {Environment.NewLine}OD : {OD} {Environment.NewLine}Circles: {HitObjects.Count}";
            }
            else
            {
                MainWindow.StatsLabel.Content = "No map info available";
            }
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
                //Stroke = new SolidColorBrush(Colors.White),
                //StrokeThickness = 4,
                Tag = order,
                Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/hitcircle.png"))),
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
                Stroke = new SolidColorBrush(Colors.Azure),
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
            //approach.Stop();
            MainWindow.PlayArea.Children.Remove(circle);
            //approach.Start();
            if (circ.isAlive)
            {
                while(approach.ElapsedMilliseconds < Preempt + HitWindow50 && circ.isAlive)
                {
                     await Task.Delay(1);                 
                }               
            }
            approach.Stop();
            if (circ.isAlive)
            {
                Engine.player.Miss();
                circ.Score = 0;
                //MessageBox.Show("miss");
            }
            circ.isAlive = false;
            
            if(circ.Score == 0)
            {
                DrawResult(circ);
            }
            else
            {
                bool resultDrew = false;
                if (circ.Score == 300)
                {
                    resultDrew = true;
                }
                approach.Reset();

                approach.Start();
                while (approach.ElapsedMilliseconds < FadeOutTime)
                {
                    if (approach.ElapsedMilliseconds >= FadeOutTime / 2 && !resultDrew)
                    {
                        resultDrew = true;
                        DrawResult(circ);
                    }
                    MainCircle.Height = 0.4 * CS * ((double)(approach.ElapsedMilliseconds / (double)FadeOutTime)) + CS;
                    MainCircle.Width = 0.4 * CS * ((double)(approach.ElapsedMilliseconds / (double)FadeOutTime)) + CS;
                    Canvas.SetTop(MainCircle, y + (CS / 2) - MainCircle.Height / 2);
                    Canvas.SetLeft(MainCircle, x + (CS / 2) - MainCircle.Width / 2);
                    MainCircle.Opacity = ((double)(FadeOutTime - approach.ElapsedMilliseconds) / (double)FadeOutTime) - 0.1;
                    await Task.Delay(1);
                }
                approach.Stop();
            }           
            
            MainWindow.PlayArea.Children.Remove(MainCircle);

            //DrawResult(circ);
            
            UpdatePlayerLabel(false);
            //ClickableCircle.Circles.Remove(circ.ID);
            approach = null;
        }
        async static void DrawResult(ClickableCircle circle)
        {
            double AnimationLength = 500;
            Image result = new Image()
            {
                Source = GetImageAfterScore(circle.Score),
                Width = CS / 3,
                Height = CS / 3,
                Opacity = 1
            };
            MainWindow.PlayArea.Children.Add(result);
            Canvas.SetLeft(result, circle.Xpos + CS / 6 + result.Width / 2); 
            Canvas.SetTop(result, circle.Ypos + CS / 6 + result.Height / 2);
            Canvas.SetZIndex(result, HitObjects.Count + 1);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while(sw.ElapsedMilliseconds <= AnimationLength)
            {
                result.Opacity = (AnimationLength - sw.ElapsedMilliseconds) / AnimationLength;
                await Task.Delay(1);
            }
            sw.Stop();
            sw = null;
            MainWindow.PlayArea.Children.Remove(result);

        }
        public static BitmapImage GetImageAfterScore(int score)
        {
            return score switch
            {               
                100 => new BitmapImage(new Uri("pack://application:,,,/Images/hit100.png")),
                50 => new BitmapImage(new Uri("pack://application:,,,/Images/hit50.png")),
                _ => new BitmapImage(new Uri("pack://application:,,,/Images/miss.png"))
            };
        }
        public static void UpdatePlayerLabel(bool Hide)
        {
            if (Hide)
            {
                MainWindow.ScoreLabel.Content = string.Empty;
                MainWindow.AccuracyLabel.Content = string.Empty;
                MainWindow.ComboLabel.Content = string.Empty;
            }
            else
            {
                MainWindow.ScoreLabel.Content = player.Score.ToString();
                MainWindow.AccuracyLabel.Content = Math.Round(player.Accuracy * 100, 2).ToString() + "%";
                MainWindow.ComboLabel.Content = player.Combo.ToString() + "x";
            }            
        }
        async public static void SpawnSlider(HitObjectEvent HitObj, int ID)
        {
            SpawnCircle((int)HitObj.coords.X, (int)HitObj.coords.Y, ID);
            //DrawSlider(HitObj.coords,HitObj.Props);
        }
        async public static void DrawSlider(Point start, string[] props)
        {
              
        }

        private static void Circle_ClickCheck(object sender, MouseButtonEventArgs e)
        {
            ClickableCircle.ClickCheck(Convert.ToInt32((sender as Ellipse).Tag));
        }
        public static async void Run()
        {
            int j = 0;
            Stopwatch.Start();
            MediaPlayer.Position = new TimeSpan(0, 0, 0, 0, HitObjects[0].Time - 500);
            while(j < HitObjects.Count)
            {
                if (Abort)
                {
                    MediaPlayer.Stop();
                    //MessageBox.Show("Aborted past beatmap.");
                    return;
                }
                if (Stopwatch.ElapsedMilliseconds + HitObjects[0].Time - 500 >= HitObjects[j].Time)
                {
                    if ((HitObjects[j].Type & 1 ) > 0)
                    {
                        SpawnCircle((int)HitObjects[j].coords.X, (int)HitObjects[j].coords.Y, j);
                    }
                    else if((HitObjects[j].Type & 2) > 0)
                    {
                        SpawnCircle((int)HitObjects[j].coords.X, (int)HitObjects[j].coords.Y, j);
                    }
                    j++;
                }
                else await Task.Delay(1);
            }
            if (Abort)
            {
                MediaPlayer.Stop();
                //MessageBox.Show("Aborted past beatmap.");
                return;
            }
            await Task.Delay(5000);
            Stopwatch.Stop();
            //ResultWindow.ShowResults(player);
            Engine.SoftReset();
            MainWindow.StartButton.Visibility = Visibility.Visible;
            MainWindow.PauseButton.Visibility = Visibility.Collapsed;         
        }
        public static bool LoadMap()
        {
            try
            {
                if (MapPath == null || MapPath == string.Empty)
                {
                    MainWindow.label1.Content = "No beatmap loaded";
                    return false;
                }
                MainWindow.label1.Content = MapName;
                StreamReader sr = new StreamReader(MapPath);
                bool begin = false;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.Contains("AudioFilename:"))
                    {
                        string name = line.Split(':')[1].Trim();
                        MapAudio = MapPath.Replace(MapPath.Split(@"\").Last(), name);
                        if (File.Exists(MapAudio))
                        {
                            MediaPlayer.Open(new Uri(MapAudio));
                            //MessageBox.Show("Map audio loaded!");
                        }
                        else
                        {
                            //MessageBox.Show("Map audio not found!");
                        }
                    }
                    if (line == "[Difficulty]")
                    {
                        HP = double.Parse(sr.ReadLine().Split(':').Last());
                        CircSize = double.Parse(sr.ReadLine().Split(':').Last());
                        OD = double.Parse(sr.ReadLine().Split(':').Last());
                        AR = double.Parse(sr.ReadLine().Split(':').Last());
                    }
                    if (!begin && line == "[HitObjects]")
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
                        if ((type & 2) > 0)
                        {
                            pars[0] = properties[5]; // curvee
                            pars[1] = properties[6]; // slides 
                            pars[2] = properties[7]; // length
                        }
                        HitObjects.Add(new HitObjectEvent(x, y, time, type, pars));
                    }
                }
                Preempt = 1250 - 750 * ((AR - 5) / 5);
                FadeIn = 800 - 500 * ((AR - 5) / 5);
                HitWindow300 = -6 * (OD - 13.75);
                HitWindow100 = -8 * (OD - 17.4375);
                HitWindow50 = -10 * (OD - 19.95);

                CS = 109 - (9 * CircSize);
                //Abort = false;
                player = new PlayerStats(HitObjects.Count);
                StatsUpdate(true);
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load map.");
                return false;
            }
        }
    }
    class ClickableCircle
    {
        public static Dictionary<int,ClickableCircle> Circles = new Dictionary<int, ClickableCircle>();
        public int ID { get; }
        public int Score { get; set; }
        public double Xpos { get; }
        public double Ypos { get; }
        public Ellipse parent { get; }
        public Ellipse ApproachCircle { get; }
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
            Xpos = Canvas.GetLeft(main);
            Ypos = Canvas.GetTop(main);
        }
        async public static void ClickCheck(int ID)
        {
            ClickableCircle check = Circles[ID];
            if (check.isAlive)
            {
                if(check.sw.ElapsedMilliseconds + Engine.HitWindow300 >= Engine.Preempt && check.sw.ElapsedMilliseconds <= Engine.Preempt + Engine.HitWindow300 )
                {
                    check.isAlive = false;
                    check.Score = 300;
                    Engine.player.AddScore(300);                                       
                    return;
                }
                else if(check.sw.ElapsedMilliseconds + Engine.HitWindow100 >= Engine.Preempt && check.sw.ElapsedMilliseconds <= Engine.Preempt + Engine.HitWindow100)
                {
                    check.isAlive = false;
                    check.Score = 100;
                    Engine.player.AddScore(100);
                    return;
                }
                else if(check.sw.ElapsedMilliseconds + Engine.HitWindow50 >= Engine.Preempt && check.sw.ElapsedMilliseconds <= Engine.Preempt + Engine.HitWindow50)
                {
                    check.isAlive = false;
                    check.Score = 50;
                    Engine.player.AddScore(50);
                    return;
                }
                //else
                //{
                //    check.Score = 0;
                //    check.isAlive = false;
                //    Engine.player.Miss();
                //    //ShakeAnimation() - todo
                //}
            }
        }
    }
    class PlayerStats
    {
        public double HP { get; set; }
        public int Score { get; set; }
        public int Combo { get; set; }
        public int ObjectsHit300 { get; set; }
        public int ObjectsHit100 { get; set; }
        public int ObjectsHit50 { get; set; }
        public int ObjectsMiss { get; set; }
        public double Accuracy { get; set; }
        public int TotalObj { get; set; }
        public PlayerStats(int TotalObj)
        {
            HP = 100;
            Score = 0;
            Combo = 0;
            ObjectsHit300 = 0;
            ObjectsHit100 = 0;
            ObjectsHit50 = 0;
            ObjectsMiss = 0;
            Accuracy = 100;
            this.TotalObj = TotalObj;
        }
        public void CalcStats()
        {
            double totalobj = ObjectsMiss + ObjectsHit50 + ObjectsHit100 + ObjectsHit300;
            Accuracy = ((double)ObjectsHit300 + 0.66 * (double)ObjectsHit100 + 0.33 * (double)ObjectsHit50) / totalobj;
        }
        public void Miss()
        {
            Combo = 0;
            ObjectsMiss++;
            CalcStats();
        }
        public void AddScore(int pts)
        {
            Combo++;
            Score += pts * Combo;
            switch (pts)
            {
                case 300:
                    ObjectsHit300++;
                    break;
                case 100:
                    ObjectsHit100++;
                    break;
                case 50:
                    ObjectsHit50++;
                    break;
                default:break;
            }
            CalcStats();
        }
    }
    class HitObjectEvent
    {
        public Point coords { get; }
        public int Time { get; }
        public int Type { get; }
        public string[] Props { get; }
        public HitObjectEvent(int x, int y, int time, int type, string[] pars)
        {
            coords = new Point(x, y);
            Time = time;
            Type = type;
            Props = pars;
        }
    }
}
