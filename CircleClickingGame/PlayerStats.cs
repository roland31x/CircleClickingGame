using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Windows;
using System.Windows.Media;

namespace CircleClickingGame
{
    class PlayerStats : INotifyPropertyChanged
    {
        static PlayerStats _singleton = new PlayerStats(0);
        public static PlayerStats Player { get { return _singleton; } }

        Visibility _visibility;
        public Visibility StatsVisibility { get { return _visibility; } private set { _visibility = value; OnPropertyChanged(); } }

        double _hp;
        public double HP { get { return _hp; } private set { _hp = value; HPWidth = 0; } }
        int _score;
        public int Score { get { return _score; } private set { _score = value; OnPropertyChanged(); } }
        int _combo;
        public int Combo { get { return _combo; } private set { _combo = value; OnPropertyChanged(); } }
        public int ObjectsHit300 { get; private set; }
        public int ObjectsHit100 { get; private set; }
        public int ObjectsHit50 { get; private set; }
        public int ObjectsMiss { get; private set; }
        double _acc;
        public double Acc { get { return _acc; } private set { _acc = value; Accuracy = string.Empty; } }
        public string Accuracy { get { return Math.Round(Acc, 2).ToString() + '%'; } private set { _ = value; OnPropertyChanged(); } }
        public int TotalObj { get; private set; }
        public int MaxCombo { get; private set; }
        MediaPlayer mp { get { return Engine.MediaPlayer; } }
        public double HPWidth { get { try { return HP / 100 * Engine.MainWindow.Width; } catch (Exception) { return 0; } } private set { _ = value; OnPropertyChanged(); } }
        public double TimeLineWidth { get { try { return (mp.Position.TotalMilliseconds / (Engine.HitObjects.Last().Time - Engine.Preempt)) * Engine.MainWindow.Width; } catch (Exception) { return 0; } } private set { _ = value; OnPropertyChanged(); } }
        public bool HasFailed { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        PlayerStats(int TotalObj)
        {
            StatsVisibility = Visibility.Collapsed;
            MaxCombo = 0;
            HP = 100;
            Score = 0;
            Combo = 0;
            ObjectsHit300 = 0;
            ObjectsHit100 = 0;
            ObjectsHit50 = 0;
            ObjectsMiss = 0;
            this.TotalObj = TotalObj;
            HasFailed = false;
        }
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void Drain()
        {
            HP -= Engine.HP / 150;
        }
        public void CalcStats()
        {
            AccCalc();
            if (Combo > MaxCombo)
            {
                MaxCombo = Combo;
            }
            if (HP <= 0)
            {
                HP = 0;
                HasFailed = true;
                Engine.MainWindow.HPBar.Visibility = Visibility.Collapsed;
            }
            if (HP >= 100)
            {
                HP = 100;
            }
        }
        void AccCalc()
        {
            double ObjScore = ObjectsMiss + ObjectsHit50 + ObjectsHit100 + ObjectsHit300;
            if(ObjScore == 0)
            {
                Acc = 100;
                return;
            }
            double acc = ((double)ObjectsHit300 + 0.66 * (double)ObjectsHit100 + 0.33 * (double)ObjectsHit50) / ObjScore;
            Acc = acc * 100;
        }
        public void Show()
        {
            StatsVisibility = Visibility.Visible;
        }
        public void Hide()
        {
            StatsVisibility = Visibility.Collapsed;
        }
        public void Progress()
        {
            TimeLineWidth = 0; // triggers an event that changes progressbar, value means nothing
        }
        public void ComboBreak()
        {
            Combo = 0;
            TotalObj++;
            HP -= Engine.HP * 0.5;
        }
        public void ReInit(int count)
        {
            Engine.MainWindow.HPBar.Visibility = Visibility.Visible;
            StatsVisibility = Visibility.Collapsed;
            MaxCombo = 0;
            HP = 100;
            Score = 0;
            Combo = 0;
            ObjectsHit300 = 0;
            ObjectsHit100 = 0;
            ObjectsHit50 = 0;
            ObjectsMiss = 0;
            AccCalc();
            this.TotalObj = count;
            HasFailed = false;
        }
        public void Miss()
        {
            Combo = 0;
            ObjectsMiss++;
            HP -= Engine.HP;
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
                    HP += 0.3 * (10 - Engine.HP) + 3 * Engine.HP / 10;
                    break;
                case 100:
                    ObjectsHit100++;
                    HP += 0.3 * (10 - Engine.HP) + 1 * Engine.HP / 10;
                    break;
                case 50:
                    ObjectsHit50++;
                    HP += 0.3 * (10 - Engine.HP);
                    break;
                case 30: // slider end
                    TotalObj++;
                    HP += 0.1 * (10 - Engine.HP);
                    break;
                case 10: // slider tick
                    TotalObj++;
                    HP += 0.05 * (10 - Engine.HP);
                    break;
                default:
                    TotalObj++;
                    break;
            }
            CalcStats();
        }
    }
}
