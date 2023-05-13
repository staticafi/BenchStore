using System.Xml.Serialization;

namespace BenchStoreBL.XMLData
{
    [XmlRoot(ElementName = "result")]
    public class XMLResultElement : IXMLElement
    {
        /// <summary>
        /// Implied
        /// </summary>
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [XmlAttribute("benchmarkname")]
        public string? BenchmarkName { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [XmlAttribute("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [XmlAttribute("block")]
        public string? Block { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [XmlAttribute("date")]
        public string? Date { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [XmlAttribute("starttime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [XmlAttribute("endtime")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [XmlAttribute("tool")]
        public string? Tool { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [XmlAttribute("toolmodule")]
        public string? ToolModule { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [XmlAttribute("version")]
        public string? Version { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [XmlAttribute("options")]
        // TODO: string?
        public string? Options { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [XmlAttribute("memlimit")]
        public string? MemLimit { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [XmlAttribute("timelimit")]
        public string? TimeLimit { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [XmlAttribute("cpuCores")]
        public uint CPUCores { get; set; }

        /// <summary>
        /// Required
        /// </summary> 
        [XmlAttribute("generator")]
        public string? Generator { get; set; }

        /// <summary>
        /// Implied
        /// </summary>
        [XmlAttribute("error")]
        public string? Error { get; set; }
    }
}

