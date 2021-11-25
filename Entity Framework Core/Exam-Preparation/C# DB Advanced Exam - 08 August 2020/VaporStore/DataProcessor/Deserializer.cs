namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var output = new StringBuilder();

            var games = JsonConvert.
                DeserializeObject<IEnumerable<GameJsonImportModel>>(jsonString);

            foreach (var jsonGame in games)
            {
                if (!IsValid(jsonGame) || jsonGame.Tags.Count() == 0 || jsonGame.Price < 0)
                {
                    output.AppendLine("Invalid Data");
                    continue;
                }

                var genre = context.Genres.FirstOrDefault(x => x.Name == jsonGame.Genre);

                //if null it will create a new developer. Search in db or create it
                var developer = context.Developers.FirstOrDefault(x => x.Name == jsonGame.Developer)
                    ?? new Developer { Name = jsonGame.Developer };

                if (genre == null)
                {
                    genre = new Genre { Name = jsonGame.Genre };
                }

                var game = new Game
                {
                    Name = jsonGame.Name,
                    Genre = genre,
                    Developer = developer,
                    Price = jsonGame.Price,
                    ReleaseDate = jsonGame.ReleaseDate.Value,

                };

                foreach (var jsonTag in jsonGame.Tags)
                {
                    var tag = context.Tags.FirstOrDefault(x => x.Name == jsonTag) ?? new Tag { Name = jsonTag };
                    game.GameTags.Add(new GameTag { Tag = tag });
                }

                context.Games.Add(game);
                context.SaveChanges();
                output.AppendLine($"Added {game.Name} ({jsonGame.Genre}) with {jsonGame.Tags.Count()} tags");
            }


            return output.ToString();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var output = new StringBuilder();

            var users = JsonConvert.
                DeserializeObject<IEnumerable<UserJsonImportModel>>(jsonString);

            foreach (var jsonUser in users)
            {
                if (!IsValid(jsonUser))
                {
                    output.AppendLine("Invalid Data");
                    continue;
                }

                //var fullName = context.Users.FirstOrDefault(x => x.FullName == jsonUser.FullName) ?? new User { FullName=jsonUser.FullName};
                //var email = context.Users.FirstOrDefault(x => x.Email == jsonUser.Email) ?? new User { Email = jsonUser.Email };

                var user = new User
                {
                    Age = jsonUser.Age,
                    Email = jsonUser.Email,
                    FullName = jsonUser.FullName,
                    Username = jsonUser.Username,
                    Cards = jsonUser.Cards.Select(x => new Card
                    {
                        Cvc = x.Cvc,
                        Number = x.Number,
                        Type= x.Type,
                    }).ToList()
                };
                context.Users.Add(user);
                context.SaveChanges();
                output.AppendLine($"Imported {jsonUser.Username} with {jsonUser.Cards.Count()} cards");
            }

            return output.ToString();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var output = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PurchaseXmlInputModel[]),
                new XmlRootAttribute("Purchases"));

            var purchases = (PurchaseXmlInputModel[])xmlSerializer.Deserialize(
                new StringReader(xmlString));

            foreach (var xmlPurchase in purchases)
            {
                if (!IsValid(xmlPurchase))
                {
                    output.AppendLine("Invalid Data");
                    continue;
                }

                bool parsedDate = DateTime.TryParseExact(
                    xmlPurchase.Date, "dd/MM/yyyy HH:mm",CultureInfo.InvariantCulture,DateTimeStyles.None, out var date);

                if (!parsedDate)
                {
                    output.AppendLine("Invalid Data");
                    continue;
                }

                var purchase = new Purchase
                {
                    Date = date,
                    Type = xmlPurchase.Type.Value,
                    ProductKey = xmlPurchase.Key,
                };

                purchase.Card = context.Cards.FirstOrDefault(x => x.Number == xmlPurchase.Card);
                purchase.Game = context.Games.FirstOrDefault(x => x.Name == xmlPurchase.Title);

                context.Purchases.Add(purchase);

                var username = context.Users.Where(x => x.Id == purchase.Card.UserId).Select(x => x.Username).FirstOrDefault();
                output.AppendLine($"Imported {xmlPurchase.Title} for {username}");
            }
            context.SaveChanges();
            return output.ToString();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}