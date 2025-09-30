using LibraryConsoleApp.Models;


namespace LibraryConsoleApp.Data
{
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
        }