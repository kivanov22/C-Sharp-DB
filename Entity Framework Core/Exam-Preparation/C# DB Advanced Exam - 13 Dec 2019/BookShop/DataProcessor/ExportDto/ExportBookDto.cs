using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BookShop.DataProcessor.ExportDto
{
    [XmlType("Book")]
   public class ExportBookDto
    {
        [XmlAttribute("Pages")]
        public int Pages { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Date { get; set; }
    }
}
