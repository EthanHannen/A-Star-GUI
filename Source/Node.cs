/*******************************************************
* Non-Parametric A* Pathfinding
* 
* Node.cs
* 
* Defines node class within the graph. Holds neighboring
* nodes and parent shapes.
* 
* Created by Ethan Hannen on 4/25/2020
*******************************************************/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;

namespace NonParametricAStar
{
    public class CNode
    {
        public Point Location;            // X,Y coordinates
        public Shape Poly = null;         // Parent polygon that contains this point
        public HashSet<CNode> Neighbors; // Holds adjacent nodes
        public double F = double.MaxValue;
        public double G = double.MaxValue;
        public double H = double.MaxValue;
        public double E = 0.0;
        public CNode Parent = null;

        public CNode(Point P, Shape S, CNode Goal = null)
        {
            Location = P;
            Poly = S;
            Neighbors = new HashSet<CNode>();
            H = (Goal == null ? 0.0 : this.Get_Distance_To(Goal));
            F = H;
        }
        public void Add_Neighbor(CNode N)
        {
            Neighbors.Add(N);
        }
        public void Remove_Neighbor(CNode N)
        {
            Neighbors.Remove(N);
        }
        public HashSet<CNode> Get_Neighbors()
        {
            return Neighbors;
        }

        public double Get_Distance_To(CNode N)
        {   // Euclidean Distance
            if (this == N)
                return 0.0;
            double X = this.Location.X - N.Location.X;
            double Y = this.Location.Y - N.Location.Y;
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }
    }
}
