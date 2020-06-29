/*******************************************************
* Non-Parametric A* Pathfinding
* 
* AStar.cs
* 
* Defines a static class for the AStar algorithm. Used
* to find the shortest path between nodes.
* 
* Created by Ethan Hannen on 4/25/2020
*******************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NonParametricAStar
{
    public static class AStar
    {
        private static HashSet<CNode> Open;   // Nodes to be expanded
        private static double G;              // G (Total Current Cost)
        private static double E;              // C (Bounded Cost)       

        public static void Initialize()
        {
            Open = new HashSet<CNode>();
            G = double.MaxValue;
            E = double.MaxValue;
        }

        private static async Task<int> Draw_Solution(double Timing)
        {
            CNode Next = Var.Goal;
            PointCollection Path = new PointCollection();
            Polyline Solution = new Polyline();

            while (Next != null)
            {
                Path.Add(new Point(Next.Location.X, Next.Location.Y));
                Next = Next.Parent;
                await Task.Delay(Var.Speed);
            }
            Solution.Points = Path;
            Graph.Add_Solution(Solution, Timing);
            return 0;
        }

        private static void Prune_Nodes()
        {
            HashSet<CNode> ToRemove = new HashSet<CNode>();
            foreach (CNode N in Open)
            {
                N.E = Get_Cost(N);
                if (N.E >= G)
                    ToRemove.Add(N);
            }
            foreach (CNode N in ToRemove)
                Open.Remove(N);
        }

        private static double Get_Cost(CNode N)
        {
            return (G - N.G) / N.H;
        }

        public static CNode Get_Best()
        {
            double Max = 0.0;
            CNode MaxNode = null;
            foreach (CNode N in Open)
            {
                if (MaxNode == null)
                    MaxNode = N;
                if (N == Var.Goal)
                    return Var.Goal; // Don't want to divide by 0!

                double E = Get_Cost(N);

                if (E > Max)
                {
                    Max = E;
                    MaxNode = N;
                }
            }
            return MaxNode;
        }

        private static bool Improve_Solution()
        { // Should be good
            CNode Current = null;
            double newE;

            while (Open.Count > 0)
            {
                Current = Get_Best();
                if (Current == null)
                    return false;
                Open.Remove(Current);
                newE = Get_Cost(Current);

                if (newE < E) E = newE;
                if (Current == Var.Goal)
                {
                    G = Current.G;
                    return true;
                }

                foreach (CNode N in Current.Get_Neighbors())
                {
                    double newG = Current.G + Current.Get_Distance_To(N);
                    if (newG < N.G)
                    {
                        N.G = newG;
                        N.Parent = Current;
                        if ((N.G + N.H) < G)
                        {
                            N.E = Get_Cost(N);
                            Open.Add(N);
                        }
                    }
                }
            }
            return false;
        }

        public static async void Run_Non_Parametric()
        {
            var Timer = new System.Diagnostics.Stopwatch();
            Open.Add(Var.Start);

            while (Open.Count > 0)
            {
                Timer.Start(); // Start the clock
                if (!Improve_Solution())
                    return; // Failed to ever reach a goal
                Timer.Stop(); // End the clock
                await Draw_Solution(Timer.Elapsed.TotalMilliseconds);
                Prune_Nodes();
            }
        }

        public static double Get_Distance(Point A, Point B, Point P)
        {
            Point Closest = new Point();
            double dX = B.X - A.X;
            double dY = B.Y - A.Y;

            double T = ((P.X - A.X) * dX + (P.Y - A.Y) * dY) / (dX * dX + dY * dY);

            if (T < 0)
            {
                Closest = new Point(A.X, A.Y);
                dX = P.X - A.X;
                dY = P.Y - A.Y;
            }
            else if (T > 1)
            {
                Closest = new Point(B.X, B.Y);
                dX = P.X - B.X;
                dY = P.Y - B.Y;
            }
            else
            {
                Closest = new Point(A.X + T * dX, A.Y + T * dY);
                dX = P.X - Closest.X;
                dY = P.Y - Closest.Y;
            }

            return Math.Sqrt(dX * dX + dY * dY);
        }


        // TO DO: Finish non-linear alrogithm. Experimental.
        /*
        public static void Run_Linear()
        {
            var Timer = new System.Diagnostics.Stopwatch();
            CNode Current = Var.Start;

            Timer.Start(); // Start the clock
            while (Current != Var.Goal)
            {
                CNode BestN = Current;
                double Best = double.MaxValue;

                foreach (CNode N in Current.Neighbors)
                {
                    if (N == Var.Goal)
                    {
                        BestN = Var.Goal;
                        break;
                    }
                    double D = Get_Distance(Current.Location, Var.Goal.Location, N.Location);
                    double RealDistance = N.Get_Distance_To(Var.Goal) + D;
                    if (RealDistance < Best)
                        BestN = N;
                }
                BestN.Parent = Current;
                Current = BestN;
            }
            Timer.Stop(); // End the clock
            //Draw_Solution(Timer.Elapsed.TotalMilliseconds);
        }
        */
    }
}
