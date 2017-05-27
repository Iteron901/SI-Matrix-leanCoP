using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace LeanCop
{
    class DrawingManager
    {
        private Canvas canvas;

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
                    putText(x * 150.0, y * 150.0, matrix[x, y]);
                }
            }
        }

        private void drawConnections(string connections)
        {
            
        }

        private void putText(double x, double y, string text)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            tb.FontSize = 40;
            Canvas.SetLeft(tb, x);
            Canvas.SetTop(tb, y);
            canvas.Children.Add(tb);
        }
    }
}
