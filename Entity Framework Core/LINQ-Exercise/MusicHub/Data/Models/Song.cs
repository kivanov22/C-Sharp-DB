namespace MusicHub.Data.Models
{
    using MusicHub.Common;
    using MusicHub.Data.Models.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.SONG_NAME_MAX_LENGTH)]
        public string Name { get; set; }

        public TimeSpan Duration { get; set; }//by default required

        public DateTime CreatedOn { get; set; }//by default required

        public  Genre Genre { get; set; }
        public int? AlbumId { get; set; }//nullable ?
        public int WriterId { get; set; }

        public decimal Price { get; set; }

    }
}
