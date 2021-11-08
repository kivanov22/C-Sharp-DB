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
            //string date = "R";
            //int num = 12;
            string result = GetMostRecentBooks(db);

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

        //10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(e => e.Author.LastName.StartsWith(input))
                .OrderBy(a => a.BookId)
                .Select(u => new
                {
                    BookName = u.Title,
                    u.Author.FirstName,
                    u.Author.LastName
                })
                .ToArray();
                


            foreach (var a in books)
            {
                sb.AppendLine($"{a.BookName} ({a.FirstName} {a.LastName})");
            }

            return sb.ToString().Trim();
        }

        //11. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
      
             int  result = context.Books
                .Where(e => e.Title.Length > lengthCheck).Count();

            return result;
        }

        //12. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var booksCopies = context.Authors
                .Select(e => new
                {
                    AuthorName = e.FirstName + " " + e.LastName,
                    Copies = e.Books.Where(a => a.AuthorId == a.Author.AuthorId)
                   .Select(d => d.Copies).Sum()

                })
                .OrderByDescending(d=>d.Copies)
               .ToArray();


            foreach (var a in booksCopies)
            {
                sb.AppendLine($"{a.AuthorName} - {a.Copies}");
            }

            return sb.ToString().Trim();

        }

        //13. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var booksProfit = context.Categories
                .Select(e => new
                {
                    CategoryName = e.Name,
                    Profit = e.CategoryBooks.Where(e=>e.CategoryId == e.Category.CategoryId)
                   .Select(d => d.Book.Copies * d.Book.Price).Sum()

                })
                .OrderByDescending(d => d.Profit)
                .ThenBy(c=>c.CategoryName)
               .ToArray();


            foreach (var a in booksProfit)
            {
                sb.AppendLine($"{a.CategoryName} - ${a.Profit:f2}");
            }

            return sb.ToString().Trim();
        }

        //14. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var recentBooks = context.Categories
                .OrderBy(n=>n.Name)
                .Select(e => new
                {
                    CategoryName =e.Name,
                    Books = e.CategoryBooks.Select(b => new
                    {
                        BookTitle = b.Book.Title,
                        BookRelase = b.Book.ReleaseDate,
                        BookRelaseDate = b.Book.ReleaseDate.Value.Year,

                    })
                .OrderByDescending(d => d.BookRelase)
                .Take(3)
               .ToArray()
                })
                .ToArray();


            foreach (var c in recentBooks)
            {
                sb.AppendLine($"--{c.CategoryName}");

                foreach (var b in c.Books)
                {
                    sb.AppendLine($"{b.BookTitle} ({b.BookRelaseDate})");
                }
            }

            return sb.ToString().Trim();
        }

        //15. Increase Prices

        //16. Remove Books

    }
}
