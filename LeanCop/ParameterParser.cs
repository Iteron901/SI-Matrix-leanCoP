using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeanCop
{
    class ParameterParser
    {
        static public String[,] parseMatrix(String s)
        {
            var re = new Regex(@"\[[-a-zA-Z()_0-9]+(,[-a-zA-Z()_0-9]+)*\]");
                String[] columns = re.Matches(s)
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

                for (int x = 0; x < width; x++)
                {
                    String[] rows = columns[x].Split(',');
                    for (int y = 0; y < rows.Length; y++)
                    {
                        String text = rows[y].Replace("[", "").Replace("]", "");

                        ret[x, y] = text;
                    }

                }

                return ret;
        }

        static public List<Connection> parseConnections(String s)
        {
            List<Connection> ret = new List<Connection>();

            var re = new Regex(@"\([0-9]+,[0-9]+\),\([0-9]+,[0-9]+\)");
            String[] pairs = re.Matches(s)
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

                Connection c = new Connection(
                    int.Parse(nums[0]),
                    int.Parse(nums[1]),
                    int.Parse(nums[2]),
                    int.Parse(nums[3])
                );
                ret.Add(c);
            }


            return ret;
        }
    }
}
