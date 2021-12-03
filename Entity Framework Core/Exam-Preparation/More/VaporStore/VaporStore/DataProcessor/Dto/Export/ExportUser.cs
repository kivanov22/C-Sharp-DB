using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class ExportUser
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        public ExportPurchase[] Purchases { get; set; }

        public decimal TotalSpent { get; set; }

    }
}
