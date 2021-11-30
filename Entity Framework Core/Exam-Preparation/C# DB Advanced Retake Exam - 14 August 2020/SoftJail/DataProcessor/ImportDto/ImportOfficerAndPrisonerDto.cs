using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficerAndPrisonerDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        [XmlElement("Name")]
        public string Name { get; set; }

        //[Required]//??
        [XmlElement("Money")]
        [Range(typeof(decimal),"0.0", "79228162514264337593543950335")]
        public decimal Money { get; set; }

        //[Required]//??
        [XmlElement("Position")]
        public string Position { get; set; }

        //[Required]//??
        [XmlElement("Weapon")]
        public string Weapon { get; set; }

        [XmlElement("DepartmentId")]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public  ImportPrisonerDto[] Prisoners { get; set; }
    }
}
