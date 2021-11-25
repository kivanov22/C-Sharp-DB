using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("Game")]
    public class GameXmlExportModel
    {
        public string Genre { get; set; }

        public decimal Price { get; set; }
    }
}
