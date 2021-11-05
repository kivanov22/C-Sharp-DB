namespace MusicHub.Data.Models
{
    using MusicHub.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Album
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.ALBUM_NAME_MAX_LENGTH)]
        public string Name { get; set; }

        public DateTime ReleaseDate { get; set; }

        //TODO: calculated property
        public decimal Price { get; set; }

        public int? ProducerId { get; set; }

        public string Producer { get; set; }
        public virtual ICollection<Song> Songs { get; set; }
    }
}
