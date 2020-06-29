/*******************************************************
* Non-Parametric A* Pathfinding
* 
* Util.cs
* 
* Various utility methods for graph and AStar support
* 
* Created by Ethan Hannen on 4/25/2020
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NonParametricAStar
{
    public class Pair<T, U>
    {
        public T Node { get; set; }
        public U Shape { get; set; }

        public Pair(T N, U S)
        {
            Node = N;
            Shape = S;
        }
    };

    public class Pos<T, U>
    {
        public T X { get; set; }
        public U Y { get; set; }

        public Pos(T X, U Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public static class Util
    {
        public static T Clamp<T>(T V, T Min, T Max) where T : IComparable<T>
        {
            if (V.CompareTo(Min) < 0)
                return Min;
            else if (V.CompareTo(Max) > 0)
                return Max;
            else
                return V;
        }

        public static async void RaiseError(TextBlock T, String S, CancellationToken K)
        {
            T.Text = "Error: " + S;
            T.Visibility = Const.VO;
            T.Opacity = 1.00;
            await Task.Delay(1000);
            for (int x = 100; x > 0; x--)
            {
                T.Opacity -= 0.01;
                await Task.Delay(10);
                if (K.IsCancellationRequested)
                    return;
            }
            T.Visibility = Const.V_;
        }

        public static void CleanInput(TextBox T)
        {
            String S = T.Text;
            S = S.Replace(",", "");
            for (int x = 0; x < S.Length; x++)
            {
                int Num = 0;
                if (!int.TryParse(S[x].ToString(), out Num))
                    S = S.Substring(0, x) + (S.Length > 0 ? S.Substring(x + 1) : "");
            }

            if (S.Length == 0)
            {
                T.Text = S;
                return;
            }
            T.CaretIndex = T.Text.Length;
            T.ScrollToEnd();
            T.Focus();
        }

        public static bool CollisionExists(HashSet<PathGeometry> Shapes, Geometry L)
        {
            IntersectionDetail D;
            foreach (PathGeometry G in Shapes)
            {
                if (G == null)
                    return false;
                D = G.FillContainsWithDetail(L);
                if (D != IntersectionDetail.Empty)
                    return true;
            }
            return false;
        }

        public static double DistanceBetween(Point A, Point B)
        {
            double X = A.X - B.X;
            double Y = A.Y - B.Y;
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public static PathGeometry Reposition(ref PointCollection Points)
        {
            Random Chance = new Random();
            PathGeometry G = new PathGeometry();
            PathFigure F = new PathFigure();
            PointCollection NewPoints = new PointCollection();
            int xMin = Const.MAX_X;
            int xMax = Const.MIN_X;
            int yMin = Const.MAX_Y;
            int yMax = Const.MIN_Y;

            foreach (Point P in Points)
            {
                if (P.X < xMin) xMin = (int)P.X;
                if (P.X > xMax) xMax = (int)P.X;
                if (P.Y < yMin) yMin = (int)P.Y;
                if (P.Y > yMax) yMax = (int)P.Y;
            }

            int xShift, yShift;
            bool Left = Chance.Next(0, 1) == 1 ? true : false;

            if (Left)
            {
                xShift = Chance.Next(0, xMin - Const.MIN_X);
                yShift = Chance.Next(0, yMin - Const.MIN_Y);
                xShift = 0 - xShift;
                yShift = 0 - yShift;
            }
            else
            {
                xShift = Chance.Next(0, Const.MAX_X - xMax);
                yShift = Chance.Next(0, Const.MAX_Y - yMax);
            }
            foreach (Point P in Points)
            {
                Point NP = new Point(P.X + xShift, P.Y + yShift);
                if (NP.X < Const.MIN_X || NP.X > Const.MAX_X || NP.Y < Const.MIN_Y || NP.Y > Const.MAX_Y)
                    return null;
                if (P == Points.First<Point>())
                    F.StartPoint = NP;
                else
                    F.Segments.Add(new LineSegment(NP, false));
                NewPoints.Add(NP);
            }
            Points = NewPoints;
            G.Figures.Add(F);
            return G;
        }
    }
}
