using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.GameAssistant.Scripts
{
    /// <summary>
    /// Reads comma-separated values (CSV) from table (Google Docs supported)
    /// </summary>
    public class CsvReader
    {
        /// <summary>
        /// Structure of table values that can be accessed by two indexes [i][j]
        /// </summary>
        public readonly List<List<string>> Rows = new List<List<string>>();

        /// <summary>
        /// Returns values as flat structure (headers are generally located in the first row)
        /// </summary>
        public CsvReader(string csvSheet)
        {
            var matches = Regex.Matches(csvSheet, "\".+?\"", RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                csvSheet = csvSheet.Replace(match.Value, match.Value.Replace("\"", null).Replace(",", "[comma]").Replace(Environment.NewLine, "[new_line]"));
            }

            var rows = csvSheet.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var row in rows)
            {
                var cells = row.Split(',').Select(j => j.Replace("[comma]", ",").Replace("[new_line]", Environment.NewLine)).ToList();

                Rows.Add(cells);
            }
        }
    }
}