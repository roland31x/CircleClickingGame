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
using System.Globalization;
using Path = System.Windows.Shapes.Path;
using System.Windows.Media.Animation;

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
        public static string OsuSongsPath;
        public static string PathFile = "User\\Settings.data";

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
        public static double DiffMultiplier;
        public static Key key1 = Key.Z;
        public static Key key2 = Key.X;

        public static bool isPaused;
        public static bool Abort;

        public static PlayerStats player;
        public static int SpawnedObj;

        public static void MainInit(MainWindow m)
        {           
            MainWindow = m;
            if (!TryLoadSettings())
            {
                MessageBox.Show("Problem occured while loading settings, resetting to default.");
                DefaultSave();
                TryLoadSettings();
            }
        }
        public static bool TryLoadSettings()
        {
            if (File.Exists(PathFile))
            {
                string toCheck;
                using (StreamReader sr = new StreamReader(PathFile))
                {
                    try
                    {
                        toCheck = sr.ReadLine();
                        if (Directory.Exists(toCheck))
                        {
                            OsuSongsPath = toCheck;
                        }
                        else
                        {
                            return false;
                        }
                        toCheck = sr.ReadLine();
                        Engine.key1 = (Key)Enum.Parse(typeof(Key), toCheck);
                        toCheck = sr.ReadLine();
                        Engine.key2 = (Key)Enum.Parse(typeof(Key), toCheck);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                };               
            }
            else
            {
                return false;
            }
        }
        public static void DefaultSave()
        {
            if (File.Exists(PathFile))
            {
                File.Delete(PathFile);
            }
            File.Create(PathFile).Dispose();
            using (StreamWriter sw = new StreamWriter(PathFile,true))
            {
                sw.WriteLine(Directory.GetCurrentDirectory());
                sw.WriteLine(((int)Key.Z));
                sw.WriteLine(((int)Key.X));
            }
        }
        public static void OverWriteSave()
        {
            File.Delete(PathFile);
            File.Create(PathFile).Dispose();
            using (StreamWriter sw = new StreamWriter(PathFile, true))
            {
                sw.WriteLine(OsuSongsPath);
                sw.WriteLine((int)Engine.key1);
                sw.WriteLine((int)Engine.key2);
            }
        }
        public static void Default()
        {
            Abort = true;
            isPaused = false;
            rng = new Random();
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
        public static async void Run()
        {
            int j = 0;
            SpawnedObj = 0;
            Stopwatch.Start();
            MediaPlayer.Position = new TimeSpan(0, 0, 0, 0, HitObjects[0].Time - 500);
            while(j < HitObjects.Count)
            {
                SpawnedObj = j;
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
                        //SpawnCircle((int)HitObjects[j].coords.X, (int)HitObjects[j].coords.Y, j);
                        new ClickableCircle((int)HitObjects[j].coords.X, (int)HitObjects[j].coords.Y).Spawn();
                    }
                    else if((HitObjects[j].Type & 2) > 0) // spawnslider actually but i need to implement it
                    {
                        ClickableCircle c = new ClickableCircle((int)HitObjects[j].coords.X, (int)HitObjects[j].coords.Y);
                        c.Spawn();
                        //SpawnCircle((int)HitObjects[j].coords.X, (int)HitObjects[j].coords.Y, j);
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
            ScoreWindow PlayerScore = new ScoreWindow();
            PlayerScore.ShowDialog();
            await Task.Delay(500);
            Stopwatch.Stop();
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
                        HP = double.Parse(sr.ReadLine().Split(':').Last(), CultureInfo.InvariantCulture);
                        CircSize = double.Parse(sr.ReadLine().Split(':').Last(), CultureInfo.InvariantCulture);
                        OD = double.Parse(sr.ReadLine().Split(':').Last(), CultureInfo.InvariantCulture);
                        AR = double.Parse(sr.ReadLine().Split(':').Last(), CultureInfo.InvariantCulture);
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
                HitWindow300 = -6 * (OD - 17.75); // base val 13.75 , adjusted for easier difficulty due to wpf fps
                HitWindow100 = -8 * (OD - 20.4375); // base val 17.4375
                HitWindow50 = -10 * (OD - 24.95); // base val 19.95

                CS = 109 - (9 * CircSize);
                //Abort = false;
                DiffMultiplier = Math.Round((double)((HP + CircSize + OD + (double)Math.Clamp(HitObjects.Count / (double)(HitObjects.Last().Time / 1000) * 8,0,16)) / 38d) * 5);
                //MessageBox.Show(DiffMultiplier.ToString());
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
    public class ClickableCircle
    {
        public int Score { get; set; }
        public double Xpos { get; }
        public double Ypos { get; }
        Ellipse MainCircle { get; set; }
        Ellipse ApproachCircle { get; set; }
        bool isAlive { get; set; }
        Stopwatch sw { get; }

        static double Preempt { get { return Engine.Preempt; } }
        static double FadeIn { get { return Engine.FadeIn; } }
        static double FadeOutTime { get { return Engine.FadeOutTime; } }
        static double HitWindow50 { get { return Engine.HitWindow50; } }
        static double HitWindow100 { get { return Engine.HitWindow100; } }
        static double HitWindow300 { get { return Engine.HitWindow300; } }
        public ClickableCircle(int x, int y)
        {
            Ellipse MainCircle = new Ellipse()
            {
                Height = Engine.CS,
                Width = Engine.CS,
                //Stroke = new SolidColorBrush(Colors.White),
                //StrokeThickness = 4,
                Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/hitcircle.png"))),
                Opacity = 0,
                Tag = this,
            };
            MainCircle.MouseDown += Circle_ClickCheck;
            Engine.MainWindow.PlayArea.Children.Add(MainCircle);
            Canvas.SetTop(MainCircle, y);
            Canvas.SetLeft(MainCircle, x);

            Ellipse circle = new Ellipse()
            {
                Height = Engine.CS * 4,
                Width = Engine.CS * 4,
                Stroke = new SolidColorBrush(Colors.Azure),
                StrokeThickness = 4,
                Opacity = 0

            };
            this.Xpos = x;
            this.Ypos = y;
            this.MainCircle = MainCircle;
            this.ApproachCircle = circle;
            Engine.MainWindow.PlayArea.Children.Add(circle);
            Canvas.SetTop(circle, y + Engine.CS / 2);
            Canvas.SetLeft(circle, x + Engine.CS / 2);
            Canvas.SetZIndex(MainCircle, Engine.HitObjects.Count + 10 - Engine.SpawnedObj);
            Canvas.SetZIndex(circle, Engine.HitObjects.Count + 10 - Engine.SpawnedObj);
            Stopwatch approach = new Stopwatch();
            sw = approach;
            isAlive = true;
        }

        public async void Spawn()
        {
            await CircleLife();
            await CircleAfterLife();
            await CheckResult();
        }
        async Task CircleLife()
        {
            sw.Start();
            while (sw.ElapsedMilliseconds < Preempt && isAlive)
            {
                if (sw.ElapsedMilliseconds < FadeIn)
                {
                    MainCircle.Opacity = (double)(sw.ElapsedMilliseconds / (double)(FadeIn));
                    ApproachCircle.Opacity = (double)(sw.ElapsedMilliseconds / (double)(FadeIn));
                }
                //MessageBox.Show(circle.Opacity.ToString());
                ApproachCircle.Height = 3 * Engine.CS * (1 - (double)(sw.ElapsedMilliseconds / (double)Preempt)) + Engine.CS;
                ApproachCircle.Width = 3 * Engine.CS * (1 - (double)(sw.ElapsedMilliseconds / (double)Preempt)) + Engine.CS;
                Canvas.SetTop(ApproachCircle, Ypos + (Engine.CS / 2) - ApproachCircle.Height / 2);
                Canvas.SetLeft(ApproachCircle, Xpos + (Engine.CS / 2) - ApproachCircle.Width / 2);

                await Task.Delay(1);
            }
            sw.Stop();
            Engine.MainWindow.PlayArea.Children.Remove(ApproachCircle);

        }
        async Task CircleAfterLife()
        {
            sw.Start();

            if (isAlive)
            {
                while (sw.ElapsedMilliseconds < Preempt + HitWindow50 && isAlive)
                {
                    await Task.Delay(1);
                }
            }
            if (isAlive)
            {
                Engine.player.Miss();
                Score = 0;
            }
            isAlive = false;
            Canvas.SetZIndex(MainCircle, 0);
            sw.Stop();
        }
        async Task CheckResult()
        {
            if (Score == 0)
            {
                DrawResult();
            }
            else
            {
                bool resultDrew = false;
                if (Score == 300)
                {
                    resultDrew = true;
                }
                sw.Reset();

                sw.Start();
                while (sw.ElapsedMilliseconds < FadeOutTime)
                {
                    if (sw.ElapsedMilliseconds >= FadeOutTime / 2 && !resultDrew)
                    {
                        resultDrew = true;
                        DrawResult();
                    }
                    MainCircle.Height = 0.4 * Engine.CS * ((double)(sw.ElapsedMilliseconds / (double)FadeOutTime)) + Engine.CS;
                    MainCircle.Width = 0.4 * Engine.CS * ((double)(sw.ElapsedMilliseconds / (double)FadeOutTime)) + Engine.CS;
                    Canvas.SetTop(MainCircle, Ypos + (Engine.CS / 2) - MainCircle.Height / 2);
                    Canvas.SetLeft(MainCircle, Xpos + (Engine.CS / 2) - MainCircle.Width / 2);
                    MainCircle.Opacity = ((double)(FadeOutTime - sw.ElapsedMilliseconds) / (double)FadeOutTime) - 0.1;
                    await Task.Delay(1);
                }
                sw.Stop();
            }
            Engine.MainWindow.PlayArea.Children.Remove(MainCircle);
            Engine.UpdatePlayerLabel(false);

        }
        async void DrawResult()
        {
            double AnimationLength = 500;
            Image result = new Image()
            {
                Source = Engine.GetImageAfterScore(Score),
                Width = Engine.CS / 3,
                Height = Engine.CS / 3,
                Opacity = 1
            };
            Engine.MainWindow.PlayArea.Children.Add(result);
            Canvas.SetLeft(result, Xpos + Engine.CS / 6 + result.Width / 2);
            Canvas.SetTop(result, Ypos + Engine.CS / 6 + result.Height / 2);
            Canvas.SetZIndex(result, 0);
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            while (sw2.ElapsedMilliseconds <= AnimationLength)
            {
                result.Opacity = (AnimationLength - sw2.ElapsedMilliseconds) / AnimationLength;
                await Task.Delay(1);
            }
            sw2.Stop();
            Engine.MainWindow.PlayArea.Children.Remove(result);
        }
        void Circle_ClickCheck(object sender, MouseButtonEventArgs e)
        {
            Click();
        }
        public void Click()
        {
            if (isAlive)
            {
                if (sw.ElapsedMilliseconds + Engine.HitWindow300 >= Engine.Preempt && sw.ElapsedMilliseconds <= Engine.Preempt + Engine.HitWindow300)
                {
                    isAlive = false;
                    Score = 300;
                    Engine.player.AddScore(300);
                    return;
                }
                else if (sw.ElapsedMilliseconds + Engine.HitWindow100 >= Engine.Preempt && sw.ElapsedMilliseconds <= Engine.Preempt + Engine.HitWindow100)
                {
                    isAlive = false;
                    Score = 100;
                    Engine.player.AddScore(100);
                    return;
                }
                else if (sw.ElapsedMilliseconds + Engine.HitWindow50 >= Engine.Preempt && sw.ElapsedMilliseconds <= Engine.Preempt + Engine.HitWindow50)
                {
                    isAlive = false;
                    Score = 50;
                    Engine.player.AddScore(50);
                    return;
                }
                else
                {
                    Score = 0;
                    isAlive = false;
                    Engine.player.Miss();
                    //ShakeAnimation() - todo
                }
            }
        }
    }
    public class PlayerStats
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
        public int MaxCombo { get; set; }
        public PlayerStats(int TotalObj)
        {
            MaxCombo = 0;
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
            if (Combo > MaxCombo)
            {
                MaxCombo = Combo;
            }
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
            Score += (int)Math.Ceiling(pts * ((double)1 + ((double)(Combo * Engine.DiffMultiplier) / 25)));
            Combo++;
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
    public class HitObjectEvent
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
