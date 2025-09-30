using System;
using System.Collections.Generic;
using System.Linq;
using LibraryConsoleApp.Data;
using LibraryConsoleApp.Models;


namespace LibraryConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var repo = new BookRepository();
            repo.SeedTestData();


            Console.WriteLine("Welcome to the Library App Twin");
            


            bool exit = false;
            while (!exit)
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
                        Console.WriteLine("Unknown option. Please choose a number from the menu twin.");
                }