namespace DocuMind.Application.Common
{
    public interface ITextSplitterService
    {
        IEnumerable<string> SplitText(string text, int chunkSize, int chunkOverlap);
    }
}