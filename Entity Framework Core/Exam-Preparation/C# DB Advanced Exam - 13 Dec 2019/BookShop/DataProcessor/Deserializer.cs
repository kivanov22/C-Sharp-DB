namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportBooksDto[]),
                new XmlRootAttribute("Books"));

            var booksDtos = (ImportBooksDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            foreach (var bookDto in booksDtos)
            {
                if (!IsValid(bookDto))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                bool isPublishDateValid = DateTime
                    .TryParseExact(bookDto.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime publishedOn);

                if (!isPublishDateValid)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var books = new Book()
                {
                    Name = bookDto.BookName,
                    Genre = (Genre)bookDto.Genre,
                    Price = bookDto.Price,
                    Pages = bookDto.Pages,
                    PublishedOn = publishedOn
                };
                context.Books.Add(books);
                sb.AppendLine($"Successfully imported book {books.Name} for {books.Price:F2}.");
            }
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportAuthorsDto[] authorsDtos = JsonConvert.DeserializeObject<ImportAuthorsDto[]>(jsonString);

            List<Author> authors = new List<Author>();

            foreach (var authorDto in authorsDtos)
            {
                if (!IsValid(authorDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var a = new Author
                {
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Phone = authorDto.Phone,
                    Email = authorDto.Email
                };

                bool doesEmailExist = authors.FirstOrDefault(x => x.Email == authorDto.Email) != null;

                if (doesEmailExist)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                foreach (var bookDto in authorDto.Books.Distinct())
                {
                    var findBook = context.Books.Find(bookDto.Id);

                    if (findBook == null)
                    {
                        continue;
                    }

                    a.AuthorsBooks.Add(new AuthorBook
                    {
                        Author = a,
                        Book = findBook,
                    });
                }

                if (a.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                authors.Add(a);
                sb.AppendLine($"Successfully imported author - {a.FirstName + " " + a.LastName} with {a.AuthorsBooks.Count} books.");
            }
            context.Authors.AddRange(authors);
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