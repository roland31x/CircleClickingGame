using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CircleClickingGame
{
    public static class CurveMaths
    {
        public static Point beziercoord(Point[] pointArray, double t)
        {
            double bx = 0, by = 0;
            int n = pointArray.Length - 1; // degree

            if (n == 1)
            { // if linear
                bx = (1 - t) * pointArray[0].X + t * pointArray[1].X;
                by = (1 - t) * pointArray[0].Y + t * pointArray[1].Y;
            }
            else if (n == 2)
            { // if quadratic
                bx = (1 - t) * (1 - t) * pointArray[0].X + 2 * (1 - t) * t * pointArray[1].X + t * t * pointArray[2].X;
                by = (1 - t) * (1 - t) * pointArray[0].Y + 2 * (1 - t) * t * pointArray[1].Y + t * t * pointArray[2].Y;
            }
            else if (n == 3)
            { // if cubic
                bx = (1 - t) * (1 - t) * (1 - t) * pointArray[0].X + 3 * (1 - t) * (1 - t) * t * pointArray[1].X + 3 * (1 - t) * t * t * pointArray[2].X + t * t * t * pointArray[3].X;
                by = (1 - t) * (1 - t) * (1 - t) * pointArray[0].Y + 3 * (1 - t) * (1 - t) * t * pointArray[1].Y + 3 * (1 - t) * t * t * pointArray[2].Y + t * t * t * pointArray[3].Y;
            }
            else
            { // generalized equation
                for (var i = 0; i <= n; i++)
                {
                    bx += binomialCoef(n, i) * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * pointArray[i].X;
                    by += binomialCoef(n, i) * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * pointArray[i].Y;
                }
            }

            return new Point(bx, by);
        }
        static long binomialCoef(long n, long k)
        {
            long r = 1;

            if (k > n)
                return 0;

            for (var d = 1; d <= k; d++)
            {
                r *= n--;
                r /= d;
            }

            return r;
        }
    }
}
