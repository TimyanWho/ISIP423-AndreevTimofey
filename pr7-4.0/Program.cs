using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketplacePr8
{

    #region Модели

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        public List<Order> Orders { get; set; }
        public Cart Cart { get; set; }

        public User()
        {
            Username = string.Empty;
            PasswordHash = string.Empty;
            Email = string.Empty;
            Orders = new List<Order>();
            Cart = new Cart();
        }

        public void ValidateForRegistration()
        {
            if (string.IsNullOrWhiteSpace(Username)) throw new ArgumentException("Username не может быть пустым");
            if (string.IsNullOrWhiteSpace(PasswordHash)) throw new ArgumentException("Password не может быть пустым");
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product()
        {
            Name = string.Empty;
            Description = string.Empty;
            Price = 0m;
            Stock = 0;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Name не может быть пустым");
            if (Price < 0) throw new ArgumentException("Price не может быть отрицательной");
            if (Stock < 0) throw new ArgumentException("Stock не может быть отрицательным");
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem()
        {
            Product = new Product();
            Quantity = 0;
        }
    }

    public class Cart
    {
        public List<CartItem> Items { get; set; }
        public Cart()
        {
            Items = new List<CartItem>();
        }

        public decimal TotalAmount()
        {
            decimal sum = 0m;
            foreach (var it in Items) sum += it.Product.Price * it.Quantity;
            return sum;
        }
    }

    public class PVZ
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public PVZ()
        {
            Name = string.Empty;
            Address = string.Empty;
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItem> Items { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PVZId { get; set; }

        public Order()
        {
            Items = new List<CartItem>();
            Total = 0m;
            CreatedAt = DateTime.UtcNow;
        }
    }

    #endregion

   


    public static class Program
    {
        private static Marketplace _mp = new Marketplace();
        private static User _currentUser = null;

        public static void Main(string[] args)
        {
            Console.WriteLine("GMWOG Marketplace — консольная версия (Практическая №8)");
            RunMainMenu();
        }

        private static void RunMainMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== Главное меню ===");
                Console.WriteLine("1) Просмотреть товары (без входа)");
                Console.WriteLine("2) Регистрация");
                Console.WriteLine("3) Вход в аккаунт");
                Console.WriteLine("4) Личный кабинет (только после входа)");
                Console.WriteLine("0) Выход");
                Console.Write("Выберите действие: ");
                var line = Console.ReadLine(); int choice; if (!int.TryParse(line, out choice)) { Console.WriteLine("Неверный ввод"); continue; }

                try
                {
                    switch (choice)
                    {
                        case 1: ShowProducts(); break;
                        case 2: RegisterInteractive(); break;
                        case 3: LoginInteractive(); break;
                        case 4: PersonalMenu(); break;
                        case 0: return;
                        default: Console.WriteLine("Неизвестный пункт меню"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }
        }

        private static void ShowProducts()
        {
            Console.WriteLine("Список товаров:");
            foreach (var p in _mp.Products)
            {
                Console.WriteLine($"Id={p.Id} {p.Name} — {p.Price} ₽ — В наличии: {p.Stock}\n  {p.Description}");
            }
        }

        private static void RegisterInteractive()
        {
            Console.WriteLine("== Регистрация ==");
            Console.Write("Логин: "); var login = Console.ReadLine();
            Console.Write("Email (опционально): "); var email = Console.ReadLine();
            Console.Write("Пароль: "); var pwd = ReadPassword();
            Console.Write("Подтвердите пароль: "); var pwd2 = ReadPassword();

            try
            {
                var user = _mp.RegisterUser(login, pwd, pwd2, email);
                Console.WriteLine("Регистрация прошла успешно. Вы вошли в систему.");
                _currentUser = user;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось зарегистрироваться: " + ex.Message);
            }
        }

        private static void LoginInteractive()
        {
            Console.WriteLine("== Вход ==");
            Console.Write("Логин: "); var login = Console.ReadLine();
            Console.Write("Пароль: "); var pwd = ReadPassword();
            try
            {
                var user = _mp.Login(login, pwd);
                Console.WriteLine("Вход выполнен. Привет, " + user.Username);
                _currentUser = user;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось войти: " + ex.Message);
            }
        }

        private static void PersonalMenu()
        {
            if (_currentUser == null) { Console.WriteLine("Сначала войдите в аккаунт"); return; }
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== Личный кабинет ===");
                Console.WriteLine("1) Посмотреть корзину");
                Console.WriteLine("2) Добавить товар в корзину");
                Console.WriteLine("3) Оформить покупку отдельного товара");
                Console.WriteLine("4) Оформить покупку корзины");
                Console.WriteLine("5) История заказов (сортировка по дате)");
                Console.WriteLine("9) Выйти из аккаунта");
                Console.WriteLine("0) Назад");
                Console.Write("Выберите: "); var line = Console.ReadLine(); int ch; if (!int.TryParse(line, out ch)) { Console.WriteLine("Неверный ввод"); continue; }
                try
                {
                    switch (ch)
                    {
                        case 1: ShowCart(); break;
                        case 2: AddToCartInteractive(); break;
                        case 3: CheckoutSingleInteractive(); break;
                        case 4: CheckoutCartInteractive(); break;
                        case 5: ShowOrderHistory(); break;
                        case 9: _currentUser = null; Console.WriteLine("Вы вышли из аккаунта"); return;
                        case 0: return;
                        default: Console.WriteLine("Неизвестный пункт"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }
        }

        private static void ShowCart()
        {
            Console.WriteLine("=== Корзина ===");
            var cart = _currentUser.Cart;
            if (cart.Items.Count == 0) { Console.WriteLine("Корзина пуста"); return; }
            foreach (var it in cart.Items) Console.WriteLine($"{it.Product.Name} x{it.Quantity} = {it.Product.Price * it.Quantity} ₽");
            Console.WriteLine("Итог: " + cart.TotalAmount() + " ₽");
        }

        private static void AddToCartInteractive()
        {
            ShowProducts();
            Console.Write("Введите Id товара: "); var s = Console.ReadLine(); int pid; if (!int.TryParse(s, out pid)) { Console.WriteLine("Неверный Id"); return; }
            Console.Write("Введите количество: "); s = Console.ReadLine(); int qty; if (!int.TryParse(s, out qty)) { Console.WriteLine("Неверное число"); return; }
            _mp.AddProductToCart(_currentUser, pid, qty);
            Console.WriteLine("Добавлено в корзину");
        }

        private static void CheckoutSingleInteractive()
        {
            ShowProducts();
            Console.Write("Введите Id товара: "); var s = Console.ReadLine(); int pid; if (!int.TryParse(s, out pid)) { Console.WriteLine("Неверный Id"); return; }
            Console.Write("Введите количество: "); s = Console.ReadLine(); int qty; if (!int.TryParse(s, out qty)) { Console.WriteLine("Неверное число"); return; }
            ShowPVZs(); Console.Write("Выберите Id ПВЗ: "); s = Console.ReadLine(); int pvz; if (!int.TryParse(s, out pvz)) { Console.WriteLine("Неверный Id"); return; }
            var order = _mp.CheckoutSingle(_currentUser, pid, qty, pvz);
            Console.WriteLine("Оформлен заказ Id=" + order.Id + " Сумма=" + order.Total + " ₽");
        }

        private static void CheckoutCartInteractive()
        {
            ShowCart();
            ShowPVZs(); Console.Write("Выберите Id ПВЗ: "); var s = Console.ReadLine(); int pvz; if (!int.TryParse(s, out pvz)) { Console.WriteLine("Неверный Id"); return; }
            var order = _mp.CheckoutCart(_currentUser, pvz);
            Console.WriteLine("Оформлен заказ Id=" + order.Id + " Сумма=" + order.Total + " ₽");
        }

        private static void ShowOrderHistory()
        {
            Console.WriteLine("Показать заказы: 1) Сначала новые  2) Сначала старые"); var s = Console.ReadLine(); bool newest = s == "1";
            var list = _mp.GetOrdersForUser(_currentUser.Id, newest);
            if (list.Count == 0) { Console.WriteLine("Заказов нет"); return; }
            foreach (var o in list)
            {
                Console.WriteLine($"OrderId={o.Id} Date={o.CreatedAt} Total={o.Total} ₽ PVZ={o.PVZId}");
                foreach (var it in o.Items) Console.WriteLine($"  {it.Product.Name} x{it.Quantity} = {it.Product.Price * it.Quantity} ₽");
            }
        }

        private static void ShowPVZs()
        {
            Console.WriteLine("Доступные ПВЗ:");
            foreach (var p in _mp.PVZs) Console.WriteLine($"Id={p.Id} {p.Name} — {p.Address}");
        }

        private static string ReadPassword()
        {
            var pwd = string.Empty;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && pwd.Length > 0) { pwd = pwd.Substring(0, pwd.Length - 1); Console.Write("\b \b"); }
                else if (key.Key != ConsoleKey.Backspace) { pwd += key.KeyChar; Console.Write("*"); }
            }
            Console.WriteLine();
            return pwd;
        }
    }

}
