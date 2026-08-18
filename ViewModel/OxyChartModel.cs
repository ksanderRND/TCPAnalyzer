using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using TCPAnalyzer.Model;

namespace TCPAnalyzer.ViewModel
{
    public class OxyChartModel
    {
        public PlotModel PlotModel { get; }

        public OxyChartModel(IReadOnlyList<Measurement> data)
        {
            PlotModel = new PlotModel
            {
                Title = "TCP Position Error over Repetitions",
                TitleFontSize = 13
            };
            PlotModel.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.BottomCenter,
                LegendPlacement = LegendPlacement.Outside,
                LegendOrientation = LegendOrientation.Horizontal
            });

            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Repetition #",
                MinorStep = 1,
                MajorStep = 1
            });

            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Value (mm)"
            });

            AddSeries(data, "X", m => m.X);
            AddSeries(data, "Y", m => m.Y);
            AddSeries(data, "Z", m => m.Z);
            AddSeries(data, "PE", m => m.PositionalError);
        }

        private void AddSeries(IReadOnlyList<Measurement> data, string title, Func<Measurement, double> selector)
        {
            var series = new LineSeries
            {
                Title = title,
                MarkerType = MarkerType.Circle,
                MarkerSize = 3
            };

            foreach (var m in data)
                series.Points.Add(new DataPoint(m.Id, selector(m)));

            PlotModel.Series.Add(series);
        }
    }
}
