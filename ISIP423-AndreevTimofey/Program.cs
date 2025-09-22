public enum Category
{
    Food,
    Electronics,
    Clothing,
}

public class Product
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public bool InStock => Quantity > 0;
    public Category Category { get; private set; }

    internal Product(string code, string name, decimal price, int quantity, Category category)
    {
        Code = code;
        Name = name;
        Price = System.Decimal.Round(price, 2);
        Quantity = quantity;
        Category = category;
    }

    internal void ChangeQuantity(int delta)
    {
        Quantity += delta;
        if (Quantity < 0) Quantity = 0;
    }

    public string FullInfo()
    {
        return $"{Code} | {Name} | Категория: {Category} | Цена: {Price:0.00} руб | Кол-во: {Quantity} | На складе: {(InStock ? "Да" : "Нет")}";
    }
}

public class SaleRecord
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => System.Decimal.Round(UnitPrice * Quantity, 2);

    public override string ToString()
    {
        return $"{ProductCode} | {ProductName} | Шт: {Quantity} | Цена: {UnitPrice:0.00} руб | Сумма: {Total:0.00} руб";
    }
}

public static class Inventory
{
    public static System.Collections.Generic.List<Product> Products { get; } = new System.Collections.Generic.List<Product>();

    public static System.Collections.Generic.Stack<SaleRecord> SalesHistory { get; } = new System.Collections.Generic.Stack<SaleRecord>();

    public static System.Collections.Generic.List<SaleRecord> CompletedSales { get; } = new System.Collections.Generic.List<SaleRecord>();

    private static int nextId = 1000;

    public static string GetNextCode()
    {
        nextId++;
        return "1" + nextId.ToString();
    }

    public static Product AddProduct(string name, decimal price, int quantity, Category category, out string error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(name)) { error = "Название товара не может быть пустым."; return null; }
        if (price < 0) { error = "Цена не может быть отрицательной."; return null; }
        if (quantity < 0) { error = "Количество не может быть отрицательным."; return null; }
        string code = GetNextCode();
        var prod = new Product(code, name.Trim(), System.Decimal.Round(price, 2), quantity, category);
        Products.Add(prod);
        return prod;
    }

    public static bool RemoveByCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        var p = FindByCode(code);
        if (p == null) return false;
        return Products.Remove(p);
    }

    public static bool Restock(string code, int quantity, out string error)
    {
        error = null;
        if (quantity <= 0) { error = "Количество поставки должно быть положительным."; return false; }
        var p = FindByCode(code);
        if (p == null) { error = "Товар с таким кодом не найден."; return false; }
        p.ChangeQuantity(quantity);
        return true;
    }

    public static bool Sell(string code, int quantity, out string error)
    {
        error = null;
        if (quantity <= 0) { error = "Количество продажи должно быть положительным."; return false; }
        var p = FindByCode(code);
        if (p == null) { error = "Товар с таким кодом не найден."; return false; }
        if (p.Quantity < quantity) { error = $"На складе недостаточно товара. Доступно: {p.Quantity}."; return false; }
        p.ChangeQuantity(-quantity);
        return true;
    }

    public static Product FindByCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        foreach (var p in Products) if (p.Code == code) return p;
        return null;
    }

    public static System.Collections.Generic.List<Product> FindByName(string query)
    {
        var res = new System.Collections.Generic.List<Product>();
        if (string.IsNullOrWhiteSpace(query)) return res;
        string q = query.Trim().ToLowerInvariant();
        foreach (var p in Products) if (p.Name.ToLowerInvariant().Contains(q)) res.Add(p);
        return res;
    }

    public static System.Collections.Generic.List<Product> FindByCategory(Category category)
    {
        var res = new System.Collections.Generic.List<Product>();
        foreach (var p in Products) if (p.Category == category) res.Add(p);
        return res;
    }

    public static void SortByPriceAscending()
    {
        Products.Sort((a, b) => a.Price.CompareTo(b.Price));
    }

    public static decimal TotalStockValue()
    {
        decimal sum = 0;
        foreach (var p in Products) sum += p.Price * p.Quantity;
        return System.Decimal.Round(sum, 2);
    }

    public static void SeedTestData()
    {
        if (Products.Count > 0) return;
        Products.Add(new Product(GetNextCode(), "Молоко 1л", 89.90m, 20, Category.Food));
        Products.Add(new Product(GetNextCode(), "Наушники Bluetooth", 2499.50m, 5, Category.Electronics));
        Products.Add(new Product(GetNextCode(), "Футболка XL", 799.00m, 12, Category.Clothing));
        Products.Add(new Product(GetNextCode(), "Хлеб", 45.00m, 30, Category.Food));
        Products.Add(new Product(GetNextCode(), "Зарядное USB-C", 1299.00m, 8, Category.Electronics));
    }

    public static void RecordSale(SaleRecord record)
    {
        if (record == null) return;
        SalesHistory.Push(record);
        CompletedSales.Add(record);
    }

    public static bool UndoLastSale(out string error)
    {
        error = null;
        if (SalesHistory.Count == 0) { error = "История продаж пуста — нечего отменять."; return false; }

        var last = SalesHistory.Pop();
        var prod = FindByCode(last.ProductCode);
        if (prod == null)
        {
            error = $"Товар с кодом {last.ProductCode} не найден в инвентаре — отмена невозможна.";
            SalesHistory.Push(last);
            return false;
        }

        prod.ChangeQuantity(last.Quantity);

        for (int i = CompletedSales.Count - 1; i >= 0; i--)
        {
            var r = CompletedSales[i];
            if (r.ProductCode == last.ProductCode && r.Quantity == last.Quantity)
            {
                CompletedSales.RemoveAt(i);
                break;
            }
        }

        return true;
    }

    public static System.Collections.Generic.List<(string ProductCode, string ProductName, int TotalQuantity, decimal TotalRevenue)> GetSalesReport()
    {
        var map = new System.Collections.Generic.Dictionary<string, (string name, int qty, decimal revenue)>();
        foreach (var s in CompletedSales)
        {
            if (!map.ContainsKey(s.ProductCode))
                map[s.ProductCode] = (s.ProductName, s.Quantity, s.Total);
            else
            {
                var cur = map[s.ProductCode];
                cur.qty += s.Quantity;
                cur.revenue += s.Total;
                map[s.ProductCode] = cur;
            }
        }

        var list = new System.Collections.Generic.List<(string, string, int, decimal)>();
        foreach (var kv in map)
            list.Add((kv.Key, kv.Value.name, kv.Value.qty, System.Decimal.Round(kv.Value.revenue, 2)));
        return list;
    }
}

public static class Program
{
    public static void Main()
    {
        Inventory.SeedTestData();
        RunMenu();
    }

    private static void RunMenu()
    {
        while (true)
        {
            PrintHeader();
            System.Console.WriteLine("Меню:");
            System.Console.WriteLine("1. Показать все товары");
            System.Console.WriteLine("2. Добавить товар");
            System.Console.WriteLine("3. Удалить товар");
            System.Console.WriteLine("4. Заказать поставку (пополнить)");
            System.Console.WriteLine("5. Продать товар");
            System.Console.WriteLine("6. Поиск товаров (код/название/категория)");
            System.Console.WriteLine("7. Сортировать по цене (возрастание)");
            System.Console.WriteLine("8. Показать статистику склада");
            System.Console.WriteLine("9. История продаж (последние сверху)");
            System.Console.WriteLine("10. Отменить последнюю продажу (undo)");
            System.Console.WriteLine("11. Отчёт о продажах (агрегация)");
            System.Console.WriteLine("0. Выход");
            System.Console.Write("Выберите пункт: ");
            string choice = (System.Console.ReadLine() ?? "").Trim();

            if (choice == "1") PrintAllProducts();
            else if (choice == "2") UiAddProduct();
            else if (choice == "3") UiRemoveProduct();
            else if (choice == "4") UiRestock();
            else if (choice == "5") UiSell();
            else if (choice == "6") UiSearch();
            else if (choice == "7") UiSortByPrice();
            else if (choice == "8") UiShowStats();
            else if (choice == "9") UiShowSalesHistory();
            else if (choice == "10") UiUndoLastSale();
            else if (choice == "11") UiShowSalesReport();
            else if (choice == "0") break;
            else System.Console.WriteLine("Неверный пункт.");

            System.Console.WriteLine();
            System.Console.WriteLine("Нажмите Enter для продолжения...");
            System.Console.ReadLine();
        }
    }

    private static void PrintHeader()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("=== Учёт товаров в магазине ===");
        System.Console.WriteLine();
    }

    private static void PrintAllProducts()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Список товаров:");
        if (Inventory.Products.Count == 0) { System.Console.WriteLine("Список пуст."); return; }
        foreach (var p in Inventory.Products) System.Console.WriteLine(p.FullInfo());
        System.Console.WriteLine();
        System.Console.WriteLine($"Общая стоимость всего запаса (цена×кол-во): {Inventory.TotalStockValue():0.00} руб");
    }

    private static int ReadPositiveInt(string prompt, int min = 1, int max = int.MaxValue)
    {
        while (true)
        {
            System.Console.Write(prompt);
            string s = System.Console.ReadLine();
            if (int.TryParse(s, out int v) && v >= min && v <= max) return v;
            System.Console.WriteLine($"Ошибка. Введите целое число от {min} до {max}.");
        }
    }

    private static decimal ReadPrice(string prompt)
    {
        while (true)
        {
            System.Console.Write(prompt);
            string s = (System.Console.ReadLine() ?? "").Trim().Replace(',', '.');
            if (decimal.TryParse(s, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal val) && val >= 0)
                return System.Decimal.Round(val, 2);
            System.Console.WriteLine("Ошибка. Введите неотрицательное число, например 199 или 199.99");
        }
    }

    private static string ReadNonEmptyString(string prompt)
    {
        while (true)
        {
            System.Console.Write(prompt);
            string s = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            System.Console.WriteLine("Ошибка. Строка не должна быть пустой.");
        }
    }

    private static Category ReadCategory()
    {
        while (true)
        {
            System.Console.WriteLine("Доступные категории:");
            var names = System.Enum.GetNames(typeof(Category));
            for (int i = 0; i < names.Length; i++) System.Console.WriteLine($"{i + 1}. {names[i]}");
            int choice = ReadPositiveInt("Выберите категорию (номер): ", 1, names.Length);
            return (Category)(choice - 1);
        }
    }

    private static void UiAddProduct()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Добавление товара:");
        string name = ReadNonEmptyString("Название: ");
        decimal price = ReadPrice("Цена (руб): ");
        int qty = ReadPositiveInt("Количество (целое >= 0): ", 0, 1000000);
        Category cat = ReadCategory();
        var prod = Inventory.AddProduct(name, price, qty, cat, out string err);
        if (prod != null) System.Console.WriteLine("Товар успешно добавлен: " + prod.FullInfo());
        else System.Console.WriteLine("Не удалось добавить товар: " + err);
    }

    private static void UiRemoveProduct()
    {
        System.Console.WriteLine();
        string code = ReadNonEmptyString("Введите код товара для удаления: ");
        var p = Inventory.FindByCode(code);
        if (p == null) { System.Console.WriteLine("Товар не найден."); return; }
        System.Console.WriteLine("Найден товар: " + p.FullInfo());
        System.Console.Write("Подтвердите удаление (y/N): ");
        string conf = (System.Console.ReadLine() ?? "").Trim().ToLowerInvariant();
        if (conf == "y" || conf == "yes")
        {
            if (Inventory.RemoveByCode(code)) System.Console.WriteLine("Товар удалён.");
            else System.Console.WriteLine("Не удалось удалить товар.");
        }
        else System.Console.WriteLine("Удаление отменено.");
    }

    private static void UiRestock()
    {
        System.Console.WriteLine();
        string code = ReadNonEmptyString("Код товара для пополнения: ");
        int qty = ReadPositiveInt("Количество для добавления (целое > 0): ", 1, 1000000);
        if (Inventory.Restock(code, qty, out string err)) System.Console.WriteLine("Поставка учтена.");
        else System.Console.WriteLine("Ошибка: " + err);
    }

    private static void UiSell()
    {
        System.Console.WriteLine();
        string code = ReadNonEmptyString("Код товара для продажи: ");
        var prod = Inventory.FindByCode(code);
        if (prod == null) { System.Console.WriteLine("Товар не найден."); return; }

        int qty = ReadPositiveInt("Количество для продажи (целое > 0): ", 1, 1000000);

        decimal unitPrice = prod.Price;
        string name = prod.Name;

        if (Inventory.Sell(code, qty, out string err))
        {
            var rec = new SaleRecord
            {
                ProductCode = code,
                ProductName = name,
                Quantity = qty,
                UnitPrice = unitPrice,
            };
            Inventory.RecordSale(rec);
            System.Console.WriteLine("Продажа выполнена и сохранена в истории.");
            System.Console.WriteLine(rec.ToString());
        }
        else
        {
            System.Console.WriteLine("Ошибка: " + err);
        }
    }

    private static void UiSearch()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Поиск:");
        System.Console.WriteLine("1. По коду");
        System.Console.WriteLine("2. По названию");
        System.Console.WriteLine("3. По категории");
        int ch = ReadPositiveInt("Выберите способ поиска: ", 1, 3);
        if (ch == 1)
        {
            string code = ReadNonEmptyString("Код: ");
            var p = Inventory.FindByCode(code);
            if (p == null) System.Console.WriteLine("Не найдено.");
            else System.Console.WriteLine(p.FullInfo());
        }
        else if (ch == 2)
        {
            string q = ReadNonEmptyString("Текст для поиска в названии: ");
            var list = Inventory.FindByName(q);
            if (list.Count == 0) System.Console.WriteLine("Ничего не найдено.");
            else { System.Console.WriteLine($"Найдено {list.Count}:"); foreach (var p in list) System.Console.WriteLine(p.FullInfo()); }
        }
        else
        {
            var cat = ReadCategory();
            var list = Inventory.FindByCategory(cat);
            if (list.Count == 0) System.Console.WriteLine("Ничего не найдено в этой категории.");
            else { System.Console.WriteLine($"Найдено {list.Count}:"); foreach (var p in list) System.Console.WriteLine(p.FullInfo()); }
        }
    }

    private static void UiSortByPrice()
    {
        System.Console.WriteLine();
        Inventory.SortByPriceAscending();
        System.Console.WriteLine("Сортировка по цене (возрастание) выполнена.");
    }

    private static void UiShowStats()
    {
        System.Console.WriteLine();
        if (Inventory.Products.Count == 0) { System.Console.WriteLine("Список пуст."); return; }
        decimal totalValue = Inventory.TotalStockValue();
        int totalItems = 0;
        decimal sumPrice = 0;
        int idxMax = 0, idxMin = 0;
        for (int i = 0; i < Inventory.Products.Count; i++)
        {
            var p = Inventory.Products[i];
            totalItems += p.Quantity;
            sumPrice += p.Price;
            if (p.Price > Inventory.Products[idxMax].Price) idxMax = i;
            if (p.Price < Inventory.Products[idxMin].Price) idxMin = i;
        }
        decimal avgPrice = Inventory.Products.Count > 0 ? System.Decimal.Round(sumPrice / Inventory.Products.Count, 2) : 0;
        System.Console.WriteLine($"Всего наименований: {Inventory.Products.Count}");
        System.Console.WriteLine($"Общее кол-во штук в запасе: {totalItems}");
        System.Console.WriteLine($"Средняя цена по наименованию: {avgPrice:0.00} руб");
        System.Console.WriteLine($"Самая дорогая позиция: {Inventory.Products[idxMax].FullInfo()}");
        System.Console.WriteLine($"Самая дёшевая позиция: {Inventory.Products[idxMin].FullInfo()}");
        System.Console.WriteLine($"Общая стоимость запасов: {totalValue:0.00} руб");
    }

    private static void UiShowSalesHistory()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("История продаж (последние вверху):");
        if (Inventory.SalesHistory.Count == 0) { System.Console.WriteLine("История пуста."); return; }
        int idx = Inventory.SalesHistory.Count;
        foreach (var rec in Inventory.SalesHistory)
        {
            System.Console.WriteLine($"{idx--}. {rec.ToString()}");
        }
    }

    private static void UiUndoLastSale()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Отмена последней продажи:");
        if (Inventory.SalesHistory.Count == 0) { System.Console.WriteLine("История пуста — нечего отменять."); return; }

        var last = Inventory.SalesHistory.Peek();
        System.Console.WriteLine("Последняя продажа:");
        System.Console.WriteLine(last.ToString());
        System.Console.Write("Подтвердите отмену (y/N): ");
        string conf = (System.Console.ReadLine() ?? "").Trim().ToLowerInvariant();
        if (!(conf == "y" || conf == "yes")) { System.Console.WriteLine("Отмена отменена."); return; }

        if (Inventory.UndoLastSale(out string err))
        {
            System.Console.WriteLine("Последняя продажа успешно отменена. Количество товара возвращено на склад.");
        }
        else
        {
            System.Console.WriteLine("Не удалось отменить последнюю продажу: " + err);
        }
    }

    private static void UiShowSalesReport()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Отчёт о продажах (агрегация по товарам):");
        var report = Inventory.GetSalesReport();
        if (report.Count == 0) { System.Console.WriteLine("Нет завершённых продаж."); return; }

        System.Console.WriteLine("Код | Наименование | Всего штук | Доход (руб)");
        decimal grandTotal = 0;
        int grandQty = 0;
        foreach (var item in report)
        {
            System.Console.WriteLine($"{item.ProductCode} | {item.ProductName} | {item.TotalQuantity} | {item.TotalRevenue:0.00}");
            grandTotal += item.TotalRevenue;
            grandQty += item.TotalQuantity;
        }
        System.Console.WriteLine();
        System.Console.WriteLine($"Итого продано штук: {grandQty}  |  Общий доход: {grandTotal:0.00} руб");
    }
}
