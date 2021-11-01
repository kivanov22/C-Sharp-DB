
namespace P03_FootballBetting.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    public class Color
    {
        [Key]
        public int ColorId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


    }
}
