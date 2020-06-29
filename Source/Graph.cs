/*******************************************************
* Non-Parametric A* Pathfinding
* 
* Graph.cs
* 
* Contains class methods for logic and display of all 
* graph obstacles, paths, nodes, and goals.
* 
* Created by Ethan Hannen on 4/25/2020
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NonParametricAStar
{
    public static class Graph
    {
        public static void Update_Data()
        {
            Text.Nodes.Text = "Nodes: " + Objects.Nodes.Count().ToString();
            Text.Edges.Text = "Edges: " + Objects.Edges.Count().ToString();
        }

        // Reset Methods
        private static void Clear_Map()
        {
            Box.Shapes.Children.Clear();
            Clear_Edges();
            Clear_Solution();
            Objects.Shapes.Clear();
            Objects.Nodes.Clear();
        }

        public static void Clear_Solution()
        {
            Box.Paths.Children.Clear();
            Text.Timing.Text = "";
            Var.Solutions = 0;
            AStar.Initialize();
        }

        public static void Add_Solution(Polyline Solution, double Timing)
        {
            foreach (Polyline V in Box.Paths.Children.OfType<Polyline>())
                V.Stroke = Brushes.Red;
            Solution.Stroke = Brushes.Chartreuse;
            Solution.StrokeThickness = 2;
            String S = String.Format("Solution #{0,2:D} | ", ++Var.Solutions);
            String T = String.Format("{0,4:F} ms\n", Timing);
            Text.Timing.Text += S + T;
            Box.Paths.Children.Add(Solution);
        }

        // Edge & Path Visuals

        public static void Clear_Edges()
        {
            UIElement U = null;
            for (int x = Box.Edges.Children.Count - 1; x >= 0; x--)
            {
                if ((U = Box.Edges.Children[x]) is Polyline)
                    Box.Edges.Children.Remove(U);
            }
            Objects.Edges.Clear();
        }

        public static void Clear_End_Edges(CNode N)
        {
            HashSet<Polyline> H;

            if (N == Var.Goal)
                H = Objects.G_Edges;
            else
                H = Objects.S_Edges;

            foreach (Polyline P in H.ToList())
            {
                Box.Edges.Children.Remove(P);
                PointCollection PC = P.Points;
                Objects.Edges.Remove(PC);
            }
            H.Clear();

            foreach (CNode C in N.Neighbors.ToList())
                N.Remove_Neighbor(N);
            N.Neighbors.Clear();
            Update_Data();
        }

        public static void Rebuild_End_Edges(CNode N)
        {
            foreach (CNode A in Objects.Nodes.ToList())
            { 
                LineGeometry L = new LineGeometry(N.Location, A.Location);

                if (!Util.CollisionExists(Objects.Shapes, L))
                    Connect_Nodes(N, A);
            }
            Update_Data();
        }

        private static void Find_Edges()
        {
            foreach (CNode A in Objects.Nodes)
            {
                foreach (CNode B in Objects.Nodes)
                {
                    if (A.Poly == B.Poly)
                        continue;

                    LineGeometry L = new LineGeometry(A.Location, B.Location);

                    if (!Util.CollisionExists(Objects.Shapes, L))
                        Connect_Nodes(A, B);
                }
            }
            Update_Data();
        }

        // Create Edges and Neighbors
        private static void Connect_Nodes(CNode A, CNode B)
        {
            PointCollection Edge = new PointCollection();
            Polyline Line = new Polyline();
            Line.Stroke = System.Windows.Media.Brushes.Chartreuse;
            Line.StrokeThickness = 1;

            A.Add_Neighbor(B);
            B.Add_Neighbor(A);
            Edge.Add(A.Location);
            Edge.Add(B.Location);
            Objects.Edges.Add(Edge);
            Line.Points = Edge;
            Box.Edges.Children.Add(Line);
            if (A == Var.Goal || B == Var.Goal)
                Objects.G_Edges.Add(Line);
            if (A == Var.Start || B == Var.Start)
                Objects.S_Edges.Add(Line);
        }

        private static void Create_Shapes(PointCollection[] Outlines)
        {
            foreach (PointCollection Points in Outlines)
            {
                Polygon Poly = new Polygon();
                CNode First = null, Last = null, Prev = null;

                foreach (Point P in Points)
                {
                    CNode Node = new CNode(P, Poly, Var.Goal);
                    if (P == Points.First<Point>())
                        First = Node;
                    else if (P == Points.Last<Point>())
                        Last = Node;
                    if (Prev != null)
                        Connect_Nodes(Node, Prev);
                    Prev = Node;
                    Objects.Nodes.Add(Node);
                }

                Connect_Nodes(First, Last);

                Poly.Points = Points;
                Poly.Fill = new SolidColorBrush(Var.ShapeColor);
                Poly.HorizontalAlignment = HorizontalAlignment.Left;
                Poly.VerticalAlignment = VerticalAlignment.Top;
                Box.Shapes.Children.Add(Poly);
            }
        }

        public static void Generate_Shapes() // Generate Points and Generate PathGeometry
        {
            Clear_Map();

            PointCollection[] Outlines = new PointCollection[Var.Shapes];

            int SizeMod = (int)Math.Ceiling(Var.Shapes / 20.0); // 1~5
            int MaxDistance = Const.MAX_DISTANCE - Var.Shapes / 2;

            for (int x = 0; x < Var.Shapes; x++)
            {
                PointCollection Points = new PointCollection();
                PathGeometry G = new PathGeometry();

                Random Chance = new Random();
                bool Colliding = true;
                do
                {
                    Points = new PointCollection();
                    int s = Chance.Next(0, Const.Shapes.GetLength(0));

                    for (int y = 0; y < Const.Shapes.GetLength(1); y++)
                    {
                        if (Const.Shapes[s, y, 0] == -1)
                            break;
                        Point P = new Point(Const.Shapes[s, y, 0], Const.Shapes[s, y, 1]);
                        P.X /= SizeMod;
                        P.Y /= SizeMod;

                        int Transform = Chance.Next(0, 1); // +/- Determination
                        int Distance = Chance.Next(0, MaxDistance);
                        P.X += Transform == 1 ? Distance : 0 - Distance;

                        Transform = Chance.Next(0, 1);
                        Distance = Chance.Next(0, MaxDistance);
                        P.Y += Transform == 1 ? Distance : 0 - Distance;
                        Points.Add(P);
                    }
                    G = Util.Reposition(ref Points);
                    if (G != null)
                        Colliding = Util.CollisionExists(Objects.Shapes, G);
                } while (Colliding);

                Objects.Shapes.Add(G);
                Outlines[x] = Points;
            }
            Create_Shapes(Outlines);
            Set_Goals();
            Find_Edges();
        }

        private static void Set_Goals() // TO DO - SET RANDOM GOALS
        {
            Point PS = new Point();
            Shape SS = new Ellipse();
            Point PG = new Point();
            Shape SG = new Ellipse();

            Random Chance = new Random();
            bool Colliding = true;
            PathGeometry G = new PathGeometry();
            PathFigure F = new PathFigure();

            do
            {
                G = new PathGeometry();
                F = new PathFigure();
                PS.X = Chance.Next(Const.MIN_X, Const.MAX_X);
                PS.Y = Chance.Next(Const.MIN_Y, Const.MAX_Y);
                F.StartPoint = PS;
                F.Segments.Add(new LineSegment(new Point(PS.X + 1, PS.Y), false));
                G.Figures.Add(F);
                Colliding = Util.CollisionExists(Objects.Shapes, G);
            } while (Colliding);

            Colliding = true;

            do
            {
                G = new PathGeometry();
                F = new PathFigure();
                PG.X = Chance.Next(Const.MIN_X, Const.MAX_X);
                PG.Y = Chance.Next(Const.MIN_Y, Const.MAX_Y);
                F.StartPoint = PG;
                F.Segments.Add(new LineSegment(new Point(PG.X + 1, PG.Y), false));
                G.Figures.Add(F);
                Colliding = Util.CollisionExists(Objects.Shapes, G);
            } while (Colliding);
            
            SS.Fill = new SolidColorBrush(Colors.Red);
            SG.Fill = new SolidColorBrush(Colors.Chartreuse);

            SS.Margin = new Thickness(PS.X - 5, PS.Y - 5, 0, 0);
            SS.Width = 10;
            SS.Height = 10;
            SS.HorizontalAlignment = HorizontalAlignment.Left;
            SS.VerticalAlignment = VerticalAlignment.Top;

            SG.Margin = new Thickness(PG.X - 5, PG.Y - 5, 0, 0);
            SG.Width = 10;
            SG.Height = 10;
            SG.HorizontalAlignment = HorizontalAlignment.Left;
            SG.VerticalAlignment = VerticalAlignment.Top;
            Box.Shapes.Children.Add(SS);
            Box.Shapes.Children.Add(SG);

            CNode NG = new CNode(PG, SG);
            CNode NS = new CNode(PS, SS, NG);
            NS.G = 0.0; // Starting node initializes with g(s) = 0 | All others get max value
            Objects.Nodes.Add(NG);
            Objects.Nodes.Add(NS);
            Var.Start = NS; Var.Goal = NG;
        }

        // Movement of Nodes
        public static void Capture_Node(UIElement O, Point P)
        {
            Var.StartPos = P;
            O.CaptureMouse();
        }

        public static void Release_Node(UIElement O)
        {
            Var.StartPos = null;
            try
            {
                O.ReleaseMouseCapture();
                Clear_End_Edges(Var.DragNode);
                Rebuild_End_Edges(Var.DragNode);
            }
            catch (Exception e)
            {
                Text.Error.Text = e.Message;
                return;
            }
        }

        public static void Move_Node(Ellipse U, Point P)
        {
            if (Var.StartPos == null)
                return;

            P.X = Util.Clamp(P.X - Var.StartPos.Value.X, 0.0, (double)Box.Shapes.MaxWidth);
            P.Y = Util.Clamp(P.Y - Var.StartPos.Value.Y, 0.0, (double)Box.Shapes.MaxHeight);
            U.Margin = new Thickness(P.X - 5, P.Y - 5, 0, 0);
            Var.DragNode.Location.X = P.X;
            Var.DragNode.Location.Y = P.Y;
        }
    }
}
