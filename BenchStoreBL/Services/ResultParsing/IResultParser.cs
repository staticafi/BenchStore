namespace BenchStoreBL.Services.ResultParsing
{
    public interface IResultParser
    {
        Task<ParsedResult> ParseCompressedResult(Stream resultStream);
        Task<ParsedResult> ParseResult(Stream resultStream);
    }
}

