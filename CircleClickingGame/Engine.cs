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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CircleClickingGame
{
    class Engine
    {
        public static MainWindow MainWindow;       
        public static Random rng;
        
        public static DispatcherTimer Timer; // wip
        //public static List<ClickableCircle> Circles;        
        public static List<HitObjectEvent> HitObjects;
        public static List<TimingPoint> TimingPoints;
        public static List<BreakEvent> BreakEvents;
        
        
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
        public static double BPM;
        public static double FadeOutTime = 200;
        public static double SliderMultiplier;
        public static double SliderVelocity = 1;
        public static double SliderTickrate;
        public static double DiffMultiplier;
        public static Key key1 = Key.Z;
        public static Key key2 = Key.X;

        public static string Score { get { return player.Score.ToString(); } }

        public static bool isPaused;
        public static bool Abort;
        public static bool Button1IsHeld = false;
        public static bool Button2IsHeld = false;
        public static bool MButton1IsHeld = false;
        public static bool MButton2IsHeld = false;
        public static bool AnyButtonHeld { get { return Button1IsHeld || Button2IsHeld || MButton1IsHeld || MButton2IsHeld; } }
        static PlayerStats _player = PlayerStats.Player;
        public static PlayerStats player { get { return _player; } }
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

            Timer = new DispatcherTimer();
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: 100);

            HitObjects = new List<HitObjectEvent>();
            TimingPoints = new List<TimingPoint>();
            BreakEvents = new List<BreakEvent>();

            Stopwatch = new Stopwatch();
            
            MainWindow.PlayArea.Background = new SolidColorBrush(Colors.Black);
            MainWindow.PauseButton.Visibility = Visibility.Collapsed;
            MediaPlayer = new MediaPlayer();
            player.ReInit(HitObjects.Count);
            MapPath = string.Empty;
            MapName = "No Beatmap Loaded";
            MapAudio = string.Empty;
            StatsUpdate(false);
            //player.Hide();

        }
        public static void SoftReset()
        {
            Abort = true;
            isPaused = false;
            rng = new Random();

            HitObjects = new List<HitObjectEvent>();
            TimingPoints = new List<TimingPoint>();
            BreakEvents = new List<BreakEvent>();

            Stopwatch = new Stopwatch();

            Timer = new DispatcherTimer();
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: 100);

            MainWindow.PlayArea.Background = new SolidColorBrush(Colors.Black);
            MainWindow.PauseButton.Visibility = Visibility.Collapsed;
            MediaPlayer = new MediaPlayer();
            player.ReInit(HitObjects.Count);
            LoadMap();
        }
        public static void StatsUpdate(bool OK)
        {
            if (OK)
            {
                MainWindow.StatsLabel.Content = $"CS : {CircSize} " +
                    $"{Environment.NewLine}AR : {AR} " +
                    $"{Environment.NewLine}OD : {OD} " +
                    $"{Environment.NewLine}HP: {HP} " +
                    $"{Environment.NewLine}Circles: {HitObjects.Count} ";

            }
            else
            {
                MainWindow.StatsLabel.Content = "No map info available";
            }
        }

        private static void Timer_Tick(object? sender, EventArgs e)
        {
            player.Drain();
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
        public static async void Run()
        {
            int j = 0;
            int k = 0;
            SpawnedObj = 0;
            Stopwatch.Start();
            Timer.Start();
            MediaPlayer.Position = new TimeSpan(0, 0, 0, 0, 0);
            while(j < HitObjects.Count)
            {
                player.Progress();
                SpawnedObj = j;
                if (Abort)
                {
                    await AbortBeatmap();
                    return;
                }
                if (k < TimingPoints.Count && Stopwatch.ElapsedMilliseconds >= TimingPoints[k].Time - Preempt)
                {
                    TimingPoints[k].Set();
                    k++;
                }
                if (Stopwatch.ElapsedMilliseconds >= HitObjects[j].Time - Preempt)
                {
                    HitObjects[j].Spawn();
                    j++;
                }               

                else await Task.Delay(1);
            }
            if (Abort)
            {
                MediaPlayer.Stop();
                await AbortBeatmap();
                return;
            }
            

            await Task.Delay(5000);
            ScoreWindow PlayerScore = new ScoreWindow();
            PlayerScore.ShowDialog();
            await Task.Delay(500);
            Stopwatch.Stop();
            Timer.Stop();
            Finished();
        }
        static async Task AbortBeatmap()
        {
            MediaPlayer.Stop();
            //MessageBox.Show("Aborted past beatmap.");
            MainWindow.PauseButton.Content = "Aborting...";
            await Task.Delay(3000);
            Finished();
            MainWindow.PauseButton.Content = "Abort";
            MainWindow.PauseButton.IsEnabled = true;
        }
        static void Finished()
        {
            Engine.SoftReset();
            MainWindow.StartButton.Visibility = Visibility.Visible;
            MainWindow.BeatmapButton.Visibility = Visibility.Visible;
            MainWindow.SettingsButton.Visibility = Visibility.Visible;
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
                bool beginHitObjRead = false;
                bool readTimingPoints = false;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if(line == null || line == string.Empty)
                    {
                        readTimingPoints = false;
                        beginHitObjRead = false;
                        continue;
                    }
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
                        SliderMultiplier = double.Parse(sr.ReadLine().Split(':').Last(), CultureInfo.InvariantCulture);
                        SliderTickrate = double.Parse(sr.ReadLine().Split(':').Last(), CultureInfo.InvariantCulture);
                    }
                    if(line == "[TimingPoints]")
                    {
                        readTimingPoints = true;
                        continue;
                    }
                    
                    if (!beginHitObjRead && line == "[HitObjects]")
                    {
                        beginHitObjRead = true;
                        readTimingPoints = false;
                        continue;
                    }
                    if (readTimingPoints)
                    {
                        string[] properties = line.Split(',');
                        int time = (int)(double.Parse(properties[0]));
                        double beatlen = double.Parse(properties[1]);
                        int inherit = int.Parse(properties[6]);
                        TimingPoints.Add(new TimingPoint(time,inherit,beatlen));
                    }
                    if (beginHitObjRead)
                    {
                        string[] properties = line.Split(',');
                        int x = int.Parse(properties[0]);
                        int y = int.Parse(properties[1]);
                        int time = int.Parse(properties[2]);
                        int type = int.Parse(properties[3]);
                        string[] pars = new string[3];
                        if ((type & 2) > 0)
                        {
                            pars[0] = properties[5]; // curve pts
                            pars[1] = properties[6]; // slides 
                            pars[2] = properties[7]; // length
                        }
                        HitObjects.Add(new HitObjectEvent(x, y, time, type, pars));
                    }
                }
                BPM = TimingPoints[0].BeatLength;
                Preempt = 1250 - 750 * ((AR - 5) / 5);
                FadeIn = 800 - 500 * ((AR - 5) / 5);
                HitWindow300 = -6 * (OD - 17.75); // base val 13.75 , adjusted for easier difficulty due to wpf fps
                HitWindow100 = -8 * (OD - 20.4375); // base val 17.4375
                HitWindow50 = -10 * (OD - 24.95); // base val 19.95

                CS = 109 - (9 * CircSize);
                //Abort = false;
                DiffMultiplier = Math.Round((double)((HP + CircSize + OD + (double)Math.Clamp(HitObjects.Count / (double)(HitObjects.Last().Time / 1000) * 8,0,16)) / 38d) * 5);
                player.ReInit(HitObjects.Count);
                StatsUpdate(true);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load map.");
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
