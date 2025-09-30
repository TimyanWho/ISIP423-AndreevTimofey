using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LibraryConsoleApp
{
    public enum Genre
    {
        Fiction = 1,
        NonFiction = 2,
        Science = 3,
        Fantasy = 4,
        History = 5
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre Genre { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public override string ToString() =>
            $"ID: {Id} | Title: {Title} | Author: {Author} | Genre: {Genre} | Year: {Year} | Price: {Price:C} | Qty: {Quantity}";
    }

    // --- Repository (LINQ heavy) ---
    public class BookRepository
    {
        private readonly List<Book> books = new List<Book>();
        private int nextId = 1;

        public IEnumerable<Book> GetAll() => books;

        public Book Add(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            book.Id = nextId++;
            books.Add(book);
            return book;
        }

        public bool RemoveById(int id)
        {
            var b = books.FirstOrDefault(x => x.Id == id);
            if (b == null) return false;
            return books.Remove(b);
        }

        // Filtering (LINQ Where)
        public IEnumerable<Book> FindByTitle(string titlePart) =>
            books.Where(b => !string.IsNullOrEmpty(b.Title) &&
                             b.Title.IndexOf(titlePart ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0);

        public IEnumerable<Book> FindByAuthor(string authorPart) =>
            books.Where(b => !string.IsNullOrEmpty(b.Author) &&
                             b.Author.IndexOf(authorPart ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0);

        public IEnumerable<Book> FindByGenre(Genre genre) => books.Where(b => b.Genre == genre);

        // Sorting (OrderBy / OrderByDescending)
        public IEnumerable<Book> SortByTitle(bool ascending = true) =>
            ascending ? books.OrderBy(b => b.Title) : books.OrderByDescending(b => b.Title);

        public IEnumerable<Book> SortByYear(bool ascending = true) =>
            ascending ? books.OrderBy(b => b.Year) : books.OrderByDescending(b => b.Year);

        // Min/Max (via OrderBy)
        public Book GetMostExpensive() => books.OrderByDescending(b => b.Price).FirstOrDefault();
        public Book GetLeastExpensive() => books.OrderBy(b => b.Price).FirstOrDefault();

        // Grouping (GroupBy + Select)
        public IEnumerable<(string Author, int Count)> GroupByAuthorCounts() =>
            books.GroupBy(b => b.Author ?? "<Unknown>")
                 .Select(g => (Author: g.Key, Count: g.Count()));

        // Seed data
        public void SeedTestData()
        {
            Add(new Book { Title = "1984", Author = "George Orwell", Genre = Genre.Fiction, Year = 1949, Price = 9.99m, Quantity = 5 });
            Add(new Book { Title = "A Brief History of Time", Author = "Stephen Hawking", Genre = Genre.Science, Year = 1988, Price = 14.5m, Quantity = 3 });
            Add(new Book { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", Genre = Genre.Fantasy, Year = 1954, Price = 25.0m, Quantity = 2 });
            Add(new Book { Title = "Sapiens", Author = "Yuval Noah Harari", Genre = Genre.History, Year = 2011, Price = 18.2m, Quantity = 4 });
            Add(new Book { Title = "Clean Code", Author = "Robert C. Martin", Genre = Genre.NonFiction, Year = 2008, Price = 32.0m, Quantity = 1 });
        }
    }

    // --- Shopping cart ---
    public class CartItem
    {
        public Book Book { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Book.Price * Quantity;
        public override string ToString() => $"{Book.Title} (ID {Book.Id}) x{Quantity} -> {Total:C}";
    }

    public class Cart
    {
        private readonly List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items => items;

        public void Add(Book book, int qty)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));

            var existing = items.FirstOrDefault(i => i.Book.Id == book.Id);
            if (existing != null) existing.Quantity += qty;
            else items.Add(new CartItem { Book = book, Quantity = qty });
        }

        public void Remove(Book book) => items.RemoveAll(i => i.Book.Id == book.Id);

        // Total price (LINQ Sum)
        public decimal GetTotal() => items.Sum(i => i.Total);

        public void Clear() => items.Clear();
    }

    class Program
    {
        static void Main()
        {
            var repo = new BookRepository();
            repo.SeedTestData();
            var cart = new Cart();

            Console.WriteLine("Library App Twin\n");

            bool exit = false;
            while (!exit)
            {
                try
                {
                    ShowMenu();
                    Console.Write("Choose option twin: ");
                    var cmd = Console.ReadLine()?.Trim();

                    switch (cmd)
                    {
                        case "1": AddBookInteractive(repo); break;
                        case "2": RemoveBookInteractive(repo); break;
                        case "3": FindByTitleInteractive(repo); break;
                        case "4": FindByAuthorInteractive(repo); break;
                        case "5": FindByGenreInteractive(repo); break;
                        case "6": SortInteractive(repo); break;
                        case "7": ShowMostAndLeastExpensive(repo); break;
                        case "8": GroupByAuthor(repo); break;
                        case "9": ListAll(repo); break;
                        case "10": InsertBlockOfBooksInteractive(repo); break;
                        case "11": AddBookToCartInteractive(repo, cart); break;
                        case "12": ShowCartInteractive(cart); break;
                        case "13": CheckoutInteractive(repo, cart); break;
                        case "0": exit = true; break;
                        default: Console.WriteLine("Unknown option — try again.\n"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error twin: {ex.Message}\n");
                }
            }

            Console.WriteLine("Goodbye twin, on foenem grave bruh");
        }

        static void ShowMenu()
        {
            Console.WriteLine("Menu twin:");
            Console.WriteLine("1) Add book twin");
            Console.WriteLine("2) Remove book by ID twin");
            Console.WriteLine("3) Find books by title twin");
            Console.WriteLine("4) Find books by author twin");
            Console.WriteLine("5) Find books by genre twin");
            Console.WriteLine("6) Sort books (title/year) twin");
            Console.WriteLine("7) Show most & least expensive twin");
            Console.WriteLine("8) Group by author (counts) twin");
            Console.WriteLine("9) List all books twin");
            Console.WriteLine("10) Insert block of books (batch import) twin");
            Console.WriteLine("11) Add book to cart twin");
            Console.WriteLine("12) Show cart and total twin");
            Console.WriteLine("13) Checkout (reduce stock and clear cart) twin");
            Console.WriteLine("0) Exit twin\n");
        }

        static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.WriteLine("Value cannot be empty twin. Try again twin.");
            }
        }

        static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out int v) && v >= min && v <= max) return v;
                Console.WriteLine($"Enter integer between {min} and {max} twin.");
            }
        }

        static decimal ReadDecimalNonNegative(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal v) && v >= 0) return v;
                Console.WriteLine("Enter non-negative decimal (use dot for decimal part) twin.\n");
            }
        }

        static Genre ReadGenre(string prompt)
        {
            Console.WriteLine(prompt);
            foreach (var g in Enum.GetValues(typeof(Genre))) Console.WriteLine($"{(int)g}) {g}");

            while (true)
            {
                Console.Write("Select genre by number or name twin: ");
                var s = Console.ReadLine();
                if (int.TryParse(s, out int gnum) && Enum.IsDefined(typeof(Genre), gnum)) return (Genre)gnum;
                if (!string.IsNullOrWhiteSpace(s) && Enum.TryParse(typeof(Genre), s.Trim(), true, out object parsed)) return (Genre)parsed;
                Console.WriteLine("Invalid genre twin. Try again twin.");
            }
        }

        static void AddBookInteractive(BookRepository repo)
        {
            Console.WriteLine("\nAdd new book twin:");
            var title = ReadNonEmptyString("Title twin: ");
            var author = ReadNonEmptyString("Author twin: ");
            var genre = ReadGenre("Choose genre twin:");
            var year = ReadIntInRange("Year twin: ", 1450, DateTime.Now.Year + 1);
            var price = ReadDecimalNonNegative("Price twin: ");
            var qty = ReadIntInRange("Quantity twin: ", 0, 1000000);

            var b = new Book { Title = title, Author = author, Genre = genre, Year = year, Price = price, Quantity = qty };
            repo.Add(b);
            Console.WriteLine($"Added twin. ID = {b.Id}\n");
        }

        static void RemoveBookInteractive(BookRepository repo)
        {
            Console.Write("Enter ID to remove twin: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (repo.RemoveById(id)) Console.WriteLine("Removed twin.\n");
                else Console.WriteLine("ID not found twin.\n");
            }
            else Console.WriteLine("Invalid ID twin.\n");
        }

        static void FindByTitleInteractive(BookRepository repo)
        {
            var q = ReadNonEmptyString("Enter title or part of it twin: ");
            var res = repo.FindByTitle(q).ToList();
            PrintBooksResults(res);
        }

        static void FindByAuthorInteractive(BookRepository repo)
        {
            var q = ReadNonEmptyString("Enter author or part of it twin: ");
            var res = repo.FindByAuthor(q).ToList();
            PrintBooksResults(res);
        }

        static void FindByGenreInteractive(BookRepository repo)
        {
            var g = ReadGenre("Choose genre to search twin:");
            var res = repo.FindByGenre(g).ToList();
            PrintBooksResults(res);
        }

        static void SortInteractive(BookRepository repo)
        {
            Console.WriteLine("1) Title asc  2) Title desc  3) Year asc  4) Year desc");
            Console.Write("Choose twin: ");
            var c = Console.ReadLine();
            IEnumerable<Book> sorted = Enumerable.Empty<Book>();
            switch (c)
            {
                case "1": sorted = repo.SortByTitle(true); break;
                case "2": sorted = repo.SortByTitle(false); break;
                case "3": sorted = repo.SortByYear(true); break;
                case "4": sorted = repo.SortByYear(false); break;
                default: Console.WriteLine("Invalid choice twin.\n"); return;
            }
            PrintBooksResults(sorted.ToList());
        }

        static void ShowMostAndLeastExpensive(BookRepository repo)
        {
            var most = repo.GetMostExpensive();
            var least = repo.GetLeastExpensive();
            Console.WriteLine();
            Console.WriteLine("Most expensive twin:");
            Console.WriteLine(most != null ? most.ToString() : "No books twin.");
            Console.WriteLine();
            Console.WriteLine("Least expensive twin:");
            Console.WriteLine(least != null ? least.ToString() : "No books twin.");
            Console.WriteLine();
        }

        static void GroupByAuthor(BookRepository repo)
        {
            var groups = repo.GroupByAuthorCounts().ToList();
            Console.WriteLine();
            Console.WriteLine("Author - Count");
            foreach (var g in groups) Console.WriteLine($"{g.Author} - {g.Count}");
            Console.WriteLine();
        }

        static void ListAll(BookRepository repo)
        {
            var all = repo.GetAll().ToList();
            PrintBooksResults(all);
        }

        static void PrintBooksResults(List<Book> books)
        {
            Console.WriteLine();
            if (books == null || books.Count == 0)
            {
                Console.WriteLine("No books found twin.\n");
                return;
            }
            foreach (var b in books) Console.WriteLine(b.ToString());
            Console.WriteLine();
        }

        static void InsertBlockOfBooksInteractive(BookRepository repo)
        {
            Console.WriteLine("\nPaste multiple lines (Title;Author;Genre;Year;Price) twin. Enter blank line to finish twin.");
            var lines = new List<string>();
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) break;
                lines.Add(line.Trim());
            }

            if (!lines.Any())
            {
                Console.WriteLine("No input provided twin.\n");
                return;
            }

            int added = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var parts = line.Split(';');
                if (parts.Length != 5)
                {
                    Console.WriteLine($"Line {i + 1}: wrong number of fields twin. Skipping twin.");
                    continue;
                }

                var title = parts[0].Trim();
                var author = parts[1].Trim();
                var genreStr = parts[2].Trim();
                var yearStr = parts[3].Trim();
                var priceStr = parts[4].Trim();

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
                {
                    Console.WriteLine($"Line {i + 1}: empty title or author twin. Skipping twin.");
                    continue;
                }

                Genre genre;
                if (int.TryParse(genreStr, out int gnum) && Enum.IsDefined(typeof(Genre), gnum)) genre = (Genre)gnum;
                else if (!string.IsNullOrWhiteSpace(genreStr) && Enum.TryParse(typeof(Genre), genreStr, true, out object parsed)) genre = (Genre)parsed;
                else
                {
                    Console.WriteLine($"Line {i + 1}: unknown genre twin '{genreStr}'. Skipping twin.");
                    continue;
                }

                if (!int.TryParse(yearStr, out int year) || year < 1450 || year > DateTime.Now.Year + 1)
                {
                    Console.WriteLine($"Line {i + 1}: invalid year twin '{yearStr}'. Skipping twin.");
                    continue;
                }

                if (!decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price) || price < 0)
                {
                    Console.WriteLine($"Line {i + 1}: invalid price twin '{priceStr}'. Skipping twin.");
                    continue;
                }

                var book = new Book { Title = title, Author = author, Genre = genre, Year = year, Price = price, Quantity = 1 };
                repo.Add(book);
                added++;
            }

            Console.WriteLine($"Batch import done twin. {added} books added twin.\n");
        }

        static void AddBookToCartInteractive(BookRepository repo, Cart cart)
        {
            Console.Write("Enter book ID to add to cart twin: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid ID twin.\n"); return; }

            var book = repo.GetAll().FirstOrDefault(b => b.Id == id);
            if (book == null) { Console.WriteLine("Book not found twin.\n"); return; }

            Console.Write($"Enter quantity to add (available {book.Quantity}) twin: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0) { Console.WriteLine("Invalid quantity twin.\n"); return; }

            if (qty > book.Quantity) { Console.WriteLine($"Not enough stock ({book.Quantity} twin).\n"); return; }

            cart.Add(book, qty);
            Console.WriteLine($"Added {qty} x '{book.Title}' to cart twin.\n");
        }

        static void ShowCartInteractive(Cart cart)
        {
            var items = cart.Items.ToList();
            Console.WriteLine();
            if (!items.Any()) { Console.WriteLine("Cart is empty twin.\n"); return; }

            Console.WriteLine("Cart contents:");
            foreach (var it in items) Console.WriteLine(it.ToString());
            Console.WriteLine($"\nTotal: {cart.GetTotal():C}\n");
        }

        static void CheckoutInteractive(BookRepository repo, Cart cart)
        {
            var items = cart.Items.ToList();
            if (!items.Any()) { Console.WriteLine("Cart empty.\n"); return; }

            foreach (var it in items)
            {
                var storeBook = repo.GetAll().FirstOrDefault(b => b.Id == it.Book.Id);
                if (storeBook == null) { Console.WriteLine($"'{it.Book.Title}' no longer exists twin. Checkout aborted twin.\n"); return; }
                if (it.Quantity > storeBook.Quantity) { Console.WriteLine($"Not enough stock for '{storeBook.Title}' twin. Available: {storeBook.Quantity} twin. Checkout aborted twin.\n"); return; }
            }

            var total = cart.GetTotal();

            foreach (var it in items)
            {
                var storeBook = repo.GetAll().First(b => b.Id == it.Book.Id);
                storeBook.Quantity -= it.Quantity;
            }

            cart.Clear();
            Console.WriteLine($"Checkout successful twin. Total paid: {total:C} twin. Cart cleared twin.\n");
        }
    }
}
