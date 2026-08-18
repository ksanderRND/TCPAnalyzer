using TCPAnalyzer.Model;

namespace TCPAnalyzer.Service
{
    public static class StatisticsService
    {
        public static MeasurementStats Calculate(IReadOnlyList<Measurement> data)
        {
            if (data.Count == 0)
                return new MeasurementStats();

            var xs = data.Select(m => m.X).ToList();
            var ys = data.Select(m => m.Y).ToList();
            var zs = data.Select(m => m.Z).ToList();
            var pes = data.Select(m => m.PositionalError).ToList();

            double meanX = xs.Average();
            double meanY = ys.Average();
            double meanZ = zs.Average();
            double meanPE = pes.Average();

            return new MeasurementStats
            {
                MeanX = meanX,
                MeanY = meanY,
                MeanZ = meanZ,
                MeanPE = meanPE,
                StdDevX = StdDev(xs,  meanX),
                StdDevY = StdDev(ys,  meanY),
                StdDevZ = StdDev(zs,  meanZ),
                StdDevPE = StdDev(pes, meanPE),
                MaxPE = pes.Max(),
                MinPE = pes.Min()
            };
        }

        private static double StdDev(List<double> values, double mean)
        {
            if (values.Count < 2) return 0;
            double sumSquares = values.Sum(v => (v - mean) * (v - mean));
            return Math.Sqrt(sumSquares / (values.Count-1));
        }
    }
}
