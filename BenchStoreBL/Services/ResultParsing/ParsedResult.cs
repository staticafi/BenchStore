using BenchStoreBL.Models;

namespace BenchStoreBL.Services.ResultParsing
{
    public class ParsedResult
    {
        public required Result Result { get; set; }
        public required string ResultFilePath { get; set; }
    }
}

