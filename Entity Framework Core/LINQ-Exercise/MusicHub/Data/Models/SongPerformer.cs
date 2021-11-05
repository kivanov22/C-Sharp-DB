using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models
{
    public class SongPerformer
    {
        [Key]
        public int SongId { get; set; }

        public int PerformerId { get; set; }


    }
}
