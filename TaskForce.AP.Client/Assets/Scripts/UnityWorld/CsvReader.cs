using System.Collections.Generic;
using System.Text;

namespace TaskForce.AP.Client.UnityWorld
{
    public class CsvReader
    {
        private readonly Core.ILogger _logger;

        public CsvReader(Core.ILogger logger)
        {
            _logger = logger;
        }

        public List<Dictionary<string, string>> ReadCsvData(string rawCsvData)
        {
            var result = new List<Dictionary<string, string>>();
            if (string.IsNullOrEmpty(rawCsvData))
            {
                _logger.Fatal("CSV data is empty.");
                return result;
            }

            var lines = SplitCsvLines(rawCsvData);
            if (lines.Count == 0)
            {
                _logger.Fatal("CSV has no header.");
                return result;
            }

            if (lines.Count == 1)
                return result;

            var headers = ParseCsvLine(lines[0]);
            for (int i = 1; i < lines.Count; i++)
            {
                var fields = ParseCsvLine(lines[i]);
                if (fields.Count != headers.Count)
                {
                    _logger.Warn($"Skipping line {i + 1}: Column count mismatch (Expected {headers.Count}, Actual {fields.Count})");
                    continue;
                }

                var row = new Dictionary<string, string>();
                for (int j = 0; j < headers.Count; j++)
                {
                    row[headers[j]] = fields[j];
                }
                result.Add(row);
            }

            return result;
        }

        private List<string> SplitCsvLines(string rawCsv)
        {
            var lines = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < rawCsv.Length; i++)
            {
                char c = rawCsv[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    sb.Append(c);
                }
                else if ((c == '\n' || c == '\r') && !inQuotes)
                {
                    if (sb.Length > 0)
                    {
                        lines.Add(sb.ToString().TrimEnd('\r'));
                        sb.Clear();
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
                lines.Add(sb.ToString());

            return lines;
        }

        private List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"'); // 이중 따옴표는 하나의 따옴표로 처리
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        fields.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            fields.Add(sb.ToString());
            return fields;
        }
    }
}
