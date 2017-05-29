using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows;

namespace LeanCop
{
    class DrawingManager
    {
        private Canvas canvas;
        private static double fontSize = 30;
        private String[,] matrix;
        private double[] _colWidth;
        private double[] _colOffset;

        public DrawingManager(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void drawProof(String[,] matrix, List<Connection> connections)
        {
            canvas.Children.Clear();

            this.matrix = matrix;
            this._colWidth = new double[this.matrix.GetLength(0)];
            this._colOffset = new double[this.matrix.GetLength(0)];

            drawConnections(connections);
            drawMatrix();

            Size canvasSize = drawingSize();
            canvas.Width = canvasSize.Width;
            canvas.Height = canvasSize.Height;
        }

        private Size drawingSize()
        {
            double width = colOffset(lastCol()) + colWidth(lastCol());
            double heigth = matrix.GetLength(1) * 100 - 50;
            return new Size(width, heigth);
        }

        private void drawMatrix()
        {
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Point position = getPosition(x, y);
                    double textWidth = colWidth(x);
                    putText(position.X, position.Y, matrix[x, y], textWidth);
                }
            }
        }

        private double colWidth(int n)
        {
            // Lazy evaluation
            if (this._colWidth[n] != 0)
                return this._colWidth[n];

            int height = matrix.GetLength(1);

            double maxWidth = 0;

            for (int i = 0; i < height; i++)
            {
                double k = displaySize(matrix[n, i]).Width;
                if (maxWidth < k)
                    maxWidth = k;
            }

            return this._colWidth[n] = maxWidth;
        }

        private double colOffset(int n)
        {
            if (n == 0)
                return 0;
            // Lazy evaluation
            if (this._colOffset[n] != 0)
                return this._colOffset[n];

            double offset = colOffset(n - 1) + colWidth(n - 1) + 20;

            return this._colOffset[n] = offset;
        }

        private Point getPosition(int x, int y)
        {
            
            return new Point(colOffset(x), y*100);
        }

        private void drawConnections(List<Connection> connections)
        {
            foreach (Connection c in connections)
            {
                drawConnection(c.x1, c.y1, c.x2, c.y2);
            }
        }

        private void drawConnection(int x1, int y1, int x2, int y2)
        {
            var off1 = displaySize(matrix[x1, y1]);
            off1.Width = colWidth(x1) - off1.Width / 2;
            var off2 = displaySize(matrix[x2, y2]);
            off2.Width = colWidth(x2) - off2.Width / 2;

            Point start = getPosition(x1, y1);
            Point end = getPosition(x2, y2);
            start.Offset(off1.Width, off1.Height / 2);
            end.Offset(off2.Width, off2.Height / 2);

            Point mid = new Point();
            mid.X = (start.X + end.X) / 2;
            mid.Y = (start.Y + end.Y) / 2;
            if (start.Y == end.Y)
                mid.Y += y1 == lastRow() ? 40 : -40;

            BezierSegment b = new BezierSegment(start, mid, end, true);
            PathGeometry path = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = start;
            path.Figures.Add(pathFigure);

            pathFigure.Segments.Add(b);
            System.Windows.Shapes.Path p = new Path();
            p.Stroke = Brushes.Red;
            p.StrokeThickness = 2;
            p.Data = path;

            canvas.Children.Add(p);
        }

        private void putText(double x, double y, string text, double width)
        {
            TextBlock tb = new TextBlock { Text = text, FontSize = fontSize };
            tb.Width = width;
            tb.TextAlignment = TextAlignment.Right;
            Canvas.SetLeft(tb, x);
            Canvas.SetTop(tb, y);
            canvas.Children.Add(tb);
        }

        private Size displaySize(String text)
        {
            TextBlock tb = new TextBlock { Text = text, FontSize = fontSize };

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            tb.Arrange(new Rect(tb.DesiredSize));

            return new Size(tb.ActualWidth, tb.ActualHeight);
        }

        private int lastCol()
        {
            return this.matrix.GetLength(0) - 1;
        }

        private int lastRow()
        {
            return this.matrix.GetLength(1) - 1;
        }
    }
}
