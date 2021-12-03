namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,//?? $"{a.FirstName} {a.LastName}
                    Books = a.AuthorsBooks
                    .Select(x => x.Book)//without this
                    .OrderByDescending(x => x.Price)
                    .Select(b => new
                    {
                        BookName = b.Name,
                        BookPrice = b.Price.ToString("f2")
                    })
                    .ToArray()
                })
                .ToArray()
                .OrderByDescending(x => x.Books.Length)
                .ThenBy(x => x.AuthorName)
                .ToArray();



            return JsonConvert.SerializeObject(authors, Formatting.Indented);
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context.Books
                .ToArray()
                .Where(x => x.PublishedOn < date && x.Genre==Genre.Science)
                .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Take(10)
                .Select(x => new ExportBookDto
                {
                    Pages = x.Pages,
                    Name = x.Name,
                    Date = x.PublishedOn.ToString("d",CultureInfo.InvariantCulture)
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportBookDto[]), new XmlRootAttribute("Books"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

             StringWriter sw = new StringWriter(sb);

            xmlSerializer.Serialize(sw, books, namespaces);

            return sb.ToString().Trim();
        }
    }
}