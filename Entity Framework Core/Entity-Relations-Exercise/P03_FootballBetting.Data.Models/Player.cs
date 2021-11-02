
namespace P03_FootballBetting.Data.Models
{
using System.ComponentModel.DataAnnotations;
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }

        [Required]
        [MaxLength(75)]
        public string Name { get; set; }

        public int SquadNumber { get; set; }

        /// <summary>
        /// Navigational properties
        /// </summary>
        public int TeamId { get; set; }

        public int PositionId { get; set; }

        public bool IsInjured { get; set; }

    }
}
