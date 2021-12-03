namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var games = context.Genres
                .ToArray()
                .Where(x => genreNames.Contains(x.Name))
                .Select(x => new
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games.Select(g => new
                    {
                        Id = g.Id,
                        Title = g.Name,
                        Developer = g.Developer.Name,
                        Tags = string.Join(", ", g.GameTags.Select(t => t.Tag.Name)),
                        Players = g.Purchases.Count(),
                    })
                     .Where(g => g.Players > 0)
                    .OrderByDescending(g => g.Players)
                    .ThenBy(g => g.Id),
                    TotalPlayers = x.Games.Sum(g => g.Purchases.Count()),
                })
                .OrderByDescending(x => x.TotalPlayers)
                .ThenBy(x => x.Id);


            string json = JsonConvert.SerializeObject(games, Formatting.Indented);

            return json;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            var users = context.Users
                .ToArray()
                .Where(x => x.Cards.Any(x => x.Purchases.Any(p => p.Type.ToString() == storeType)))
                .Select(x => new ExportUser
                {
                    Username = x.Username,
                    TotalSpent = x.Cards.Sum(x => x.Purchases.Where(x => x.Type.ToString() == storeType)
                    .Sum(p => p.Game.Price)),

                    Purchases = x.Cards.SelectMany(x => x.Purchases)
                    .Where(x => x.Type.ToString() == storeType)
                    .Select(p => new ExportPurchase
                    {
                        Card = p.Card.Number,
                        Cvc = p.Card.Cvc,
                        Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new ExportGame
                        {
                            Title = p.Game.Name,
                            Price = p.Game.Price,
                            Genre = p.Game.Genre.Name
                        }
                    })
                    .OrderBy(x => x.Date)
                    .ToArray()
                })
                .OrderByDescending(x => x.TotalSpent)
                .ThenBy(x => x.Username)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUser[]), 
                new XmlRootAttribute("Users"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter sw = new StringWriter();

            xmlSerializer.Serialize(sw,users,namespaces);

            return sw.ToString();
        }
    }
}