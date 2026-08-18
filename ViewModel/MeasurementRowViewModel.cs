using TCPAnalyzer.Model;

namespace TCPAnalyzer.ViewModel
{
    public class MeasurementRowViewModel(Measurement measurement, bool isOutlier)
    {
        public int Id => measurement.Id;
        public double X => measurement.X;
        public double Y => measurement.Y;
        public double Z => measurement.Z;
        public double PositionalError => measurement.PositionalError;
        public bool IsOutlier { get; } = isOutlier;
    }
}
