using BenchStoreBL.XMLData;

namespace BenchStoreBL.Services.XMLElementParsing
{
    public interface IXMLElementParser
    {
        TElement ParseXMLElement<TElement>(Stream stream)
            where TElement : IXMLElement;
    }
}

