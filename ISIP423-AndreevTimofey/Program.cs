namespace LibraryConsoleApp.Models
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
}