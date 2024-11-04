using System.Xml;
using System.Xml.Serialization;

using BenchStoreBL.XMLData;

namespace BenchStoreBL.Services.XMLElementParsing
{
    internal class XMLElementParser : IXMLElementParser
    {
        public TElement ParseXMLElement<TElement>(Stream stream)
            where TElement : IXMLElement
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.ValidationType = ValidationType.DTD;
            settings.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.XmlResolver = new XmlUrlResolver();
            settings.ValidationEventHandler += (sender, args) =>
            {
                Console.WriteLine($"XML Parser: {args.Message}");
            };

            using XmlReader reader = XmlReader.Create(stream, settings);
            var xmlSerializer = new XmlSerializer(typeof(TElement));
            TElement? xmlElement = (TElement?)xmlSerializer.Deserialize(reader);

            if (xmlElement == null)
            {
                throw new ArgumentException("Failed to serialize result from XML!");
            }

            return xmlElement;
        }
    }
}

