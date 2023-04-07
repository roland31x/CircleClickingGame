using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows;
using System.Security.Cryptography;
using System.Security.Policy;

namespace CircleClickingGame
{
    public class ClickableSlider : Clickable
    {
        Ellipse SliderBall { get; set; }
        Ellipse SliderBallHitbox { get; set; }
        Ellipse EndCircle { get; set; }
        Ellipse StartCircle { get; set; }
        Path Body { get; set; }
        Path BodyLine { get; set; }
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
        bool WasTicked { get; set; }
        LinearGradientBrush pinkgr { get { return (LinearGradientBrush)App.Current.FindResource("GradientPinkPurple"); } }
        static double Multiplier { get { return Engine.SliderMultiplier; } }
        static double SV { get { return Engine.SliderVelocity; } }
        public ClickableSlider(int x, int y, string[] props)
        {
            Xpos = x;
            Ypos = y;
            Length = double.Parse(props[2]);
            Repeat = double.Parse(props[1]);

            

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

            StartCircle = new Ellipse()
            {
                Height = Engine.CS,
                Width = Engine.CS,
                Stroke = Brushes.White,
                StrokeThickness = 4,
                Fill = pinkgr,
                Opacity = 0,
            };
            Canvas.SetZIndex(StartCircle, 0);
            Engine.MainWindow.PlayArea.Children.Add(StartCircle);
            Canvas.SetTop(StartCircle, y - MainCircle.Height / 2);
            Canvas.SetLeft(StartCircle, x - MainCircle.Width / 2);

            EndCircle = new Ellipse()
            {
                Height = Engine.CS,
                Width = Engine.CS,
                Stroke = Brushes.White,
                StrokeThickness = 4,
                Fill = pinkgr,
                Opacity = 0,
            };


            
            BuildBody(x, y, props);

            Canvas.SetTop(EndCircle, endYpos - EndCircle.Height / 2);
            Canvas.SetLeft(EndCircle, endXpos - EndCircle.Width / 2);
            Canvas.SetZIndex(EndCircle, 0);
            if (Repeat > 1)
            {
                EndCircle.StrokeThickness = 7;
                EndCircle.Stroke = new SolidColorBrush(Colors.Red);               
            }

            Engine.MainWindow.PlayArea.Children.Add(EndCircle);   

            SliderBall = new Ellipse()
            {
                Height = Engine.CS - Engine.CS * 0.2,
                Width = Engine.CS - Engine.CS * 0.2,
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
                Height = Engine.CS * 2.5,
                Width = Engine.CS * 2.5,
                Stroke = Brushes.Azure,
                StrokeThickness = 5,
                Fill = new SolidColorBrush(Colors.Transparent),
                Opacity = 1,
                Tag = this,
            };
            SliderBallHitbox.MouseDown += Circle_ClickCheck;
            Canvas.SetLeft(SliderBallHitbox, -SliderBallHitbox.Width / 2);
            Canvas.SetTop(SliderBallHitbox, -SliderBallHitbox.Height / 2);
            Canvas.SetZIndex(SliderBallHitbox, Canvas.GetZIndex(MainCircle) + 1);

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
            Canvas.SetZIndex(ApproachCircle, Canvas.GetZIndex(MainCircle));
            Engine.MainWindow.PlayArea.Children.Add(ApproachCircle);

            sw = new Stopwatch();
            isAlive = true;
        }
        void BuildBody(int x, int y, string[] pars)
        {

            PathFigure pathFigure = new PathFigure();

            pathFigure.StartPoint = new Point(x, y);

            PathSegment slidersegment;

            string[] curvepts = pars[0].Split('|');
            char type = char.Parse(curvepts[0]);
            if (curvepts.Length == 1 + 1)
            {
                type = 'L';
            }
            if (type == 'B')
            {
                int c = 1;
                bool done = false;
                bool ellipsedrew = false;
                Point? Last = null;
                List<Point> pts;
                while (c < curvepts.Length && !done)
                {
                    pts = new List<Point>();
                    if (c == 1)
                    {
                        pts.Add(StartPoint);
                        Last = StartPoint;
                    }
                    for (int i = c; i < curvepts.Length; i++)
                    {
                        double xc = double.Parse(curvepts[i].Split(':')[0].Trim());
                        double yc = double.Parse(curvepts[i].Split(':')[1].Trim());
                        Point toAdd = new Point(xc, yc);

                        if (Last != null && toAdd == Last)
                        {
                            //pts.Add(toAdd);
                            c = i;
                            Last = null;
                            break;
                        }
                        if (i == curvepts.Length - 1)
                        {
                            done = true;
                        }
                        pts.Add(toAdd);
                        Last = toAdd;

                    }
                    Point[] Curve = new Point[pts.Count];
                    for (int i = 0; i < pts.Count; i++)
                    {
                        Curve[i] = pts[i];
                    }                   
                    for (double t = 0.01; t <= 1; t += 0.01)
                    {
                        Point PX = CurveMaths.beziercoord(Curve, t);
                        if (done && !ellipsedrew)
                        {
                            Point pos = CurveMaths.beziercoord(Curve, 1);
                            endXpos = pos.X;
                            endYpos = pos.Y;
                            ellipsedrew = true;
                        }
                        LineSegment ls = new LineSegment()
                        {
                            Point = PX,
                        };
                        pathFigure.Segments.Add(ls);
                    }
                }

            }
            else if (type == 'L' || type == 'P')
            {
                Point[] pts = new Point[curvepts.Length + 1];
                pts[0] = StartPoint;
                for (int i = 1; i < curvepts.Length; i++)
                {                   
                    double xc = double.Parse(curvepts[i].Split(':')[0].Trim());
                    double yc = double.Parse(curvepts[i].Split(':')[1].Trim());
                    pts[i] = new Point(xc, yc);
                    Point[] calc = new Point[] { pts[i - 1], pts[i] };
                    for (double t = 0.05; t <= 1; t+= 0.05)
                    {
                        Point px = CurveMaths.beziercoord(calc, t);
                        pathFigure.Segments.Add(new LineSegment() { Point = px });
                    }                  
                    if (i == curvepts.Length - 1)
                    { 
                        endXpos = xc;
                        endYpos = yc;
                    }
                }
            }
            else
            {
                // TODO PERFECT CURVE TYPE
            }

            if (Repeat > 1)
            {
                PathFigure reversed = new PathFigure();
                reversed.StartPoint = endpoint;               

                for(int i = pathFigure.Segments.Count - 1; i >= 1; i--)
                {
                    reversed.Segments.Add(pathFigure.Segments[i]);
                }
                reversed.Segments.Add(new LineSegment() { Point = StartPoint });
                
                ReversedBodyPG = new PathGeometry()
                {
                    Figures = new PathFigureCollection { reversed },
                };
                ReversedBodyPG.Freeze();
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
                StrokeThickness = Engine.CS - 8,
                Stroke = pinkgr,
                Opacity = 0,
            };
            BodyLine = new Path()
            {
                Data = pathGeometry,
                StrokeThickness = Engine.CS,
                Stroke = Brushes.White,
                Opacity = 0,
            };
            Canvas.SetZIndex(Body, -1);
            Engine.MainWindow.PlayArea.Children.Add(Body);
            Canvas.SetZIndex(BodyLine, -2);
            Engine.MainWindow.PlayArea.Children.Add(BodyLine);
        }
        public async void Spawn()
        {
            Stopwatch lifetime = new Stopwatch();
            lifetime.Start();
            await CircleLife();
            CircleAfterLife();
            while (lifetime.ElapsedMilliseconds < Preempt)
            {
                await Task.Delay(1);
            }
            lifetime.Stop();
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
                    if (i == Repeat)
                    {

                        StartCircle.Stroke = Brushes.White;
                        StartCircle.StrokeThickness = 4;
                        EndCircle.Stroke = Brushes.White;
                        EndCircle.Stroke = Brushes.White;
                    }
                    else if (i < Repeat - 1)
                    {
                        StartCircle.StrokeThickness = 4;
                        StartCircle.Stroke = new SolidColorBrush(Colors.Red);
                    }

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
            WasTicked = false;
            while (isInAnimation)
            {
                await Task.Delay(1);
            }
            dispatcherTimer.Stop();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            WasTicked = true;
            if (Engine.AnyButtonHeld && SliderBallHitbox.IsMouseDirectlyOver)
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
        }

        private void PathAnimationStoryboard_Completed(object? sender, EventArgs e)
        {
            isInAnimation = false;
            if (SliderBallHitbox.IsMouseDirectlyOver && Engine.AnyButtonHeld)
            {
                Engine.player.AddScore(30);
            }
            else
            {
                SliderBall.Fill = new SolidColorBrush(Colors.Red);
                endWasHit = false;
            }
        }
        async void AnimationDone()
        {
            Engine.MainWindow.PlayArea.Children.Remove(SliderBallHitbox);
            
            Canvas.SetZIndex(SliderBall, 1);
            Canvas.SetZIndex(EndCircle, 1);
            Canvas.SetZIndex(MainCircle, 1);
            Stopwatch fadeoutsw = new Stopwatch();
            fadeoutsw.Start();
            while (fadeoutsw.ElapsedMilliseconds < FadeOutTime)
            {
                MainCircle.Opacity = ((double)(FadeOutTime - fadeoutsw.ElapsedMilliseconds) / (double)FadeOutTime);
                EndCircle.Opacity = ((double)(FadeOutTime - fadeoutsw.ElapsedMilliseconds) / (double)FadeOutTime);
                SliderBall.Opacity = ((double)(FadeOutTime - fadeoutsw.ElapsedMilliseconds) / (double)FadeOutTime);
                Body.Opacity = ((double)(FadeOutTime - fadeoutsw.ElapsedMilliseconds) / (double)FadeOutTime);
                BodyLine.Opacity = ((double)(FadeOutTime - fadeoutsw.ElapsedMilliseconds) / (double)FadeOutTime) - 0.4;
                StartCircle.Opacity = ((double)(FadeOutTime - fadeoutsw.ElapsedMilliseconds) / (double)FadeOutTime);
                await Task.Delay(1);
            }
            fadeoutsw.Stop();
            Engine.MainWindow.PlayArea.Children.Remove(BodyLine);
            Engine.MainWindow.PlayArea.Children.Remove(MainCircle);
            Engine.MainWindow.PlayArea.Children.Remove(SliderBall);
            Engine.MainWindow.PlayArea.Children.Remove(Body);
            Engine.MainWindow.PlayArea.Children.Remove(StartCircle);
            Engine.MainWindow.PlayArea.Children.Remove(EndCircle);
            
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
                    Body.Opacity = (double)(sw.ElapsedMilliseconds / (double)(FadeIn));
                    BodyLine.Opacity = (double)(sw.ElapsedMilliseconds / (double)(FadeIn));
                    StartCircle.Opacity = (double)(sw.ElapsedMilliseconds / (double)(FadeIn));
                }

                ApproachCircle.Height = 3 * Engine.CS * (1 - (double)(sw.ElapsedMilliseconds / (double)Preempt)) + Engine.CS;
                ApproachCircle.Width = 3 * Engine.CS * (1 - (double)(sw.ElapsedMilliseconds / (double)Preempt)) + Engine.CS;
                Canvas.SetTop(ApproachCircle, Ypos - ApproachCircle.Height / 2);
                Canvas.SetLeft(ApproachCircle, Xpos - ApproachCircle.Width / 2);

                await Task.Delay(1);
            }
            Engine.MainWindow.PlayArea.Children.Remove(ApproachCircle);
            sw.Stop();
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
            sw.Stop();
            if (isAlive)
            {
                circleWasHit = false;
                Engine.player.ComboBreak();
            }
            isAlive = false;

        }
        async void ResultCheck()
        {
            
            int score = 0;
            if (!WasTicked)
            {
                wasFollowed = circleWasHit || endWasHit;
            }
            bool[] res = new bool[] { circleWasHit, wasFollowed, endWasHit };
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i])
                {
                    score++;
                }
            }

            if (score == 0)
            {
                Score = 0;
                Engine.player.Miss();
            }
            if (score == 1)
            {
                Score = 50;
                Engine.player.AddScore(Score);
            }
            if (score == 2)
            {
                Score = 100;
                Engine.player.AddScore(Score);
            }
            if (score == 3)
            {
                Score = 300;
                Engine.player.AddScore(Score);
            }
            if (Score != 300)
            {
                DrawResult();
            }
        }
        void Circle_ClickCheck(object sender, MouseButtonEventArgs e)
        {
            //(sender as Ellipse).Stroke = new SolidColorBrush(Colors.Red);
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
                if (sw.ElapsedMilliseconds >= Preempt - HitWindow50 && sw.ElapsedMilliseconds <= Preempt + HitWindow50)
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
}
