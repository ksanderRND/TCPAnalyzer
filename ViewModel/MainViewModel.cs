using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using TCPAnalyzer.Adapters;
using TCPAnalyzer.Model;
using TCPAnalyzer.Service;

namespace TCPAnalyzer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly IFileParser _parser;
        private readonly IFileDialog _fileDialog;

        public ObservableCollection<MeasurementRowViewModel> Measurements { get; } = [];
        private IReadOnlyList<Measurement> _data = [];
        public MeasurementStats Stats { get; private set; } = new();

        private OxyChartModel? _chart;
        public OxyChartModel? Chart
        {
            get => _chart;
            private set
            {
                _chart = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasChart));
                OnPropertyChanged(nameof(ChartButtonLabel));
            }
        }
        public bool HasChart => Chart is not null;
        public string ChartButtonLabel => HasChart ? "Hide Chart" : "Show Chart";

        private string _loadedFileName = "No file loaded";
        public string LoadedFileName
        {
            get => _loadedFileName;
            set { _loadedFileName = value; OnPropertyChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public IReadOnlyList<OutlierThresholdMode> SigmaOptions { get; } =
            Enum.GetValues<OutlierThresholdMode>();

        private OutlierThresholdMode _thresholdMode = OutlierThresholdMode.TwoSigma;
        public OutlierThresholdMode ThresholdMode
        {
            get => _thresholdMode;
            set
            {
                _thresholdMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotCustomMode));
                if (_thresholdMode != OutlierThresholdMode.Custom)
                    CustomThreshold = ComputeSigmaThreshold(_thresholdMode);
            }
        }
        public bool IsNotCustomMode => ThresholdMode != OutlierThresholdMode.Custom;

        private double _customThreshold;
        public double CustomThreshold
        {
            get => _customThreshold;
            set
            {
                _customThreshold = value;
                OnPropertyChanged();
                ValidateCustomThreshold(value);
                if (!HasErrors)
                    RecomputeOutliers();
            }
        }

        private readonly Dictionary<string, string> _errors = [];

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
            => propertyName is not null && _errors.TryGetValue(propertyName, out var error)
                ? [error]
                : Array.Empty<string>();

        private void ValidateCustomThreshold(double value)
        {
            const string prop = nameof(CustomThreshold);
            if (!double.IsFinite(value) || value < 0)
                _errors[prop] = "Threshold must be a positive number.";
            else
                _errors.Remove(prop);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
            OnPropertyChanged(nameof(HasErrors));
        }

        public RelayCommand LoadCommand { get; }
        public RelayCommand ShowChartCommand { get; }

        public MainViewModel(IFileParser parser, IFileDialog fileDialog)
        {
            _parser = parser;
            _fileDialog = fileDialog;
            LoadCommand = new RelayCommand(OnLoad);
            ShowChartCommand = new RelayCommand(OnToggleChart, () => _data.Count > 0);
        }

        private void OnLoad()
        {
            var path = _fileDialog.OpenFile(
                "Select measurement file",
                "Measurement files (*.txt;*.csv)|*.txt;*.csv|Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*");

            if (path is null)
                return;

            ParseResult result;
            try
            {
                result = _parser.LoadFromFile(path);
            }
            catch (UnauthorizedAccessException)
            {
                StatusMessage = "Cannot read file: access denied.";
                return;
            }
            catch (IOException ex)
            {
                StatusMessage = $"Cannot read file: {ex.Message}";
                return;
            }

            LoadedFileName = Path.GetFileName(path);
            StatusMessage = BuildStatusMessage(result);

            _data = result.Measurements;
            Stats = StatisticsService.Calculate(_data);
            OnPropertyChanged(nameof(Stats));

            if (ThresholdMode != OutlierThresholdMode.Custom)
                CustomThreshold = ComputeSigmaThreshold(ThresholdMode);
            else
                RecomputeOutliers();

            ShowChartCommand.RaiseCanExecuteChanged();

            if (HasChart)
                Chart = _data.Count > 0 ? new OxyChartModel(_data) : null;
        }

        private static string BuildStatusMessage(ParseResult result)
        {
            if (result.Measurements.Count == 0 && result.SkippedLines.Count == 0)
                return "Empty file.";

            if (result.Measurements.Count == 0)
                return "No valid measurements found — unexpected file structure.";

            var parts = new List<string>();

            if (result.Measurements.Count == 1)
                parts.Add("Only 1 measurement loaded — statistics are not meaningful.");

            if (result.SkippedLines.Count > 0)
                parts.Add($"Skipped lines: {string.Join(", ", result.SkippedLines)}.");

            return string.Join(" ", parts);
        }

        private void RecomputeOutliers()
        {
            Measurements.Clear();
            foreach (var m in _data)
                Measurements.Add(new MeasurementRowViewModel(m, m.PositionalError > CustomThreshold));
        }

        private double ComputeSigmaThreshold(OutlierThresholdMode mode) => mode switch
        {
            OutlierThresholdMode.OneSigma   => Stats.MeanPE + Stats.StdDevPE,
            OutlierThresholdMode.TwoSigma   => Stats.MeanPE + 2.0 * Stats.StdDevPE,
            OutlierThresholdMode.ThreeSigma => Stats.MeanPE + 3.0 * Stats.StdDevPE,
            _ => CustomThreshold
        };

        private void OnToggleChart()
        {
            Chart = HasChart ? null : new OxyChartModel(_data);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public enum OutlierThresholdMode
    {
        OneSigma,
        TwoSigma,
        ThreeSigma,
        Custom
    }
}
