using System.Globalization;
using System.IO;
using TCPAnalyzer.Model;

namespace TCPAnalyzer.Service
{
    public class MeasurementFileParser : IFileParser
    {
        public ParseResult LoadFromFile(string path)
        {
            var measurements = new List<Measurement>();
            var skippedLines = new List<int>();
            int measurementId = 1;

            foreach (var (line, lineNumber) in File.ReadLines(path).Select((l, i) => (l, i + 1)))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var measurement = ParseLine(line, measurementId);
                if (measurement is not null)
                {
                    measurements.Add(measurement);
                    measurementId++;
                }
                else
                {
                    skippedLines.Add(lineNumber);
                }
            }

            return new ParseResult(measurements, skippedLines);
        }

        private static Measurement? ParseLine(string line, int id)
        {
            var parts = line.Split(',');
            if (parts.Length < 3)
                return null;

            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y) ||
                !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double z))
                return null;

            return new Measurement { Id = id, X = x, Y = y, Z = z };
        }
    }

    public record ParseResult(IReadOnlyList<Measurement> Measurements, IReadOnlyList<int> SkippedLines);
}
