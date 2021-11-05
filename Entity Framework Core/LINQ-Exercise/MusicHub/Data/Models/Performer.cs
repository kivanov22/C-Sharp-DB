namespace MusicHub.Data.Models
{
    using MusicHub.Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Performer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(ValidationConstants.PERFORMER_NAME_MAX_LENGTH)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(ValidationConstants.PERFORMER_NAME_MAX_LENGTH)]
        public string LastName { get; set; }

        public int Age { get; set; }

        public decimal NetWorth { get; set; }

        public virtual ICollection<Performer> SongPerformer { get; set; }
    }
}
