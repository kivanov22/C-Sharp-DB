using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDto
{
    [XmlType("customer")]
    public class ExportTotalSaleByCustomerDto
    {
        [XmlAttribute("full-name")]
        public string FullName { get; set; }

        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }

        [XmlAttribute("spent-money")]
        public decimal SpentMoney { get; set; }
    }
}
