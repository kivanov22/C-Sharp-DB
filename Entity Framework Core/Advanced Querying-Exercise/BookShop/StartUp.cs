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
            string date = "sK";
            string result = GetBookTitlesContaining(db, date);

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

        //5. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(i => i.BookId)
                .Select(b => b.Title)
                .ToArray();

            foreach (var b in books)
            {
                sb.AppendLine(b);
            }
            return sb.ToString().Trim();
        }

        //6. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            string[] formatedCategories = input.Split(" ",StringSplitOptions.RemoveEmptyEntries);


            var books = context.BooksCategories
                .Select(b => new
                {
                    BookTitle =b.Book.Title,
                    CategoryName = b.Category.Name
                })
                .OrderBy(b => b.BookTitle)
                .ToArray();
               

            foreach (var b in books)
            {
                if (formatedCategories.Contains(b.CategoryName.ToLower()))
                {
                sb.AppendLine(b.BookTitle);
                }
            }
            return sb.ToString().Trim();
        }

        //7. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            DateTime givenDate = DateTime.Parse(date);

            var books = context.Books
                 .OrderByDescending(d => d.ReleaseDate)
                .Where(b => b.ReleaseDate < givenDate)
                .Select(e => new
                {
                    BookName = e.Title,
                    EditionType = e.EditionType,
                    Price = e.Price
                })
                .ToArray();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.BookName} - {b.EditionType} - ${b.Price}");
            }

            return sb.ToString().Trim();
               
        }

        //8. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Authors
                .Where(e => e.FirstName.EndsWith(input))
                .OrderBy(e => e.FirstName)
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName
                })
                .ToArray();


            foreach (var a in authors)
            {
                sb.AppendLine($"{a.FirstName} {a.LastName}");
            }

            return sb.ToString().Trim();
        }

        //9. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b=>b.Title.Contains(input))
                .OrderBy(e=>e.Title)
                .Select(e=>new { BookName = e.Title })
                .ToArray();


            foreach (var a in books)
            {
                sb.AppendLine($"{a.BookName}");
            }

            return sb.ToString().Trim();
        }
    }
}
