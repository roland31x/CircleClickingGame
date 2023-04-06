﻿using System;
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
    public class ClickableSlider : Clickable
    {
        Ellipse SliderBall { get; set; }
        Ellipse SliderBallHitbox { get; set; }
        Ellipse EndCircle { get; set; }
        Path Body { get; set; }
        public double endXpos { get; private set; }
        public double endYpos { get; private set; }
        Point endpoint { get { return new Point(endXpos, endYpos); } }
        PathGeometry BodyPG { get; set; }
        PathGeometry ReversedBodyPG { get; set; }
        double Length { get; }
        double Repeat { get; }
        bool isInAnimation { get; set; }
        bool circleWasHit { get; set; }
        bool wasFollowed { get; set; }
        bool endWasHit { get; set; }
        static double Multiplier { get { return Engine.SliderMultiplier; } }
        static double SV { get { return Engine.SliderVelocity; } }
        public ClickableSlider(int x, int y, string[] props)
        {
            Xpos = x;
            Ypos = y;
            Length = double.Parse(props[2]);
            Repeat = double.Parse(props[1]);
            BuildBody(x, y, props);

            MainCircle = new Ellipse()
            {
                Height = Engine.CS,
                Width = Engine.CS,
                //Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 4,
                Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/hitcircle.png"))),
                Opacity = 0,
                Tag = this,
            };
            Canvas.SetZIndex(MainCircle, Engine.HitObjects.Count + 10 - Engine.SpawnedObj);
            Engine.MainWindow.PlayArea.Children.Add(MainCircle);
            MainCircle.MouseDown += Circle_ClickCheck;
            Canvas.SetTop(MainCircle, y - MainCircle.Height / 2);
            Canvas.SetLeft(MainCircle, x - MainCircle.Width / 2);

            EndCircle = new Ellipse()
            {
                Height = Engine.CS,
                Width = Engine.CS,
                Stroke = new SolidColorBrush(Colors.Aquamarine),
                StrokeThickness = 4,
                Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/hitcircle.png"))),
                Opacity = 0,
            };
            if(Repeat > 1)
            {
                EndCircle.Stroke = new SolidColorBrush(Colors.Red);
                Body.Stroke = new SolidColorBrush(Colors.Red);
            }
            //Engine.MainWindow.PlayArea.Children.Add(EndCircle);
            Canvas.SetTop(EndCircle, endYpos - EndCircle.Height / 2);
            Canvas.SetLeft(EndCircle, endXpos - EndCircle.Width / 2);


            SliderBall = new Ellipse()
            {
                Height = Engine.CS - Engine.CS * 0.1,
                Width = Engine.CS - Engine.CS * 0.1,
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Colors.Orange), //new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/hitcircle.png"))),
                Opacity = 1,
            };
            Canvas.SetLeft(SliderBall, -SliderBall.Width / 2);
            Canvas.SetTop(SliderBall, -SliderBall.Height / 2);
            Canvas.SetZIndex(SliderBall, Canvas.GetZIndex(MainCircle) - 1);

            SliderBallHitbox = new Ellipse()
            {
                Height = Engine.CS * 2,
                Width = Engine.CS * 2,
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 5,
                Fill = new SolidColorBrush(Colors.Transparent),
                Opacity = 0.2,
                Tag = this,
            };
            Canvas.SetLeft(SliderBallHitbox, -SliderBallHitbox.Width / 2);
            Canvas.SetTop(SliderBallHitbox, -SliderBallHitbox.Height / 2);
            Canvas.SetZIndex(SliderBallHitbox, Canvas.GetZIndex(MainCircle));
           
            ApproachCircle = new Ellipse()
            {
                Height = Engine.CS * 4,
                Width = Engine.CS * 4,
                Stroke = new SolidColorBrush(Colors.Azure),
                StrokeThickness = 4,
                Opacity = 0,
            };
            Canvas.SetTop(ApproachCircle, y - ApproachCircle.Height / 2);
            Canvas.SetLeft(ApproachCircle, x - ApproachCircle.Width / 2);
            Canvas.SetZIndex(ApproachCircle, Engine.HitObjects.Count + 10 - Engine.SpawnedObj);
            Engine.MainWindow.PlayArea.Children.Add(ApproachCircle);

            Canvas.SetZIndex(Body, 0);
            sw = new Stopwatch();
            isAlive = true;
        }
        void BuildBody(int x, int y, string[] pars)
        {

            PathFigure pathFigure = new PathFigure();

            pathFigure.StartPoint = new Point(x, y);

            PathSegment slidersegment;

            string[] curvepts = pars[0].Split('|');
            endXpos = double.Parse(curvepts.Last().Split(':')[0].Trim());
            endYpos = double.Parse(curvepts.Last().Split(':')[1].Trim());
            if (curvepts[0] == "B")
            {
                slidersegment = new PolyBezierSegment();
                for (int i = 1; i < curvepts.Length; i++)
                {
                    double xc = double.Parse(curvepts[i].Split(':')[0].Trim());
                    double yc = double.Parse(curvepts[i].Split(':')[1].Trim());
                    (slidersegment as PolyBezierSegment).Points.Add(new Point(xc, yc));
                }
                pathFigure.Segments.Add(slidersegment);
            }
            else if (curvepts[0] == "L" || curvepts[0] == "P")
            {
                slidersegment = new PolyLineSegment();
                for (int i = 1; i < curvepts.Length; i++)
                {
                    double xc = double.Parse(curvepts[i].Split(':')[0].Trim());
                    double yc = double.Parse(curvepts[i].Split(':')[1].Trim());
                    (slidersegment as PolyLineSegment).Points.Add(new Point(xc, yc));
                }
                pathFigure.Segments.Add(slidersegment);
            }
            else
            {
                //slidersegment = new ArcSegment();
                //double xc = double.Parse(curvepts[1].Split(':')[0].Trim());
                //double yc = double.Parse(curvepts[1].Split(':')[1].Trim());
                //(slidersegment as ArcSegment).Point = new Point(xc, yc);
                ////(slidersegment as ArcSegment).IsLargeArc = true;
                //xc = double.Parse(curvepts[2].Split(':')[0].Trim());
                //yc = double.Parse(curvepts[2].Split(':')[1].Trim());
                //(slidersegment as ArcSegment).Size = new Size(xc, yc);

                //pathFigure.Segments.Add(slidersegment);
                throw new NotImplementedException();

            }

            if(Repeat > 1)
            {              
                PathFigure reversed = new PathFigure();
                reversed.StartPoint = endpoint;
                if (curvepts[0] == "B")
                {
                    slidersegment = new PolyBezierSegment();
                    for (int i = curvepts.Length - 2; i >= 1; i--)
                    {
                        double xc = double.Parse(curvepts[i].Split(':')[0].Trim());
                        double yc = double.Parse(curvepts[i].Split(':')[1].Trim());
                        (slidersegment as PolyBezierSegment).Points.Add(new Point(xc, yc));
                        //(slidersegment as PolyBezierSegment).Points.Add(StartPoint);
                    }
                    reversed.Segments.Add(new LineSegment() { Point = StartPoint } );
                    reversed.Segments.Add(slidersegment);
                }
                else if (curvepts[0] == "L" || curvepts[0] == "P")
                {
                    for (int i = curvepts.Length - 2; i >= 1; i--)
                    {
                        slidersegment = new LineSegment();
                        double xc = double.Parse(curvepts[i].Split(':')[0].Trim());
                        double yc = double.Parse(curvepts[i].Split(':')[1].Trim());
                        (slidersegment as LineSegment).Point = new Point(xc, yc);
                        reversed.Segments.Add(slidersegment);
                    }                  
                    reversed.Segments.Add(new LineSegment() { Point = StartPoint });
                }
                else
                {
                    //slidersegment = new ArcSegment();
                    //double xc = double.Parse(curvepts[1].Split(':')[0].Trim());
                    //double yc = double.Parse(curvepts[1].Split(':')[1].Trim());
                    //(slidersegment as ArcSegment).Point = new Point(xc, yc);
                    ////(slidersegment as ArcSegment).IsLargeArc = true;
                    //xc = double.Parse(curvepts[2].Split(':')[0].Trim());
                    //yc = double.Parse(curvepts[2].Split(':')[1].Trim());
                    //(slidersegment as ArcSegment).Size = new Size(xc, yc);

                    //pathFigure.Segments.Add(slidersegment);
                    throw new NotImplementedException();

                }
                ReversedBodyPG = new PathGeometry()
                {
                    Figures = new PathFigureCollection { reversed },
                };
                ReversedBodyPG.Freeze();
                //Path path1 = new Path()
                //{
                //    Data = ReversedBodyPG,
                //    StrokeThickness = Engine.CS,
                //    Stroke = new SolidColorBrush(Colors.Yellow),
                //    Opacity = 1,
                //};

                //Engine.MainWindow.PlayArea.Children.Add(path1);
            }
                       
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection
            {
                pathFigure
            };
            BodyPG = pathGeometry;
            BodyPG.Freeze();

            Body = new Path()
            {
                Data = pathGeometry,
                StrokeThickness = Engine.CS,
                Stroke = new SolidColorBrush(Colors.Aquamarine),
                Opacity = 0.3,
            };
            
            Engine.MainWindow.PlayArea.Children.Add(Body);
        }
        public async void Spawn()
        {
            await CircleLife();
            CircleAfterLife();
            if(sw.ElapsedMilliseconds < Preempt)
            {
                sw.Start();
                while (sw.ElapsedMilliseconds < Preempt)
                {
                    await Task.Delay(1);
                }
                sw.Stop();
            }         
            wasFollowed = true;
            endWasHit = true;
            Engine.MainWindow.PlayArea.Children.Add(SliderBall);
            Engine.MainWindow.PlayArea.Children.Add(SliderBallHitbox);
            //
            Engine.MainWindow.PlayArea.Children.Remove(MainCircle);
            //
            if (Repeat < 2)
            {
                await SliderAnimation(BodyPG);
            }
            else
            {
                for (int i = 1; i <= Repeat; i++)
                {
                    if(i == Repeat)
                    {
                        Body.Stroke = new SolidColorBrush(Colors.Aquamarine);
                        //if (i % 2 == 1)
                        //{
                            
                        //    //MainCircle.Stroke = null;
                        //    //EndCircle.Stroke = new SolidColorBrush(Colors.Aquamarine);
                        //}
                        //else
                        //{
                        //    //MainCircle.Stroke = new SolidColorBrush(Colors.Aquamarine);
                        //    //EndCircle.Stroke = null;
                        //}
                    }
                    //else if (i < Repeat - 1)
                    //{
                    //    MainCircle.Stroke = new SolidColorBrush(Colors.Red);
                    //}

                    if (i % 2 == 1)
                    {
                        await SliderAnimation(BodyPG);
                    }
                    else
                    {
                        await SliderAnimation(ReversedBodyPG);
                    }
                }
            }
            AnimationDone();
            ResultCheck();
            
        }
        async Task SliderAnimation(PathGeometry SliderPath)
        {
            double dur = Length / (Multiplier * 100 * SV) * Engine.BPM;
            Duration duration = new Duration(TimeSpan.FromMilliseconds((int)(dur / Repeat) + 10));
           
            TranslateTransform animatedTranslateTransform = new TranslateTransform();
            SliderBall.RenderTransform = animatedTranslateTransform;
            DoubleAnimationUsingPath translateXAnimation = new DoubleAnimationUsingPath();
            translateXAnimation.PathGeometry = SliderPath;
            translateXAnimation.Duration = duration;
            translateXAnimation.Source = PathAnimationSource.X;
            Storyboard.SetTarget(translateXAnimation, SliderBall);
            Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

            DoubleAnimationUsingPath translateYAnimation = new DoubleAnimationUsingPath();
            translateYAnimation.PathGeometry = SliderPath;
            translateYAnimation.Duration = duration;
            translateYAnimation.Source = PathAnimationSource.Y;
            Storyboard.SetTarget(translateYAnimation, SliderBall);
            Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            // Create a Storyboard to contain and apply the animations.
                   

            TranslateTransform animatedTranslateTransform1 = new TranslateTransform();
            SliderBallHitbox.RenderTransform = animatedTranslateTransform1;
            DoubleAnimationUsingPath translateXAnimation1 = new DoubleAnimationUsingPath();
            translateXAnimation1.PathGeometry = SliderPath;
            translateXAnimation1.Duration = duration;
            translateXAnimation1.Source = PathAnimationSource.X;
            Storyboard.SetTarget(translateXAnimation1, SliderBallHitbox);
            Storyboard.SetTargetProperty(translateXAnimation1, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

            DoubleAnimationUsingPath translateYAnimation1 = new DoubleAnimationUsingPath();
            translateYAnimation1.PathGeometry = SliderPath;
            translateYAnimation1.Duration = duration;
            translateYAnimation1.Source = PathAnimationSource.Y;
            Storyboard.SetTarget(translateYAnimation1, SliderBallHitbox);
            Storyboard.SetTargetProperty(translateYAnimation1, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            Storyboard pathAnimationStoryboard = new Storyboard();
            pathAnimationStoryboard.Completed += PathAnimationStoryboard_Completed;

            pathAnimationStoryboard.Children.Add(translateXAnimation);
            pathAnimationStoryboard.Children.Add(translateYAnimation);
            pathAnimationStoryboard.Children.Add(translateXAnimation1);
            pathAnimationStoryboard.Children.Add(translateYAnimation1);
            
            pathAnimationStoryboard.Begin();
            isInAnimation = true;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Engine.BPM / Engine.SliderTickrate);
            dispatcherTimer.Start();
            while (isInAnimation) // slider tickrate
            {         
                await Task.Delay(1);
            }
            dispatcherTimer.Stop();

        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if (Engine.ButtonIsHeld && SliderBallHitbox.IsMouseDirectlyOver)
            {
                SliderBall.Fill = new SolidColorBrush(Colors.Orange);
                Engine.player.AddScore(10);
            }
            else
            {
                SliderBall.Fill = new SolidColorBrush(Colors.Red);
                wasFollowed = false;
                Engine.player.ComboBreak();
            }
            Engine.UpdatePlayerLabel(false);
        }

        private void PathAnimationStoryboard_Completed(object? sender, EventArgs e)
        {
            isInAnimation = false;
            if (!SliderBallHitbox.IsMouseDirectlyOver && !Engine.ButtonIsHeld)
            {
                SliderBall.Fill = new SolidColorBrush(Colors.Red);
                endWasHit = false;
            }
            else
            {
                Engine.player.AddScore(30);
            }
            Engine.UpdatePlayerLabel(false);
           
        }
        async void AnimationDone()
        {
            SliderBallHitbox.Opacity = 0;
            Canvas.SetZIndex(SliderBall, 1);
            //Canvas.SetZIndex(EndCircle, 1);
            Canvas.SetZIndex(MainCircle, 1);
            sw.Reset();
            sw.Start();
            while (sw.ElapsedMilliseconds < FadeOutTime)
            {
                MainCircle.Opacity = ((double)(FadeOutTime - sw.ElapsedMilliseconds) / (double)FadeOutTime) - 0.1;
                //EndCircle.Opacity = ((double)(FadeOutTime - sw.ElapsedMilliseconds) / (double)FadeOutTime) - 0.1;
                SliderBall.Opacity = ((double)(FadeOutTime - sw.ElapsedMilliseconds) / (double)FadeOutTime) - 0.1;
                Body.Opacity = ((double)(FadeOutTime - sw.ElapsedMilliseconds) / (double)FadeOutTime) - 0.1;
                await Task.Delay(1);
            }
            sw.Stop();
            Engine.MainWindow.PlayArea.Children.Remove(SliderBallHitbox);
            Engine.MainWindow.PlayArea.Children.Remove(MainCircle);
            Engine.MainWindow.PlayArea.Children.Remove(SliderBall);
            Engine.MainWindow.PlayArea.Children.Remove(Body);
        }
        async Task CircleLife()
        {
            sw.Start();
            while (sw.ElapsedMilliseconds < Preempt && isAlive)
            {
                if (sw.ElapsedMilliseconds < FadeIn)
                {
                    EndCircle.Opacity = (double)(sw.ElapsedMilliseconds / (double)(FadeIn));
                    MainCircle.Opacity = (double)(sw.ElapsedMilliseconds / (double)(FadeIn));
                    ApproachCircle.Opacity = (double)(sw.ElapsedMilliseconds / (double)(FadeIn));
                    Body.Opacity = (double)(sw.ElapsedMilliseconds / (double)(FadeIn)) * 0.3;
                }

                ApproachCircle.Height = 3 * Engine.CS * (1 - (double)(sw.ElapsedMilliseconds / (double)Preempt)) + Engine.CS;
                ApproachCircle.Width = 3 * Engine.CS * (1 - (double)(sw.ElapsedMilliseconds / (double)Preempt)) + Engine.CS;
                Canvas.SetTop(ApproachCircle, Ypos - ApproachCircle.Height / 2);
                Canvas.SetLeft(ApproachCircle, Xpos - ApproachCircle.Width / 2);

                await Task.Delay(1);
            }
            Engine.MainWindow.PlayArea.Children.Remove(ApproachCircle);

        }
        async Task CircleAfterLife()
        {
            if (isAlive)
            {
                while (sw.ElapsedMilliseconds < Preempt + HitWindow50 && isAlive)
                {
                    await Task.Delay(1);
                }
            }
            if (isAlive)
            {
                circleWasHit = false;
                Engine.player.Miss();
            }
            isAlive = false;
        }
        async void ResultCheck()
        {
            bool[] res = new bool[] { circleWasHit, wasFollowed, endWasHit };
            int score = 0;
            for(int i = 0; i < res.Length; i++)
            {
                if (res[i])
                {
                    score++;
                }
            }
            if(score == 0)
            {
                Score = 0;
                Engine.player.Miss();
            }
            if(score == 1)
            {
                Score = 50;
                Engine.player.AddScore(Score);
            }
            if(score == 2)
            {
                Score = 100;
                Engine.player.AddScore(Score);
            }
            if(score == 3)
            {
                Score = 300;
                Engine.player.AddScore(Score);
            }

            Engine.UpdatePlayerLabel(false);
            if (Score != 300)
            {
                DrawResult();
            }
        }
        void Circle_ClickCheck(object sender, MouseButtonEventArgs e)
        {
            Click();
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
            Canvas.SetLeft(result, endXpos - result.Width / 2);
            Canvas.SetTop(result, endYpos - result.Height / 2);
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
        public override void Click()
        {
            if (isAlive)
            {
                if (sw.ElapsedMilliseconds + Engine.HitWindow50 >= Engine.Preempt && sw.ElapsedMilliseconds <= Engine.Preempt + Engine.HitWindow50)
                {
                    isAlive = false;
                    circleWasHit = true;
                    return;
                }
                else
                {
                    circleWasHit = false;
                    isAlive = false;
                    Engine.player.ComboBreak();
                    
                }
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
    public class ClickableCircle : Clickable
    {
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
            Canvas.SetTop(MainCircle, y - MainCircle.Height / 2);
            Canvas.SetLeft(MainCircle, x - MainCircle.Width / 2);

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
            Canvas.SetTop(circle, y - circle.Height / 2);
            Canvas.SetLeft(circle, x - circle.Width / 2);
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
                Canvas.SetTop(ApproachCircle, Ypos - ApproachCircle.Height / 2);
                Canvas.SetLeft(ApproachCircle, Xpos - ApproachCircle.Width / 2);

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
                    Canvas.SetTop(MainCircle, Ypos - MainCircle.Height / 2);
                    Canvas.SetLeft(MainCircle, Xpos - MainCircle.Width / 2);
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
            Canvas.SetLeft(result, Xpos - result.Width / 2);
            Canvas.SetTop(result, Ypos - result.Height / 2);
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
        public override void Click()
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
                }
            }
        }
    }
}
