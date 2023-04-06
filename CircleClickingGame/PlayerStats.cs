using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleClickingGame
{
    public class PlayerStats
    {
        public double HP { get; set; }
        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int ObjectsHit300 { get; private set; }
        public int ObjectsHit100 { get; private set; }
        public int ObjectsHit50 { get; private set; }
        public int ObjectsMiss { get; private set; }
        public double Accuracy { get { return AccCalc(); } }
        public int TotalObj { get; private set; }
        public int MaxCombo { get; private set; }
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
            this.TotalObj = TotalObj;
        }
        public void CalcStats()
        {
            if (Combo > MaxCombo)
            {
                MaxCombo = Combo;
            }
        }
        double AccCalc()
        {
            double ObjScore = ObjectsMiss + ObjectsHit50 + ObjectsHit100 + ObjectsHit300;
            if(ObjScore == 0)
            {
                return 100;
            }
            return ((double)ObjectsHit300 + 0.66 * (double)ObjectsHit100 + 0.33 * (double)ObjectsHit50) / ObjScore;
        }
        public void ComboBreak()
        {
            Combo = 0;
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
