﻿using System;
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

        public DrawingManager(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void drawProof(String matrix, String connections)
        {
            canvas.Children.Clear();

            this.matrix = get2dMatrix(matrix);

            drawConnections(connections, this.matrix);
            drawMatrix(this.matrix);

            Size canvasSize = drawingSize();
            canvas.Width = canvasSize.Width;
            canvas.Height = canvasSize.Height;
        }

        private Size drawingSize()
        {
            int cols = this.matrix.GetLength(0);
            double width = colOffset(this.matrix, cols - 1) + colWidth(this.matrix, cols - 1);
            double heigth = matrix.GetLength(1) * 100 - 50;
            return new Size(width, heigth);
        }

        private string[,] get2dMatrix(string matrix)
        {
            var re = new Regex(@"\[[-a-zA-Z()_0-9]+(,[-a-zA-Z()_0-9]+)*\]");
            String[] columns = re.Matches(matrix)
                .OfType<Match>()
                .Select(m => m.Groups[0].Value)
                .ToArray();

            int height = 0, width = 0;
            width = columns.Length;
            foreach (String col in columns)
            {
                int colHeigth = col.Split(',').Length;
                if (colHeigth > height)
                    height = colHeigth;
            }

            String[,] ret = new String[width, height];

            for (int x = 0; x < width; x++ )
            {
                String[] rows = columns[x].Split(',');
                for (int y = 0; y < rows.Length; y++)
                {
                    String text = rows[y].Replace("[", "").Replace("]","");

                    ret[x, y] = text;
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
                    double textWidth = colWidth(matrix, x);
                    putText(position.X, position.Y, matrix[x, y], textWidth);
                }
            }
        }

        private double colWidth(String[,] matrix, int n)
        {
            int height = matrix.GetLength(1);

            double maxWidth = 0;

            for (int i = 0; i < height; i++)
            {
                double k = displaySize(matrix[n, i]).Width;
                if (maxWidth < k)
                    maxWidth = k;
            }

            return maxWidth;
        }

        private double colOffset(String[,] matrix, int n)
        {
            double offset = 0;
            for (int i = 0; i < n; i++)
            {
                offset += colWidth(matrix, i);
            }
            offset += n * 20;

            return offset;
        }

        private Point getPosition(int x, int y)
        {
            
            return new Point(colOffset(this.matrix, x), y*100);
        }

        private void drawConnections(string connections, string[,] matrix)
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


                drawConnection(int.Parse(nums[0]), int.Parse(nums[1]), int.Parse(nums[2]), int.Parse(nums[3]), matrix);

            }
        }

        private void drawConnection(int x1, int y1, int x2, int y2, String[,] matrix)
        {
            var off1 = displaySize(matrix[x1, y1]);
            off1.Width = colWidth(matrix, x1) - off1.Width / 2;
            var off2 = displaySize(matrix[x2, y2]);
            off2.Width = colWidth(matrix, x2) - off2.Width / 2;

            Point start = getPosition(x1, y1);
            Point end = getPosition(x2, y2);
            start.Offset(off1.Width, off1.Height / 2);
            end.Offset(off2.Width, off2.Height / 2);

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
    }
}
