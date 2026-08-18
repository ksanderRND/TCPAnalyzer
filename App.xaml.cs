using System.Windows;
using TCPAnalyzer.Adapters;
using TCPAnalyzer.Service;
using TCPAnalyzer.View;
using TCPAnalyzer.ViewModel;

namespace TCPAnalyzer
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var viewModel = new MainViewModel(
                new MeasurementFileParser(),
                new FileDialogAdapter());
            new MainView(viewModel).Show();
        }
    }
}
