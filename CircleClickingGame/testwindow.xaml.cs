using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircleClickingGame
{
    /// <summary>
    /// Interaction logic for testwindow.xaml
    /// </summary>
    public partial class testwindow : Window
    {
        public testwindow()
        {
            InitializeComponent();
            SliderTest.t = this;
            SliderTest test = new SliderTest(0, 0);
        }


    }
    class SliderTest
    {
        public static testwindow t;
        public SliderTest(int x, int y)
        {
            DoubleAnimationUsingPath d = new DoubleAnimationUsingPath();

            PathFigure pathFigure = new PathFigure();

            pathFigure.StartPoint = new Point(x, y);

            PolyBezierSegment lineSegment1 = new PolyBezierSegment();
            lineSegment1.Points.Add(new Point(100, 20));
            lineSegment1.Points.Add(new Point(300, 200));
            lineSegment1.Points.Add(new Point(500, 150));
            lineSegment1.Points.Add(new Point(200, 300));
            lineSegment1.Points.Add(new Point(500, 300));



            pathFigure.Segments.Add(lineSegment1);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();

            pathGeometry.Figures.Add(pathFigure);

            Path path = new Path()
            {
                Data = pathGeometry,
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            t.MainCanvas.Children.Add(path);
            Canvas.SetTop(path, 0);
            Canvas.SetLeft(path, 0);

        }
    }
}
