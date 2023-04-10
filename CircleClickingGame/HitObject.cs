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
using System.Threading;

namespace CircleClickingGame
{
    public interface IGameEvent
    {
        int Time { get; }

        public void StartEvent();
    }
    public class HitObjectEvent : IGameEvent
    {
        public static double ScaleMultiplier { get { return Engine.ScaleMultiplier; } }
        public int Time { get; }
        int Type { get; }
        double Xpos { get; }
        double Ypos { get; }
        string[] Props { get; } 
        public HitObjectEvent(int x, int y, int time, int type, string[] pars)
        {
            Time = time;
            Type = type;
            Props = pars;
            Xpos = x * Engine.ScaleMultiplier;
            Ypos = y * Engine.ScaleMultiplier;
        }
        public Clickable Create(double x, double y, string[] Props)
        {
            if ((Type & 1) > 0)
            {
                return new ClickableCircle(x, y);
            }
            else if ((Type & 2) > 0)
            {
                return new ClickableSlider(x, y, Props);
            }
            else return new ClickableCircle(x, y);
        }
        public void StartEvent()
        {
            Create(Xpos,Ypos,Props).Spawn();
        }
    }
    public class BreakEvent : IGameEvent
    {
        public int Time { get; set; }
        int EndTime { get; set; }
        DispatcherTimer Timer { get; set; }
        public BreakEvent(int start, int end)
        {
            Time = start;
            EndTime = end;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(EndTime - Time);
            Timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Engine.Timer.Start();
            Timer.Stop();
        }

        public void StartEvent()
        {
            Engine.Timer.Stop();
            Timer.Start();
        }
    }
    public class TimingPoint : IGameEvent
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
        public void StartEvent()
        {
            if(BeatLength >= 0)
            {
                Engine.BPM = BeatLength;
                Engine.SliderVelocity = 1;
            }
            else
            {
                Engine.SliderVelocity = Math.Abs(100 / BeatLength);
            }
        }
    }
    
    public abstract class Clickable
    {
        protected static double Preempt { get { return Engine.Preempt; } }
        protected static double FadeIn { get { return Engine.FadeIn; } }
        protected static double FadeOutTime { get { return Engine.FadeOutTime; } }
        protected static double HitWindow50 { get { return Engine.HitWindow50; } }
        protected static double HitWindow100 { get { return Engine.HitWindow100; } }
        protected static double HitWindow300 { get { return Engine.HitWindow300; } }

        public int Score { get; protected set; }
        public double Xpos { get; protected set; }
        public double Ypos { get; protected set; }
        public Point StartPoint { get { return new Point(Xpos, Ypos); } }
        protected Ellipse MainCircle { get; set; }
        protected Ellipse ApproachCircle { get; set; }
        protected Stopwatch sw { get; set; }
        protected bool isAlive { get; set; }

        public abstract void Click();
        public abstract void Spawn();
    }
}
