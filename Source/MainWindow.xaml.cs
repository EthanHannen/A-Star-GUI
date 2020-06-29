/*******************************************************
* Non-Parametric A* Pathfinding
* 
* MainWindow.cs
* 
* Contains all methods for visual objects as well as the
* entry point for the program. 
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NonParametricAStar
{
    public static class Const
    {
        // Conditions
        public const bool PASS = true;
        public const bool FAIL = false;

        // Positioning
        public const int MIN_Y = 0;
        public const int MAX_Y = 500;
        public const int MIN_X = 0;
        public const int MAX_X = 800;
        public const int MAX_DISTANCE = 55;

        // Controls
        public const Visibility VO = Visibility.Visible;
        public const Visibility V_ = Visibility.Collapsed;

        // Default Shape Coordinates
        public static int[,,] Shapes = new int[8, 6, 2] {
            // [Shape, Vertices, Vertex[x,y]]
            { {270,270}, {310,130}, {350,270}, { -1, -1}, { -1, -1}, { -1, -1} }, // Triangle
            { {480,390}, {440,240}, {550,330}, { -1, -1}, { -1, -1}, { -1, -1} }, // Triangle
            { {500,280}, {500,100}, {620,100}, {620,280}, { -1, -1}, { -1, -1} }, // Rectangle
            { {150,420}, {150,320}, {430,320}, {430,420}, { -1, -1}, { -1, -1} }, // Rectangle
            { {370,210}, {370,110}, {430,100}, {480,150}, { -1, -1}, { -1, -1} }, // Trapezoid
            { {700,290}, {645,120}, {700, 90}, {730,130}, { -1, -1}, { -1, -1} }, // Trapezoid
            { {135,255}, {105,175}, {185, 85}, {265,165}, {215,270}, { -1, -1} }, // Pentagon
            { {585,385}, {585,325}, {645,275}, {705,315}, {705,385}, {645,425} }  // Hexagon
        };
    }
    public static class Box
    {
        public static Grid Shapes = new Grid();               // Shape Continer
        public static Grid Edges = new Grid();               // Edges Container
        public static Grid Paths = new Grid();               // Solution Container
        public static Grid Data = new Grid();               // Metrics Container
    }

    public static class Text
    {
        public static TextBlock Nodes  = new TextBlock();
        public static TextBlock Edges  = new TextBlock();
        public static TextBlock Timing = new TextBlock();
        public static TextBlock Error  = new TextBlock();
    }

    public static class Var
    {
        // Graph Data
        public static int Solutions = 0;                      // Number of Solutions
        public static int Shapes = 0;                         // Number of Shapes
        public static int Speed = 0;                          // Speed of Solution Mapping
        public static CNode Start;                            // Origination Node for the agent
        public static CNode Goal;                             // Agent's specified target Node
        public static CNode DragNode;                         // Node being dragged by mouse

        // Node Drag Events
        public static Nullable<Point> StartPos = null;        // Location From
        public static object DragObject = null;               // Ellipse Being Dragged
        public static bool isDragging = false;                // User is Dragging Mouse

        public static Color ShapeColor = new Color();
    }

    public static class Objects
    {
        public static HashSet<CNode> Nodes
            = new HashSet<CNode>();                           // Node Collection
        public static HashSet<PointCollection> Edges
            = new HashSet<PointCollection>();                 // Edge Collection
        public static HashSet<Polyline> G_Edges
            = new HashSet<Polyline>();                        // Edges Connected to Goal
        public static HashSet<Polyline> S_Edges
            = new HashSet<Polyline>();                        // Edges Connected to Goal
        public static HashSet<PathGeometry> Shapes 
            = new HashSet<PathGeometry>();                    // Shape Collection
    }

    public partial class MainWindow : Window
    {
        CancellationTokenSource CTS = null;

        public MainWindow()
        {
            InitializeComponent();
            Box.Shapes = G_Shapes;
            Box.Edges = G_Edges;
            Box.Paths = G_Paths;
            Box.Data = G_Data;
            Text.Nodes = T_Nodes;
            Text.Edges = T_Edges;
            Text.Timing = T_Times;
            Text.Error = T_Error;
            Box.Edges.Visibility = Const.V_;
            Set_Color(45, 35, 255);
        }

        private void Show_Edges(object sender, RoutedEventArgs e)
        {
            Box.Edges.Visibility = (bool)CB_Show_Edges.IsChecked ? Const.VO : Const.V_;
        }

        private void Set_Solution_Speed(object sender, RoutedEventArgs e)
        {
            Var.Speed = Var.Speed == 0 ? 50 : 0;
        }

        private void Set_Num_Shapes(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Var.Shapes = (int)(sender as Slider).Value;
            Slider_Shapes_Head.Text = "Number of Shapes [" + Var.Shapes.ToString() + "]";
        }

        private void Set_Color(int R, int G, int B)
        {
            Var.ShapeColor = Color.FromRgb((byte)R, (byte)G, (byte)B);
            foreach (Polygon P in Box.Shapes.Children.OfType<Polygon>())
                P.Fill = new SolidColorBrush(Var.ShapeColor);
        }

        private void Set_Color(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sB == null)
                return;
            Set_Color((int)sR.Value, (int)sG.Value, (int)sB.Value);
        }

        private void Generate_Shapes(object sender, RoutedEventArgs e)
        {
            Graph.Generate_Shapes();
            AddNodeMethods();
        }

        private void Find_Path(object sender, RoutedEventArgs e)
        {
            if (Var.Shapes == 0)
            {
                if (CTS != null)
                    CTS.Cancel();
                CTS = new CancellationTokenSource();
                Util.RaiseError(T_Error, "No shapes generated.", CTS.Token);
                return;
            }
            // TO DO: Finish experimental linear algorithm
            // Defaulting to Non-Parametric for now
            //if (A1.IsChecked == true)
                AStar.Run_Non_Parametric();
            //else
              //AStar.Run_Linear();
        }

        private void Clear_Solution(object sender, RoutedEventArgs e)
        {
            Graph.Clear_Solution();
        }

        public void NodeButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (sender as Shape == Var.Goal.Poly)
                Var.DragNode = Var.Goal;
            else
                Var.DragNode = Var.Start;
            Point P = e.GetPosition(sender as UIElement);
            Graph.Capture_Node(sender as UIElement, P);
        }

        public void NodeButtonUp(object sender, MouseButtonEventArgs e)
        {
            Graph.Release_Node(sender as UIElement);
        }

        public void NodeMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Graph.Move_Node(sender as Ellipse, e.GetPosition(Box.Shapes));
        }

        public void NodeEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        public void NodeLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void AddNodeMethods()
        {
            foreach (Ellipse E in Box.Shapes.Children.OfType<Ellipse>())
            {
                E.MouseLeftButtonDown += new MouseButtonEventHandler(NodeButtonDown);
                E.MouseLeftButtonUp += new MouseButtonEventHandler(NodeButtonUp);
                E.MouseEnter += new MouseEventHandler(NodeEnter);
                E.MouseLeave += new MouseEventHandler(NodeLeave);
                E.MouseMove += new MouseEventHandler(NodeMove);
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Visit_Source(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.freepik.com");
        }
    }
}
