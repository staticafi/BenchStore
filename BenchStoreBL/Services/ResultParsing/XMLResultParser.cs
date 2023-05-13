using BenchStoreBL.Models;
using BenchStoreBL.Models.Mappers;
using BenchStoreBL.Services.ResultStoring;
using BenchStoreBL.Services.XMLElementParsing;
using BenchStoreBL.XMLData;

using ICSharpCode.SharpZipLib.BZip2;

namespace BenchStoreBL.Services.ResultParsing
{
    internal class XMLResultParser : IResultParser
    {
        private readonly IXMLElementParser _xmlElementParser;
        private readonly IFileStoring _resultStorageService;

        public XMLResultParser(IXMLElementParser xmlElementParser, IFileStoring resultStorageService)
        {
            _xmlElementParser = xmlElementParser;
            _resultStorageService = resultStorageService;
        }

        public async Task<ParsedResult> ParseCompressedResult(Stream resultStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (BZip2InputStream bzipInputStream = new BZip2InputStream(resultStream))
                {
                    bzipInputStream.IsStreamOwner = false;
                    await bzipInputStream.CopyToAsync(memoryStream);
                }

                memoryStream.Position = 0;
                XMLResultElement xmlResultElement = _xmlElementParser.ParseXMLElement<XMLResultElement>(memoryStream);
                Result result = xmlResultElement.MapToModel();
                memoryStream.Position = 0;
                string tempFilePath = await _resultStorageService.StoreTemporaryFile(memoryStream);

                return new ParsedResult
                {
                    Result = result,
                    ResultFilePath = tempFilePath,
                };
            }
        }

        public async Task<ParsedResult> ParseResult(Stream resultStream)
        {
            XMLResultElement xmlResultElement = _xmlElementParser.ParseXMLElement<XMLResultElement>(resultStream);
            Result result = xmlResultElement.MapToModel();
            resultStream.Position = 0;
            string tempFilePath = await _resultStorageService.StoreTemporaryFile(resultStream);

            return new ParsedResult
            {
                Result = result,
                ResultFilePath = tempFilePath,
            };
        }
    }
}

