using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Windows;

namespace CircleClickingGame
{
    public class PlayerStats : INotifyPropertyChanged
    {
        Visibility _visibility;
        public Visibility StatsVisibility { get { return _visibility; } private set { _visibility = value; OnPropertyChanged(); } }

        double _hp;
        int _score;
        int _combo;
        string _acc;
        public string ComboString { get { return 'x' + Combo.ToString(); } set { OnPropertyChanged(); } }
        public double HP { get { return _hp; } private set { _hp = value; OnPropertyChanged(); } }
        public int Score { get { return _score; } private set { _score = value; OnPropertyChanged(); } }
        public int Combo { get { return _combo; } private set { _combo = value; ComboString = ""; } }
        public int ObjectsHit300 { get; private set; }
        public int ObjectsHit100 { get; private set; }
        public int ObjectsHit50 { get; private set; }
        public int ObjectsMiss { get; private set; }
        public string Accuracy { get { return _acc; } private set { _acc = value; OnPropertyChanged(); } }
        public int TotalObj { get; private set; }
        public int MaxCombo { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public PlayerStats(int TotalObj)
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
        }
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void CalcStats()
        {
            AccCalc();
            if (Combo > MaxCombo)
            {
                MaxCombo = Combo;
            }
        }
        void AccCalc()
        {
            double ObjScore = ObjectsMiss + ObjectsHit50 + ObjectsHit100 + ObjectsHit300;
            if(ObjScore == 0)
            {
                Accuracy = "100%";
                return;
            }
            double acc = ((double)ObjectsHit300 + 0.66 * (double)ObjectsHit100 + 0.33 * (double)ObjectsHit50) / ObjScore;
            Accuracy = Math.Round(acc * 100 , 2).ToString() + '%';
        }
        public void Show()
        {
            StatsVisibility = Visibility.Visible;
        }
        public void Hide()
        {
            StatsVisibility = Visibility.Collapsed;
        }
        public void ComboBreak()
        {
            Combo = 0;
            TotalObj++;
        }
        public void ReInit(int count)
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
            AccCalc();
            this.TotalObj = count;
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
                default:
                    TotalObj++;
                    break;
            }
            CalcStats();
        }
    }
}
