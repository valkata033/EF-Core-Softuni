namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //var result = ExportAlbumsInfo(context, 9);
            var result = ExportSongsAboveDuration(context, 4);

            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albums = context
                .Albums
                .Where(a => a.ProducerId.Value == producerId)
                .Include(a => a.Producer)
                .Include(a => a.Songs)
                .ThenInclude(s => s.Writer)
                .ToArray()
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate,
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs
                        .Select(s => new
                        {
                            SongName = s.Name,
                            Price = s.Price,
                            WriterName = s.Writer.Name
                        })
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.WriterName)
                        .ToArray(),
                    AlbumPrice = a.Price
                })
                .OrderByDescending(a => a.AlbumPrice)
                .ToArray();

            foreach (var a in albums)
            {
                sb
                    .AppendLine($"-AlbumName: {a.AlbumName}")
                    .AppendLine($"-ReleaseDate: {a.ReleaseDate.ToString("MM/dd/yyyy")}")
                    .AppendLine($"-ProducerName: {a.ProducerName}")
                    .AppendLine($"-Songs:");

                int counter = 1;
                foreach (var s in a.Songs)
                {
                    sb
                        .AppendLine($"---#{counter++}")
                        .AppendLine($"---SongName: {s.SongName}")
                        .AppendLine($"---Price: {s.Price:f2}")
                        .AppendLine($"---Writer: {s.WriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {a.AlbumPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context
                .Songs
                .Include(s => s.SongPerformers)
                .ThenInclude(s => s.Performer)
                .Include(s => s.Writer)
                .Include(s => s.Album)
                .ThenInclude(s => s.Producer)
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    PerformerFullName = s.SongPerformers
                        .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                        .FirstOrDefault(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ThenBy(s => s.PerformerFullName)
                .ToArray();

            int counter = 1;
            foreach (var s in songs)
            {
                sb
                    .AppendLine($"-Song #{counter++}")
                    .AppendLine($"---SongName: {s.Name}")
                    .AppendLine($"---Writer: {s.WriterName}")
                    .AppendLine($"---Performer: {s.PerformerFullName}")
                    .AppendLine($"---AlbumProducer: {s.AlbumProducer}")
                    .AppendLine($"---Duration: {s.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
