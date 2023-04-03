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
using System.Drawing;

namespace CircleClickingGame
{
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
    }
    public class ClickableSlider
    {
        public int Score { get; set; }
        public double Xpos { get; }
        public double Ypos { get; }
        Ellipse MainCircle { get; set; }
        Ellipse ApproachCircle { get; set; }
        Ellipse SliderBall { get; set; }
        Ellipse SliderBallHitbox { get; set; }
        Path Body { get; set; }
        double Length { get; }
        bool isAlive { get; set; }
        Stopwatch sw { get; }

        static double Preempt { get { return Engine.Preempt; } }
        static double FadeIn { get { return Engine.FadeIn; } }
        static double FadeOutTime { get { return Engine.FadeOutTime; } }
        static double HitWindow50 { get { return Engine.HitWindow50; } }
        static double Velocity { get { return Engine.SliderVelocity; } }
        public ClickableSlider(int x, int y, string[] props)
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

            //MainCircle.MouseDown += Circle_ClickCheck;
            Engine.MainWindow.PlayArea.Children.Add(MainCircle);
            Canvas.SetTop(MainCircle, y);
            Canvas.SetLeft(MainCircle, x);

            Ellipse SliderB = new Ellipse()
            {
                Height = Engine.CS - Engine.CS * 0.1,
                Width = Engine.CS - Engine.CS * 0.1,
                //Stroke = new SolidColorBrush(Colors.White),
                //StrokeThickness = 4,
                Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/hitcircle.png"))),
                Opacity = 0,
                Tag = this,
            };



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

            Path sp = new Path()
            {

            };
            Canvas.SetTop(circle, y + Engine.CS / 2);
            Canvas.SetLeft(circle, x + Engine.CS / 2);
            Canvas.SetZIndex(MainCircle, Engine.HitObjects.Count + 10 - Engine.SpawnedObj);
            Canvas.SetZIndex(circle, Engine.HitObjects.Count + 10 - Engine.SpawnedObj);
            Stopwatch approach = new Stopwatch();
            sw = approach;
            isAlive = true;
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
}
