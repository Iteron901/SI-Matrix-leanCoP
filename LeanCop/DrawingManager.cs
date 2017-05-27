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
        private static double fontSize = 40;
        private double[] rowCoords;
        private double[] colCoords;

        public DrawingManager(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void drawProof(String matrix, String connections)
        {
            canvas.Children.Clear();

            String[,] matrix2d = get2dMatrix(matrix);

            drawConnections(connections);
            drawMatrix(matrix2d);

        }

        private string[,] get2dMatrix(string matrix)
        {
            var re = new Regex(@"\[[-a-zA-Z]+(,[-a-zA-Z]+)*\]");
            String[] columns = re.Matches(matrix)
                .OfType<Match>()
                .Select(m => m.Groups[0].Value)
                .ToArray();

            int heigth = 0, width = 0;
            width = columns.Length;
            foreach (String col in columns)
            {
                int colHeigth = col.Split(',').Length;
                if (colHeigth > heigth)
                    heigth = colHeigth;
            }

            String[,] ret = new String[width, heigth];

            for (int x = 0; x < width; x++ )
            {
                String[] rows = columns[x].Split(',');
                for (int y = 0; y < rows.Length; y++)
                {
                    ret[x, y] = rows[y].Replace("[", "").Replace("]","");
                }

            }

            return ret;
        }

        private void drawMatrix(string[,] matrix)
        {
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Point position = getPosition(x, y);
                    putText(position.X, position.Y, matrix[x, y]);
                }
            }
        }

        private Point getPosition(int x, int y)
        {
            return new Point(x*100.0+50, y*100.0+50);
        }

        private void drawConnections(string connections)
        {
            var re = new Regex(@"\([0-9]+,[0-9]+\),\([0-9]+,[0-9]+\)");
            String[] pairs = re.Matches(connections)
                .OfType<Match>()
                .Select(m => m.Groups[0].Value)
                .ToArray();

            var num = new Regex(@"[0-9]+");
            foreach (String pair in pairs)
            {
                String[] nums = num.Matches(pair)
                    .OfType<Match>()
                    .Select(m => m.Groups[0].Value)
                    .ToArray();
                Line line = new Line();
                var start = getPosition(int.Parse(nums[0]), int.Parse(nums[1]));
                var end = getPosition(int.Parse(nums[2]), int.Parse(nums[3]));

                drawConnection(start, end);

            }
        }

        private void drawConnection(Point start, Point end)
        {
            Point mid = new Point();
            mid.X = (start.X + end.X) / 2;
            mid.Y = (start.Y + end.Y) / 2;
            if (start.Y == end.Y)
                mid.Y -= 40;

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

        private void putText(double x, double y, string text)
        {
            TextBlock tb = new TextBlock { Text = text, FontSize = fontSize };

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            tb.Arrange(new Rect(tb.DesiredSize));

            var offsetX = -tb.ActualWidth / 2;
            var offsetY = -tb.ActualHeight / 2;

            Canvas.SetLeft(tb, x + offsetX);
            Canvas.SetTop(tb, y + offsetY);
            canvas.Children.Add(tb);
        }

        private Size displaySize(String text)
        {
            TextBlock tb = new TextBlock { Text = text, FontSize = fontSize };

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            tb.Arrange(new Rect(tb.DesiredSize));

            return new Size(tb.ActualWidth, tb.ActualHeight);
        }
    }
}
