
namespace P03_FootballBetting.Data.Models
{
using System.ComponentModel.DataAnnotations;
    public class Town
    {
        [Key]
        public int TownId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public int CountryId { get; set; }
    }
}
