using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{
    [XmlType("CategoryProduct")]
    public class ImportCategoryProductDto
    {
        [XmlElement("categoryId")]
        public int CategoryId { get; set; }

        [XmlElement("productId")]
        public int ProductId { get; set; }
    }
}
