namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            string result = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();
            //order by before making it to string
            var producerAlbums = context
                .Albums
                .ToArray()//translate so that db knows materialize
                .Where(p => p.ProducerId == producerId)
                .OrderByDescending(d => d.Price)
                .Select(e => new
                {
                    AlbumName = e.Name,
                    ReleaseDate = e.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = e.Producer.Name,
                    AlbumSongs = e.Songs
                    .ToArray()//translate so that db knows
                    .Select(s => new
                    {
                        SongName = s.Name,
                        Price = s.Price.ToString("f2"),
                        Writer = s.Writer.Name
                    })
                .OrderByDescending(d => d.SongName)
                .ThenBy(s => s.Writer)
                .ToArray(),
                    TotalAlbumPrice = e.Price.ToString("f2")
                })
                .ToArray();

            foreach (var album in producerAlbums)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                int i = 1;
                foreach (var s in album.AlbumSongs)
                {
                    sb.AppendLine($"---#{i++}");
                    sb.AppendLine($"---SongName: {s.SongName}");
                    sb.AppendLine($"---Price: {s.Price}");
                    sb.AppendLine($"---Writer: {s.Writer}");

                }
                sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice}");

            }

            return sb.ToString().Trim();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songDuration = context
                .Songs
                .ToArray()
                .Where(t => t.Duration.TotalSeconds > duration)
                .Select(e => new
                {
                    SongName = e.Name,
                    PerformerFullName = e.SongPerformers
                    .ToArray()
                    .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                    .FirstOrDefault(),
                    WriterName = e.Writer.Name,
                    AlbumProducer = e.Album.Producer.Name,
                    Duration = e.Duration.ToString("c",CultureInfo.InvariantCulture),
                })
                .OrderBy(e => e.SongName)
                .ThenBy(w => w.WriterName)
                .ThenBy(p => p.PerformerFullName)
                .ToArray();

            int i = 1;
            foreach (var s in songDuration)
            {
                sb.AppendLine($"-Song #{i++}");
                sb.AppendLine($"---SongName: {s.SongName}");
                sb.AppendLine($"---Writer: {s.WriterName}");
                sb.AppendLine($"---Performer: {s.PerformerFullName}");
                sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}");
                sb.AppendLine($"---Duration: {s.Duration}");


            }

            return sb.ToString().Trim();
        }
    }
}
