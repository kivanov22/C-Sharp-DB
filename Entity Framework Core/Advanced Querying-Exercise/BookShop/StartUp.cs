namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            //string ageRestriction = Console.ReadLine();

            string result = GetBooksByPrice(db);

            Console.WriteLine(result);
        }

        //2. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            //parse a string checks case
            AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            string[] bookTitle = context.Books
                .Where(a => a.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            foreach (var t in bookTitle)
            {
                sb.AppendLine(t);
            }

            return sb.ToString().Trim();
        }

        //3. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            string goldenEdition = "Gold";

            EditionType editionType = Enum.Parse<EditionType>(goldenEdition);

            var goldenEditionTitle = context.Books
                .Where(c => c.Copies < 5000 && c.EditionType == editionType)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            foreach (var b in goldenEditionTitle)
            {
                sb.AppendLine(b);
            }

            return sb.ToString().Trim();
        }

        //4. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(p => p.Price > 40)
                .OrderByDescending(p => p.Price)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .ToArray();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.Price}");
            }

            return sb.ToString().Trim();
        }

    }
}
