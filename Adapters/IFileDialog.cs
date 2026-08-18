namespace TCPAnalyzer.Adapters
{
    public interface IFileDialog
    {
        string? OpenFile(string title, string filter);
    }
}
