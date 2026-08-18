namespace TCPAnalyzer.Model
{
    public class Measurement
    {
        public int Id { get; init; }
        public double X { get; init; }
        public double Y { get; init; }
        public double Z { get; init; }
        public double PositionalError => Math.Sqrt(X * X + Y * Y + Z * Z);
    }
}
