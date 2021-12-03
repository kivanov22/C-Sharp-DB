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
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportGameDto[] gamesDtos = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            List<Game> games = new List<Game>();

            foreach (var gameDto in gamesDtos)
            {
                if (!IsValid(gameDto) || gameDto.Tags.Count() == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                bool isReleaseDateValid = DateTime
                    .TryParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime releaseDate);

                if (!isReleaseDateValid )
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                

                var developer = context.Developers.FirstOrDefault(x => x.Name == gameDto.Developer)
                    ?? new Developer { Name = gameDto.Developer };

                var genre = context.Genres.FirstOrDefault(x => x.Name == gameDto.Genre)
                    ?? new Genre { Name = gameDto.Genre };

                var g = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = releaseDate,
                    Developer = developer,
                    Genre = genre
                };

                foreach (var tagDto in gameDto.Tags)
                {
                    var tag = context.Tags.FirstOrDefault(x => x.Name == tagDto)
                        ?? new Tag { Name = tagDto };

                    g.GameTags.Add(new GameTag { Tag = tag });
                }

                games.Add(g);
                sb.AppendLine($"Added {gameDto.Name} ({gameDto.Genre}) with {gameDto.Tags.Count()} tags");
            }
            context.Games.AddRange(games);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportUserCardDto[] usersDtos = JsonConvert.DeserializeObject<ImportUserCardDto[]>(jsonString);

            List<User> users = new List<User>();


            foreach (var userDto in usersDtos)
            {
                if (!IsValid(userDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var u = new User
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age,
                };


                foreach (var cardDto in userDto.Cards)
                {
                    var c = new Card
                    {
                        Number = cardDto.Number,
                        Cvc = cardDto.Cvc,
                        Type = (CardType)cardDto.Type
                    };
                    u.Cards.Add(c);
                }

                users.Add(u);
                sb.AppendLine($"Imported {u.Username} with {u.Cards.Count} cards");
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPurchaseDto[]),
                new XmlRootAttribute("Purchases"));

            var purchasesDtos = (ImportPurchaseDto[])xmlSerializer.Deserialize(new StringReader(xmlString));


            foreach (var purchaseDto in purchasesDtos)
            {
                if (!IsValid(purchaseDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var isDateValid = DateTime
                    .TryParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

                var p = new Purchase
                {
                    Type = purchaseDto.Type.Value,
                    ProductKey = purchaseDto.Key,
                    Card = context.Cards.FirstOrDefault(x => x.Number == purchaseDto.Card),
                    Game = context.Games.FirstOrDefault(x => x.Name == purchaseDto.Title),
                    Date = date,
                };
                context.Purchases.Add(p);

                var username = context.Users.Where(x => x.Id == p.Card.UserId)
                    .Select(x => x.Username).FirstOrDefault();

                sb.AppendLine($"Imported {purchaseDto.Title} for {username}");
            }
            context.SaveChanges();


            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}