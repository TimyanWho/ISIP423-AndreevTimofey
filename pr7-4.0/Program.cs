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

    #region Marketplace Core (in-memory)

    public class Marketplace
    {
        public List<User> Users { get; private set; }
        public List<Product> Products { get; private set; }
        public List<PVZ> PVZs { get; private set; }
        public List<Order> Orders { get; private set; }

        private int _userIdSeq = 1;
        private int _orderIdSeq = 1;

        public Marketplace()
        {
            Users = new List<User>();
            Products = new List<Product>();
            PVZs = new List<PVZ>();
            Orders = new List<Order>();

            Seed();
        }
        public User RegisterUser(string username, string password, string passwordConfirm, string email)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username пустой");
            if (password == null) throw new ArgumentException("Password пустой");
            if (password != passwordConfirm) throw new ArgumentException("Пароли не совпадают");
            if (Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))) throw new InvalidOperationException("Пользователь с таким именем уже существует");

            var user = new User { Id = _userIdSeq++, Username = username, PasswordHash = HashPwd(password), Email = email };
            user.ValidateForRegistration();
            Users.Add(user);
            return user;
        }

        public User Login(string username, string password)
        {
            var user = Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) throw new InvalidOperationException("Пользователь не найден");
            if (user.PasswordHash != HashPwd(password)) throw new InvalidOperationException("Неверный пароль");
            return user;
        }

        public void AddProductToCart(User user, int productId, int quantity)
        {
            if (user == null) throw new ArgumentNullException("user");
            var pr = Products.FirstOrDefault(p => p.Id == productId);
            if (pr == null) throw new InvalidOperationException("Товар не найден");
            if (quantity <= 0) throw new ArgumentException("Quantity должен быть положительным");
            if (pr.Stock < quantity) throw new InvalidOperationException("Недостаточно товара на складе");

            var exist = user.Cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (exist != null)
            {
                exist.Quantity += quantity;
            }
            else
            {
                user.Cart.Items.Add(new CartItem { ProductId = productId, Product = pr, Quantity = quantity });
            }
        }

        public Order CheckoutSingle(User user, int productId, int quantity, int pvzId)
        {
            if (user == null) throw new ArgumentNullException("user");
            var pr = Products.FirstOrDefault(p => p.Id == productId);
            if (pr == null) throw new InvalidOperationException("Товар не найден");
            if (quantity <= 0) throw new ArgumentException("Quantity должен быть положительным");
            if (pr.Stock < quantity) throw new InvalidOperationException("Недостаточно товара на складе");

            pr.Stock -= quantity;

            var order = new Order { Id = _orderIdSeq++, UserId = user.Id, PVZId = pvzId, CreatedAt = DateTime.UtcNow };
            order.Items.Add(new CartItem { ProductId = pr.Id, Product = pr, Quantity = quantity });
            order.Total = pr.Price * quantity;

            Orders.Add(order);
            user.Orders.Add(order);

            return order;
        }

        public Order CheckoutCart(User user, int pvzId)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (user.Cart.Items.Count == 0) throw new InvalidOperationException("Корзина пуста");

            foreach (var it in user.Cart.Items)
            {
                var pr = Products.FirstOrDefault(p => p.Id == it.ProductId);
                if (pr == null) throw new InvalidOperationException("Товар в корзине не найден");
                if (pr.Stock < it.Quantity) throw new InvalidOperationException("Недостаточно товара на складе для " + pr.Name);
            }

            var order = new Order { Id = _orderIdSeq++, UserId = user.Id, PVZId = pvzId, CreatedAt = DateTime.UtcNow };
            decimal total = 0m;
            foreach (var it in user.Cart.Items)
            {
                var pr = Products.First(p => p.Id == it.ProductId);
                pr.Stock -= it.Quantity;
                order.Items.Add(new CartItem { ProductId = pr.Id, Product = pr, Quantity = it.Quantity });
                total += pr.Price * it.Quantity;
            }
            order.Total = total;

            Orders.Add(order);
            user.Orders.Add(order);

            user.Cart.Items.Clear();

            return order;
        }

        public List<Order> GetOrdersForUser(int userId, bool newestFirst)
        {
            var list = Orders.Where(o => o.UserId == userId).ToList();
            if (newestFirst) list = list.OrderByDescending(o => o.CreatedAt).ToList(); else list = list.OrderBy(o => o.CreatedAt).ToList();
            return list;
        }

        private static string HashPwd(string pwd)
        {
            if (pwd == null) return string.Empty;
            return pwd.GetHashCode().ToString();
        }

        private void Seed()
        {
            Products.Add(new Product { Id = 1, Name = "Клавиатура механическая", Description = "RGB, Cherry MX", Price = 4999.00m, Stock = 10 });
            Products.Add(new Product { Id = 2, Name = "Мышь игровая", Description = "16000 DPI", Price = 2499.00m, Stock = 20 });
            Products.Add(new Product { Id = 3, Name = "Коврик для мыши", Description = "Большой, тканевый", Price = 799.00m, Stock = 50 });

            PVZs.Add(new PVZ { Id = 1, Name = "ПВЗ - Центральный", Address = "ул. Ленина 1" });
            PVZs.Add(new PVZ { Id = 2, Name = "ПВЗ - Северный", Address = "ул. Садовая 10" });
            PVZs.Add(new PVZ { Id = 3, Name = "ПВЗ - Юго-Восточный", Address = "ул. Восточная 23" });
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
