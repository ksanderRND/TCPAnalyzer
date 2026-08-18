namespace TCPAnalyzer.Service
{
    public interface IFileParser
    {
        ParseResult LoadFromFile(string path);
    }
}
