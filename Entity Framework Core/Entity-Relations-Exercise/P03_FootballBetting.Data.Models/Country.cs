
namespace P03_FootballBetting.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        [MaxLength(75)]
        public string Name { get; set; }
    }
}
