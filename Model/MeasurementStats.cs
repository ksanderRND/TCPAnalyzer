namespace TCPAnalyzer.Model
{
    public class MeasurementStats
    {
        public double MeanX { get; init; }
        public double MeanY { get; init; }
        public double MeanZ { get; init; }
        public double StdDevX { get; init; }
        public double StdDevY { get; init; }
        public double StdDevZ { get; init; }

        public double MeanPE { get; init; }
        public double StdDevPE { get; init; }
        public double MaxPE { get; init; }
        public double MinPE { get; init; }
        public double PERange => MaxPE - MinPE;
    }
}
