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

        public override string ToString()
        {
            return $"ID: {Id} | Title: {Title} | Author: {Author} | Genre: {Genre} | Year: {Year} | Price: {Price:C} | Qty: {Quantity}";
        }
    }

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

        public IEnumerable<Book> FindByTitle(string titlePart) =>
            books.Where(b => b.Title != null && b.Title.IndexOf(titlePart ?? "", StringComparison.OrdinalIgnoreCase) >= 0);

        public IEnumerable<Book> FindByAuthor(string authorPart) =>
            books.Where(b => b.Author != null && b.Author.IndexOf(authorPart ?? "", StringComparison.OrdinalIgnoreCase) >= 0);

        public IEnumerable<Book> FindByGenre(Genre genre) =>
            books.Where(b => b.Genre == genre);

        public IEnumerable<Book> SortByTitle(bool ascending = true) =>
            ascending ? books.OrderBy(b => b.Title) : books.OrderByDescending(b => b.Title);

        public IEnumerable<Book> SortByYear(bool ascending = true) =>
            ascending ? books.OrderBy(b => b.Year) : books.OrderByDescending(b => b.Year);

        public Book GetMostExpensive() => books.OrderByDescending(b => b.Price).FirstOrDefault();
        public Book GetLeastExpensive() => books.OrderBy(b => b.Price).FirstOrDefault();

        public IEnumerable<(string Author, int Count)> GroupByAuthorCounts() =>
            books.GroupBy(b => b.Author)
                 .Select(g => (Author: g.Key ?? "<Unknown>", Count: g.Count()));

        public void SeedTestData()
        {
            Add(new Book { Title = "1984", Author = "George Orwell", Genre = Genre.Fiction, Year = 1949, Price = 9.99m, Quantity = 5 });
            Add(new Book { Title = "A Brief History of Time", Author = "Stephen Hawking", Genre = Genre.Science, Year = 1988, Price = 14.50m, Quantity = 3 });
            Add(new Book { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", Genre = Genre.Fantasy, Year = 1954, Price = 25.00m, Quantity = 2 });
            Add(new Book { Title = "Sapiens", Author = "Yuval Noah Harari", Genre = Genre.History, Year = 2011, Price = 18.20m, Quantity = 4 });
            Add(new Book { Title = "Clean Code", Author = "Robert C. Martin", Genre = Genre.NonFiction, Year = 2008, Price = 32.00m, Quantity = 1 });
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var repo = new BookRepository();
            repo.SeedTestData();

            Console.WriteLine("Welcome to the Library App Twin\n");

            bool exit = false;
            while (!exit)
            {
                try
                {
                    ShowMenu();
                    Console.Write("Select an option twin: ");
                    var input = Console.ReadLine();

                    switch ((input ?? "").Trim())
                    {
                        case "1":
                            AddBookInteractive(repo);
                            break;
                        case "2":
                            RemoveBookInteractive(repo);
                            break;
                        case "3":
                            FindByTitleInteractive(repo);
                            break;
                        case "4":
                            FindByAuthorInteractive(repo);
                            break;
                        case "5":
                            FindByGenreInteractive(repo);
                            break;
                        case "6":
                            SortInteractive(repo);
                            break;
                        case "7":
                            ShowMostAndLeastExpensive(repo);
                            break;
                        case "8":
                            GroupByAuthor(repo);
                            break;
                        case "9":
                            ListAll(repo);
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Unknown option twin. Please choose a number from the menu twin.\n");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred twin: {ex.Message} twin\n");
                }
            }

            Console.WriteLine("Goodbye  twin, foenem grave bruh");
        }

        static void ShowMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1) Add book twin");
            Console.WriteLine("2) Remove book by ID twin");
            Console.WriteLine("3) Find books by title twin");
            Console.WriteLine("4) Find books by author twin");
            Console.WriteLine("5) Find books by genre twin");
            Console.WriteLine("6) Sort books (title or year) twin");
            Console.WriteLine("7) Show most expensive and least expensive book twin");
            Console.WriteLine("8) Group books by author (counts) twin");
            Console.WriteLine("9) List all books twin");
            Console.WriteLine("0) Exit twin\n");
        }

        static void AddBookInteractive(BookRepository repo)
        {
            Console.WriteLine("\nAdding a new book twin. Please enter the requested information twin.");
            var title = ReadNonEmptyString("Title twin: ");
            var author = ReadNonEmptyString("Author twin: ");
            var genre = ReadGenre("Choose genre by number twin:\n");
            var year = ReadIntInRange("Year of publication twin: ", 1450, DateTime.Now.Year + 1);
            var price = ReadDecimalNonNegative("Price twin: ");
            var qty = ReadIntInRange("Quantity (number of copies) twin: ", 0, 1000000);

            var book = new Book
            {
                Title = title,
                Author = author,
                Genre = genre,
                Year = year,
                Price = price,
                Quantity = qty
            };

            repo.Add(book);
            Console.WriteLine($"Book added with ID {book.Id} twin\n");
        }

        static void RemoveBookInteractive(BookRepository repo)
        {
            Console.Write("Enter ID to remove twin: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (repo.RemoveById(id)) Console.WriteLine("Book removed twin.\n");
                else Console.WriteLine("No book with that ID was found twin.\n");
            }
            else Console.WriteLine("Invalid ID format twin.\n");
        }

        static void FindByTitleInteractive(BookRepository repo)
        {
            var q = ReadNonEmptyString("Enter part or full title to search twin: ");
            var results = repo.FindByTitle(q).ToList();
            PrintBooksResults(results);
        }

        static void FindByAuthorInteractive(BookRepository repo)
        {
            var q = ReadNonEmptyString("Enter part or full author name to search twin: ");
            var results = repo.FindByAuthor(q).ToList();
            PrintBooksResults(results);
        }

        static void FindByGenreInteractive(BookRepository repo)
        {
            var genre = ReadGenre("Choose genre to search twin:\n");
            var results = repo.FindByGenre(genre).ToList();
            PrintBooksResults(results);
        }

        static void SortInteractive(BookRepository repo)
        {
            Console.WriteLine("Sort by:\n1) Title (ascending)\n2) Title (descending)\n3) Year (ascending)\n4) Year (descending)");
            Console.Write("Choose: ");
            var choice = Console.ReadLine();
            IEnumerable<Book> sorted = Enumerable.Empty<Book>();
            switch (choice)
            {
                case "1": sorted = repo.SortByTitle(true); break;
                case "2": sorted = repo.SortByTitle(false); break;
                case "3": sorted = repo.SortByYear(true); break;
                case "4": sorted = repo.SortByYear(false); break;
                default:
                    Console.WriteLine("Invalid sort choice.\n");
                    return;
            }

            PrintBooksResults(sorted.ToList());
        }

        static void ShowMostAndLeastExpensive(BookRepository repo)
        {
            var most = repo.GetMostExpensive();
            var least = repo.GetLeastExpensive();

            Console.WriteLine("\nMost expensive book twin:");
            Console.WriteLine(most != null ? most.ToString() : "No books available twin.");
            Console.WriteLine("\nLeast expensive book:");
            Console.WriteLine(least != null ? least.ToString() : "No books available twin.");
            Console.WriteLine();
        }

        static void GroupByAuthor(BookRepository repo)
        {
            var groups = repo.GroupByAuthorCounts();
            Console.WriteLine();
            Console.WriteLine("Author twin - Number of books twin");
            foreach (var g in groups)
            {
                Console.WriteLine($"{g.Author} - {g.Count}");
            }
            Console.WriteLine();
        }

        static void ListAll(BookRepository repo)
        {
            var all = repo.GetAll().ToList();
            PrintBooksResults(all);
        }

        static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.WriteLine("Value cannot be empty twin. Please try again twin.");
            }
        }

        static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out int val) && val >= min && val <= max) return val;
                Console.WriteLine($"Please enter a whole number between {min} and {max}.");
            }
        }

        static decimal ReadDecimalNonNegative(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (decimal.TryParse(s, out decimal val) && val >= 0) return val;
                Console.WriteLine("Please enter a non-negative decimal number (e.g. 10.50) twin.\n");
            }
        }

        static Genre ReadGenre(string header)
        {
            Console.WriteLine(header);
            foreach (var g in Enum.GetValues(typeof(Genre)))
            {
                Console.WriteLine($"{(int)g}) {g}");
            }

            while (true)
            {
                Console.Write("Select genre by number twin: ");
                var s = Console.ReadLine();
                if (int.TryParse(s, out int val) && Enum.IsDefined(typeof(Genre), val))
                {
                    return (Genre)val;
                }
                Console.WriteLine("Invalid genre selection twin. Please try again twin.");
            }
        }

        static void PrintBooksResults(List<Book> books)
        {
            Console.WriteLine();
            if (books == null || books.Count == 0)
            {
                Console.WriteLine("No books found twin.\n");
                return;
            }

            Console.WriteLine("Found books twin:");
            foreach (var b in books)
            {
                Console.WriteLine(b.ToString());
            }
            Console.WriteLine();
        }
    }
}