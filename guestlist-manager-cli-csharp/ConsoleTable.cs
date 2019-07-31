//Ref: https://stackoverflow.com/questions/856845/how-to-best-way-to-draw-table-in-console-app-c

using System.Collections.Generic;
using System.Linq;

namespace guestlist_manager_cli_csharp
{
    public class ConsoleTable
    {
        private readonly string[] titles;
        private readonly List<int> lengths;
        private readonly List<string[]> rows = new List<string[]>();

        public ConsoleTable(params string[] titles)
        {
            this.titles = titles;
            lengths = titles.Select(t => t.Length).ToList();
        }

        /// <summary>
        /// Add row to table with consideration of its content length
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(params object[] row)
        {
            if (row.Length != titles.Length)
            {
                throw new System.Exception($"Added row length [{row.Length}] is not equal to title row length [{titles.Length}]");
            }
            rows.Add(row.Select(o => o.ToString()).ToArray());
            for (int i = 0; i < titles.Length; i++)
            {
                if (rows.Last()[i].Length > lengths[i])
                {
                    lengths[i] = rows.Last()[i].Length;
                }
            }
        }


        /// <summary>
        /// Display pretty table on the screen
        /// </summary>
        public void Print()
        {
            lengths.ForEach(l => System.Console.Write("+-" + new string('-', l) + '-'));
            System.Console.WriteLine("+");

            string line = "";
            for (int i = 0; i < titles.Length; i++)
            {
                line += "| " + titles[i].PadRight(lengths[i]) + ' ';
            }
            System.Console.WriteLine(line + "|");

            lengths.ForEach(l => System.Console.Write("+-" + new string('-', l) + '-'));
            System.Console.WriteLine("+");

            foreach (var row in rows)
            {
                line = "";
                for (int i = 0; i < row.Length; i++)
                {
                    if (int.TryParse(row[i], out int n))
                    {
                        line += "| " + row[i].PadLeft(lengths[i]) + ' ';  // numbers are padded to the left
                    }
                    else
                    {
                        line += "| " + row[i].PadRight(lengths[i]) + ' ';
                    }
                }
                System.Console.WriteLine(line + "|");
            }

            lengths.ForEach(l => System.Console.Write("+-" + new string('-', l) + '-'));
            System.Console.WriteLine("+");
        }
    }
}
