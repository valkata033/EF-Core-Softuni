namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            //var command = int.Parse(Console.ReadLine());
            using var context = new BookShopContext();
            //DbInitializer.ResetDatabase(context);

            var result = RemoveBooks(context);
            Console.WriteLine(result);
        }

        //Problem 02
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestrictionEnum;
            bool hasSuccess = Enum.TryParse<AgeRestriction>(command, true, out ageRestrictionEnum);

            if (!hasSuccess)
            {
                return String.Empty;
            }

            string[] getBooks = context
                .Books
                .Where(b => b.AgeRestriction == ageRestrictionEnum)
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            return String.Join(Environment.NewLine, getBooks);
        }

        //Problem 03
        public static string GetGoldenBooks(BookShopContext context)
        {
            var getBooks = context
                .Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, getBooks);
        }

        //Problem 04
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var getBooks = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Price,
                    b.Title
                })
                .OrderByDescending(b => b.Price)
                .ToArray();

            foreach (var book in getBooks)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 05
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var getBooks = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, getBooks);
        }

        //Problem 06
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var inputData = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();

            var getBooks = context
                .Books
                .Select(b => new
                {
                    b.Title,
                    BookCategories = b.BookCategories.Select(bc => bc.Category.Name).ToArray()
                })
                .OrderBy(b => b.Title)
                .ToArray()
                .Where(b => 
                {
                    var inputToLower = inputData.Select(c => c.ToLower()).ToArray();
                    var categoriesToLower = b.BookCategories.Select(c => c.ToLower()).ToArray();
                    var intersectedCollection = inputToLower.Intersect(categoriesToLower).ToArray();
                    return intersectedCollection.Count() > 0;
                });

            var sb = new StringBuilder();

            foreach (var book in getBooks)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 07
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var getBooks = context
                .Books
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToArray()
                .Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture));

            var sb = new StringBuilder();

            foreach (var book in getBooks)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 08
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context
                .Authors
                .ToArray()
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName
                })
                .OrderBy(a => $"{a.FirstName} {a.LastName}")
                .ToArray();

            var sb = new StringBuilder();

            foreach (var a in authors)
            {
                sb.AppendLine($"{a.FirstName} {a.LastName}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 09
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var getBooks = context
                .Books
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray()
                .Where(b => b.Contains(input, StringComparison.InvariantCultureIgnoreCase));

            var sb = new StringBuilder();

            foreach (var book in getBooks)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 10
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var getBooks = context
                .Books
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    FirstName = b.Author.FirstName,
                    LastName = b.Author.LastName
                })
                .OrderBy(b => b.BookId)
                .ToArray()
                .Where(b => b.LastName.StartsWith(input, StringComparison.InvariantCultureIgnoreCase));

            var sb = new StringBuilder();

            foreach (var b in getBooks)
            {
                sb.AppendLine($"{b.Title} ({b.FirstName} {b.LastName})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 11
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context
                .Books
                .Select(b => b.Title)
                .Where(b => b.Length > lengthCheck)
                .ToArray();

            return books.Count();
        }

        //Problem 12
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context
                .Authors
                .Select(a => new
                {
                    FullName = $"{a.FirstName} {a.LastName}",
                    TotalCopies = a.Books.Select(b => b.Copies).Sum()
                })
                .OrderByDescending(b => b.TotalCopies)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var a in authors)
            {
                sb.AppendLine($"{a.FullName} - {a.TotalCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var getProfit = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks
                                   .Select(b => b.Book.Copies * b.Book.Price)
                                   .Sum()
                })
                .OrderByDescending(b => b.TotalProfit)
                .ThenBy(b => b.Name)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var x in getProfit)
            {
                sb.AppendLine($"{x.Name} ${x.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 14
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var getBookCategories = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    books = c.CategoryBooks
                        .Select(b => new
                        {
                            ReleaseDate = b.Book.ReleaseDate,
                            BookTitle = b.Book.Title
                        })
                        .OrderByDescending(b => b.ReleaseDate)
                        .Take(3)
                })
                .OrderBy(c => c.Name)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var c in getBookCategories)
            {
                sb.AppendLine($"--{c.Name}");

                foreach (var b in c.books)
                {
                    sb.AppendLine($"{b.BookTitle} ({b.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 15
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var b in books)
            {
                b.Price += 5;
            }

            context.SaveChanges();
        }

        //Problem 16
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.Copies < 4200);

            var deletedBooks = books.Count();

            context.Books.RemoveRange(books);
            context.SaveChanges();

            return deletedBooks;
        }

    }
}
