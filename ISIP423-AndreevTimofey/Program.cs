using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

bool flag = true;
string text = "fa";
string command = " ";
List<List<String>> History = new List<List<String>>();
while (command != "0")
{
    Console.WriteLine("Введите команду:");
    Console.WriteLine("1. Работать");
    Console.WriteLine("2. Посмотреть статистику");
    Console.WriteLine("0. Уйти");
    command = Console.ReadLine();
    if (command == "1")
    {
        Console.WriteLine("Введите текст на русском(!):");
        flag = true;
        while (flag)
        {
            flag = false;
            text = Console.ReadLine();
            if (text.Length < 3) flag = true;
        }
        List<String> Record = new List<String>();
        Record.Add(text);
        string[] words = text.Replace(".", string.Empty).Replace(",", string.Empty).Replace("!", string.Empty).Replace("?", string.Empty).Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        Console.WriteLine($"Кол-во слов: {words.Length}");
        Record.Add($"Кол-во слов: {words.Length}");
        int minl = words[0].Length;
        string minw = words[0];
        int maxl = words[0].Length;
        string maxw = words[0];
        foreach (string word in words)
        {
            if (word.Length < minl)
            {
                minl = word.Length;
                minw = word;
            }
            if (word.Length > maxl)
            {
                maxl = word.Length;
                maxw = word;
            }
        }
        Console.WriteLine($"Самое короткое слово: {minw}({minl})");
        Record.Add($"Самое короткое слово: {minw}({minl})");

        string[] sent = text.Split(new char[] { '?', '.', '!' }, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"Кол-во предложений: {sent.Length}");
        Record.Add($"Кол-во предложений: {sent.Length}");

        List<char> glas = new List<char>() { 'а', 'ы', 'е', 'у', 'и', 'о', 'э', 'ё', 'я', 'ю' };
        List<char> sogl = new List<char>() { 'с', 'т', 'п', 'н', 'м', 'б', 'в', 'г', 'д', 'п', 'р', 'к', 'л', 'ф', 'ш', 'щ', 'ч', 'й', 'х', 'ъ', 'ь', 'ц', 'з' };

        int glcnt = 0, sglcnt = 0;
        foreach (char l in text.ToLower())
        {
            if (glas.Contains(l)) glcnt++;
            if (sogl.Contains(l)) sglcnt++;
        }
        Console.WriteLine($"Гласных: {glcnt}");
        Record.Add($"Гласных: {glcnt}");
        Console.WriteLine($"Согласных: {sglcnt}");
        Record.Add($"Согласных: {sglcnt}");

        Console.WriteLine($"Самое длинное слово: {maxw}({maxl})");
        Record.Add($"Самое длинное слово: {maxw}({maxl})");
        double all_let = 0;
        List<char> all = glas;
        all.AddRange(sogl);
        foreach (var a in all)
        {
            if (text.ToLower().Count(f => f == a) != 0) all_let += text.ToLower().Count(f => f == a);
        }
        foreach (var a in all)
        {
            if (text.ToLower().Count(f => f == a) != 0) Console.WriteLine($"Буквы {a}: {text.ToLower().Count(f => f == a)} | Встречаемость: {Math.Round(Convert.ToDouble(text.ToLower().Count(f => f == a)) / all_let * 100, 2)}%");
            Record.Add($"Буквы {a}: {text.ToLower().Count(f => f == a)} | Встречаемость: {Math.Round(Convert.ToDouble(text.ToLower().Count(f => f == a)) / all_let * 100, 2)}%");
        }
        History.Add(Record);
    }else if (command == "2")
    {
        int cnt = 1;
        foreach(var record in History)
        {
            Console.WriteLine($"Текст #{cnt}");
            foreach (var line in record)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine();
            cnt++;
        }
    }
}