System.Console.WriteLine("Подсчёт потраченных за день (рубли)");

int ReadCount()
{
    while (true)
    {
        System.Console.Write("Введите количество операций (2-40): ");
        string s = System.Console.ReadLine();
        if (int.TryParse(s, out int n) && n >= 2 && n <= 40) return n;
        System.Console.WriteLine("Ошибка. Введите целое число от 2 до 40.");
    }
}

int n = ReadCount();
string[] names = new string[n];
decimal[] prices = new decimal[n];

for (int i = 0; i < n; i++)
{
    while (true)
    {
        System.Console.Write($"Запись {i + 1}/{n}: ");
        string line = System.Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) { System.Console.WriteLine("Пустая строка."); continue; }
        int pos = line.IndexOf(';');
        if (pos < 0) { System.Console.WriteLine("Нужно разделять ';'"); continue; }
        string name = line.Substring(0, pos).Trim();
        string amountPart = line.Substring(pos + 1).Trim().Replace(',', '.');
        if (string.IsNullOrEmpty(name)) { System.Console.WriteLine("Название не может быть пустым."); continue; }
        if (decimal.TryParse(amountPart, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal val))
        {
            if (val < 0) { System.Console.WriteLine("Сумма не может быть отрицательной."); continue; }
            names[i] = name;
            prices[i] = System.Decimal.Round(val, 2);
            break;
        }
        else
        {
            System.Console.WriteLine("Не удалось распознать сумму. Пример: 235 или 235.50");
        }
    }
}

decimal Sum(decimal[] arr)
{
    decimal s = 0;
    for (int i = 0; i < arr.Length; i++) s += arr[i];
    return s;
}

void PrintAll()
{
    System.Console.WriteLine();
    System.Console.WriteLine("Записи:");
    for (int i = 0; i < n; i++)
        System.Console.WriteLine($"{i + 1}. {names[i]} — {prices[i]:0.00} руб");
    System.Console.WriteLine($"Итого: {Sum(prices):0.00} руб");
}

void ShowStats()
{

}

void BubbleSort()
{

}

void ConvertCurrency()
{

}


void SearchByName()
{

}

while (true)
{
    System.Console.WriteLine();
    System.Console.WriteLine("Меню:");
    System.Console.WriteLine("1. Вывод данных");
    System.Console.WriteLine("2. Статистика");
    System.Console.WriteLine("3. Сортировка по цене (пузырёк)");
    System.Console.WriteLine("4. Конвертация валюты");
    System.Console.WriteLine("5. Поиск по названию");
    System.Console.WriteLine("0. Выход");
    System.Console.Write("Выберите пункт: ");
    string choice = System.Console.ReadLine()?.Trim();

    if (choice == "1") PrintAll();
    else if (choice == "2") ShowStats();
    else if (choice == "3") BubbleSort();
    else if (choice == "4") ConvertCurrency();
    else if (choice == "5") SearchByName();
    else if (choice == "0") break;
    else System.Console.WriteLine("Неверный пункт.");

    System.Console.WriteLine("Нажмите Enter для продолжения...");
    System.Console.ReadLine();
}

System.Console.WriteLine("Bye bye");
