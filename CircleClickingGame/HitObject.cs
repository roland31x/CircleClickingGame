using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CircleClickingGame
{
    public class HitObjectEvent
    {
        Point coords { get; }
        public int Time { get; }
        int Type { get; }
        string[] Props { get; }
        public HitObjectEvent(int x, int y, int time, int type, string[] pars)
        {
            coords = new Point(x, y);
            Time = time;
            Type = type;
            Props = pars;
        }
        public void Spawn()
        {
            if ((Type & 1) > 0)
            {
                new ClickableCircle((int)coords.X, (int)coords.Y).Spawn();
            }
            else if ((Type & 2) > 0) 
            {
                new ClickableSlider((int)coords.X, (int)coords.Y, Props).Spawn();
            }
        }
    }
    public class TimingPoint
    {
        public int Time { get; }
        public int Inherited { get; }
        public double BeatLength { get; }

        public TimingPoint(int time, int inherited, double beatLength)
        {
            Time = time;
            Inherited = inherited;
            BeatLength = beatLength;
        }
        public void Set()
        {
            if(BeatLength >= 0)
            {
                Engine.BPM = BeatLength;
            }
            else
            {
                Engine.SliderVelocity = Math.Abs(100 / BeatLength);
            }
        }
    }
    
    public abstract class Clickable
    {
        public int Score { get; protected set; }
        public double Xpos { get; protected set; }
        public double Ypos { get; protected set; }
        public Point StartPoint { get { return new Point(Xpos, Ypos); } }
        protected Ellipse MainCircle { get; set; }
        protected Ellipse ApproachCircle { get; set; }
        protected Stopwatch sw { get; set; }
        protected bool isAlive { get; set; }
        protected static double Preempt { get { return Engine.Preempt; } }
        protected static double FadeIn { get { return Engine.FadeIn; } }
        protected static double FadeOutTime { get { return Engine.FadeOutTime; } }
        protected static double HitWindow50 { get { return Engine.HitWindow50; } }
        protected static double HitWindow100 { get { return Engine.HitWindow100; } }
        protected static double HitWindow300 { get { return Engine.HitWindow300; } }
        public abstract void Click();
    }
}
