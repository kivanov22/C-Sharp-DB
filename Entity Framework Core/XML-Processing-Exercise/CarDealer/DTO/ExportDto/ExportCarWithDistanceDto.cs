using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDto
{
    [XmlType("car")]
    public class ExportCarWithDistanceDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("travelled-distance")]
        public string TravelledDistance { get; set; }
    }
}
