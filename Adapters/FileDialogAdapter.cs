using Microsoft.Win32;

namespace TCPAnalyzer.Adapters
{
    public class FileDialogAdapter : IFileDialog
    {
        public string? OpenFile(string title, string filter)
        {
            var dialog = new OpenFileDialog { Title = title, Filter = filter };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}
