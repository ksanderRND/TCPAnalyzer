using System.Windows;
using TCPAnalyzer.ViewModel;

namespace TCPAnalyzer.View
{
    public partial class MainView : Window
    {
        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
