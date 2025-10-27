// Практическая работа №7 — Декомпозиция и рабочее меню (C#)
// Файл: Pr7_Decomposition_and_Skeleton.cs
// Описание: обновлённый каркас — все сущности + реализованы основные
// методы AutoService (in-memory). Добавлено консольное меню в Main для
// управления сервисом и тестирования логики. Совместимо с C# 7.3.

using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoServicePr7
{
    public enum PartCategory { Engine = 0, Transmission = 1, Electrical = 2, Suspension = 3, Brake = 4, Body = 5, Other = 6 }
    public enum RepairStatus { Pending = 0, InProgress = 1, Completed = 2, Refused = 3, Failed = 4 }

    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PartCategory Category { get; set; }
        public decimal Price { get; set; }

        public Part()
        {
            Name = string.Empty;
            Category = PartCategory.Other;
            Price = 0m;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Name не может быть пустым");
            if (Price < 0) throw new ArgumentException("Price не может быть отрицательной");
        }
    }

    public class InventoryEntry
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public Part Part { get; set; }
        public int Quantity { get; set; }

        public InventoryEntry()
        {
            Part = new Part();
            Quantity = 0;
        }

        public void Validate()
        {
            if (Quantity < 0) throw new ArgumentException("Quantity не может быть отрицательной");
        }
    }

    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<Car> Cars { get; set; }

        public Client()
        {
            FullName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Cars = new List<Car>();
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(FullName)) throw new ArgumentException("FullName не может быть пустым");
        }
    }

    public class Car
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Owner { get; set; }
        public string Model { get; set; }
        public string VIN { get; set; }
        public int? Year { get; set; }

        public Car()
        {
            Owner = new Client();
            Model = string.Empty;
            VIN = string.Empty;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Model)) throw new ArgumentException("Model не может быть пустым");
        }
    }

    public class Mechanic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal HourlyRate { get; set; }

        public Mechanic()
        {
            Name = string.Empty;
            HourlyRate = 0m;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Name не может быть пустым");
            if (HourlyRate < 0) throw new ArgumentException("HourlyRate не может быть отрицательным");
        }
    }

    public class RepairOrder
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int? RequiredPartId { get; set; }
        public Part RequiredPart { get; set; }
        public decimal LaborCost { get; set; }
        public decimal PartCost { get; set; }
        public decimal TotalCost { get; set; }
        public RepairStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public RepairOrder()
        {
            Client = new Client();
            Car = new Car();
            RequiredPartId = null;
            RequiredPart = new Part();
            LaborCost = 0m;
            PartCost = 0m;
            TotalCost = 0m;
            Status = RepairStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Validate()
        {
            if (ClientId <= 0) throw new ArgumentException("ClientId должен быть положительным");
            if (CarId <= 0) throw new ArgumentException("CarId должен быть положительным");
            if (LaborCost < 0) throw new ArgumentException("LaborCost не может быть отрицательной");
            if (PartCost < 0) throw new ArgumentException("PartCost не может быть отрицательной");
        }
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }
        public int PlacedAtClientCounter { get; set; }
        public int ArrivesAfterClients { get; set; }
        public bool IsDelivered { get; set; }
        public Dictionary<int, int> Items { get; set; }
        public decimal TotalCost { get; set; }

        public PurchaseOrder()
        {
            PlacedAtClientCounter = 0;
            ArrivesAfterClients = 2;
            IsDelivered = false;
            Items = new Dictionary<int, int>();
            TotalCost = 0m;
        }

        public void Validate()
        {
            if (ArrivesAfterClients < 0) throw new ArgumentException("ArrivesAfterClients не может быть отрицательным");
            if (Items == null || Items.Count == 0) throw new ArgumentException("Items не может быть пустым");
            foreach (var kv in Items)
            {
                if (kv.Key <= 0) throw new ArgumentException("PartId должен быть положительным");
                if (kv.Value <= 0) throw new ArgumentException("Quantity должен быть положительным");
            }
            if (TotalCost < 0) throw new ArgumentException("TotalCost не может быть отрицательной");
        }
    }

    public class AutoService
    {
        public decimal Balance { get; private set; }
        public List<InventoryEntry> Inventory { get; private set; }
        public Queue<RepairOrder> ClientQueue { get; private set; }
        public List<RepairOrder> OrdersHistory { get; private set; }
        public List<PurchaseOrder> IncomingPurchases { get; private set; }
        public int ServedClientsCounter { get; private set; }
        public decimal RefusalPenaltyPercent { get; set; }

        // Справочники (в реальной интеграции — данные из БД)
        public List<Part> PartsCatalog { get; private set; }
        public List<Client> Clients { get; private set; }
        public List<Mechanic> Mechanics { get; private set; }

        public AutoService(decimal startingBalance)
        {
            if (startingBalance < 0) throw new ArgumentException("startingBalance не может быть отрицательным");
            Balance = startingBalance;
            Inventory = new List<InventoryEntry>();
            ClientQueue = new Queue<RepairOrder>();
            OrdersHistory = new List<RepairOrder>();
            IncomingPurchases = new List<PurchaseOrder>();
            ServedClientsCounter = 0;
            RefusalPenaltyPercent = 0.5m;

            PartsCatalog = new List<Part>();
            Clients = new List<Client>();
            Mechanics = new List<Mechanic>();
        }

        // Декомпозиция — публичные методы реализованы простым, но корректным образом

        public void EnqueueClient(RepairOrder order)
        {
            if (order == null) throw new ArgumentNullException("order");
            order.Validate();
            order.Status = RepairStatus.Pending;
            ClientQueue.Enqueue(order);
        }

        public void ProcessNextClient()
        {
            if (ClientQueue.Count == 0)
            {
                Console.WriteLine("Нет клиентов в очереди.");
                return;
            }

            var order = ClientQueue.Dequeue();
            Console.WriteLine($"Обрабатываем заказ #{order.Id} для клиента {order.Client.FullName} (машина: {order.Car.Model})");

            // Если нужна запчасть
            if (order.RequiredPartId.HasValue && order.RequiredPartId.Value > 0)
            {
                if (HasPart(order.RequiredPartId.Value, 1))
                {
                    // Успешный ремонт
                    ConsumePart(order.RequiredPartId.Value, 1);
                    Balance += order.TotalCost;
                    order.Status = RepairStatus.Completed;
                    Console.WriteLine($"Ремонт выполнен. Получено {order.TotalCost} ₽. Баланс: {Balance} ₽");
                }
                else
                {
                    // Нет нужной детали — по условию выберем отказ и начислим штраф
                    RefuseClient(order);
                }
            }
            else
            {
                // Работа без детали — просто начисляем оплату
                Balance += order.TotalCost;
                order.Status = RepairStatus.Completed;
                Console.WriteLine($"Работа выполнена (без замены деталей). Получено {order.TotalCost} ₽. Баланс: {Balance} ₽");
            }

            OrdersHistory.Add(order);
            ServedClientsCounter++;
            CheckAndDeliverPurchases();
        }

        public void PlacePurchaseOrder(PurchaseOrder po)
        {
            if (po == null) throw new ArgumentNullException("po");
            po.Validate();

            // Посчитаем стоимость по текущему каталогу
            decimal total = 0m;
            foreach (var kv in po.Items)
            {
                var part = PartsCatalog.FirstOrDefault(p => p.Id == kv.Key);
                if (part == null) throw new InvalidOperationException("Указанная деталь не найдена в каталоге");
                total += part.Price * kv.Value;
            }

            if (total > Balance) throw new InvalidOperationException("Недостаточно средств для размещения заказа");

            Balance -= total;
            po.TotalCost = total;
            po.PlacedAtClientCounter = ServedClientsCounter;
            po.IsDelivered = false;

            IncomingPurchases.Add(po);
            Console.WriteLine($"Заказ поставщика размещён на сумму {total} ₽. Баланс: {Balance} ₽");
        }

        public void CheckAndDeliverPurchases()
        {
            var toDeliver = IncomingPurchases.Where(x => !x.IsDelivered && (ServedClientsCounter - x.PlacedAtClientCounter) >= x.ArrivesAfterClients).ToList();
            foreach (var po in toDeliver)
            {
                foreach (var kv in po.Items)
                {
                    ReceiveParts(kv.Key, kv.Value);
                }
                po.IsDelivered = true;
                Console.WriteLine($"Поставка #{po.Id} доставлена (через {po.ArrivesAfterClients} клиентов)");
            }
        }

        public bool HasPart(int partId, int quantity = 1)
        {
            var entry = Inventory.FirstOrDefault(i => i.PartId == partId);
            return entry != null && entry.Quantity >= quantity;
        }

        public void ConsumePart(int partId, int quantity = 1)
        {
            var entry = Inventory.FirstOrDefault(i => i.PartId == partId);
            if (entry == null) throw new InvalidOperationException("Деталь не найдена на складе");
            if (entry.Quantity < quantity) throw new InvalidOperationException("Недостаточно деталей на складе");
            entry.Quantity -= quantity;
        }

        public void ReceiveParts(int partId, int quantity)
        {
            var entry = Inventory.FirstOrDefault(i => i.PartId == partId);
            if (entry == null)
            {
                var part = PartsCatalog.FirstOrDefault(p => p.Id == partId);
                if (part == null) throw new InvalidOperationException("Деталь не найдена в каталоге при доставке");
                entry = new InventoryEntry { PartId = partId, Part = part, Quantity = quantity };
                Inventory.Add(entry);
            }
            else
            {
                entry.Quantity += quantity;
            }
        }

        public void RefuseClient(RepairOrder order)
        {
            // Наказание за отказ: штраф равен проценту от стоимости заказа
            var penalty = order.TotalCost * RefusalPenaltyPercent;
            Balance -= penalty;
            order.Status = RepairStatus.Refused;
            Console.WriteLine($"Клиенту отказано. Штраф: {penalty} ₽. Баланс: {Balance} ₽");
        }

        public IEnumerable<InventoryEntry> GetInventory() { return Inventory; }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var svc = new AutoService(10000m);
            SeedData(svc);
            RunMenu(svc);
        }

        private static void SeedData(AutoService svc)
        {
            // Parts catalog
            svc.PartsCatalog.Add(new Part { Id = 1, Name = "Фильтр масляный", Category = PartCategory.Other, Price = 15.50m });
            svc.PartsCatalog.Add(new Part { Id = 2, Name = "Фильтр воздушный", Category = PartCategory.Other, Price = 20.00m });
            svc.PartsCatalog.Add(new Part { Id = 3, Name = "Тормозная колодка (пара)", Category = PartCategory.Brake, Price = 45.00m });
            svc.PartsCatalog.Add(new Part { Id = 4, Name = "Свеча зажигания", Category = PartCategory.Electrical, Price = 8.00m });
            svc.PartsCatalog.Add(new Part { Id = 5, Name = "Ремень ГРМ", Category = PartCategory.Engine, Price = 120.00m });

            // Inventory
            svc.Inventory.Add(new InventoryEntry { Id = 1, PartId = 1, Part = svc.PartsCatalog.First(p => p.Id == 1), Quantity = 5 });
            svc.Inventory.Add(new InventoryEntry { Id = 2, PartId = 2, Part = svc.PartsCatalog.First(p => p.Id == 2), Quantity = 3 });
            svc.Inventory.Add(new InventoryEntry { Id = 3, PartId = 3, Part = svc.PartsCatalog.First(p => p.Id == 3), Quantity = 10 });
            svc.Inventory.Add(new InventoryEntry { Id = 4, PartId = 4, Part = svc.PartsCatalog.First(p => p.Id == 4), Quantity = 20 });
            svc.Inventory.Add(new InventoryEntry { Id = 5, PartId = 5, Part = svc.PartsCatalog.First(p => p.Id == 5), Quantity = 1 });

            // Clients & Cars
            var client = new Client { Id = 1, FullName = "Иванов Иван", Email = "ivanov@example.com", Phone = "+7-900-000-0000" };
            var car = new Car { Id = 1, ClientId = 1, Owner = client, Model = "Lada Vesta", VIN = "X1Y2Z3VINEX", Year = 2018 };
            client.Cars.Add(car);
            svc.Clients.Add(client);

            // Mechanic
            svc.Mechanics.Add(new Mechanic { Id = 1, Name = "Петров Сергей", HourlyRate = 800m });

            Console.WriteLine("Сеанс инициализирован: добавлены справочники (Parts, Inventory, Clients, Mechanics)");
        }

        private static void RunMenu(AutoService svc)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== AutoService Menu ===");
                Console.WriteLine("1) Показать склад");
                Console.WriteLine("2) Показать каталог запчастей");
                Console.WriteLine("3) Показать клиентов");
                Console.WriteLine("4) Добавить заказ (клиент)");
                Console.WriteLine("5) Обработать следующего клиента");
                Console.WriteLine("6) Разместить заказ у поставщика");
                Console.WriteLine("7) Показать ожидаемые поставки");
                Console.WriteLine("8) Показать баланс");
                Console.WriteLine("0) Выход");
                Console.Write("Выберите действие: ");
                var line = Console.ReadLine();
                int choice;
                if (!int.TryParse(line, out choice)) { Console.WriteLine("Неверный ввод"); continue; }

                try
                {
                    switch (choice)
                    {
                        case 1: ShowInventory(svc); break;
                        case 2: ShowCatalog(svc); break;
                        case 3: ShowClients(svc); break;
                        case 4: CreateOrderInteractive(svc); break;
                        case 5: svc.ProcessNextClient(); break;
                        case 6: CreatePurchaseOrderInteractive(svc); break;
                        case 7: ShowIncomingPurchases(svc); break;
                        case 8: Console.WriteLine($"Баланс: {svc.Balance} ₽"); break;
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

        private static void ShowInventory(AutoService svc)
        {
            Console.WriteLine("Склад:");
            foreach (var e in svc.GetInventory())
            {
                Console.WriteLine($"PartId={e.PartId} Name={e.Part.Name} Qty={e.Quantity} Price={e.Part.Price}");
            }
        }

        private static void ShowCatalog(AutoService svc)
        {
            Console.WriteLine("Каталог запчастей:");
            foreach (var p in svc.PartsCatalog)
            {
                Console.WriteLine($"Id={p.Id} Name={p.Name} Price={p.Price}");
            }
        }

        private static void ShowClients(AutoService svc)
        {
            Console.WriteLine("Клиенты:");
            foreach (var c in svc.Clients)
            {
                Console.WriteLine($"Id={c.Id} Name={c.FullName} Cars={c.Cars.Count}");
                foreach (var car in c.Cars) Console.WriteLine($"  CarId={car.Id} Model={car.Model} VIN={car.VIN}");
            }
        }

        private static void CreateOrderInteractive(AutoService svc)
        {
            Console.Write("Введите Id клиента (например 1): ");
            var s = Console.ReadLine(); int cid; if (!int.TryParse(s, out cid)) { Console.WriteLine("Неверно"); return; }
            var client = svc.Clients.FirstOrDefault(c => c.Id == cid);
            if (client == null) { Console.WriteLine("Клиент не найден"); return; }
            if (client.Cars.Count == 0) { Console.WriteLine("У клиента нет машин"); return; }
            var car = client.Cars[0];

            Console.WriteLine("Выберите Id детали из каталога (введите 0, если ремонт без замены детали): ");
            ShowCatalog(svc);
            s = Console.ReadLine(); int pid; if (!int.TryParse(s, out pid)) { Console.WriteLine("Неверно"); return; }
            int? reqPartId = null; Part part = null;
            decimal partCost = 0m;
            if (pid != 0)
            {
                part = svc.PartsCatalog.FirstOrDefault(p => p.Id == pid);
                if (part == null) { Console.WriteLine("Деталь не найдена"); return; }
                reqPartId = pid;
                partCost = part.Price;
            }

            Console.Write("Введите стоимость работы (labor), например 500: ");
            s = Console.ReadLine(); decimal labor; if (!decimal.TryParse(s, out labor)) { Console.WriteLine("Неверно"); return; }
            if (labor < 0) { Console.WriteLine("Labor не может быть отрицательной"); return; }

            var order = new RepairOrder
            {
                Id = svc.OrdersHistory.Count + svc.ClientQueue.Count + 1,
                ClientId = client.Id,
                Client = client,
                CarId = car.Id,
                Car = car,
                RequiredPartId = reqPartId,
                RequiredPart = (part == null ? new Part() : part),
                LaborCost = labor,
                PartCost = partCost,
                TotalCost = labor + partCost,
                Status = RepairStatus.Pending
            };

            svc.EnqueueClient(order);
            Console.WriteLine($"Заказ добавлен в очередь. ID={order.Id} TotalCost={order.TotalCost}");
        }

        private static void CreatePurchaseOrderInteractive(AutoService svc)
        {
            var po = new PurchaseOrder();
            Console.WriteLine("Создание заказа поставки. Введите пары PartId:Quantity (в строку через запятую), например: 1:5,3:2");
            ShowCatalog(svc);
            Console.Write("Ввод: ");
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) { Console.WriteLine("Пустой ввод"); return; }
            var pairs = line.Split(',');
            foreach (var pair in pairs)
            {
                var kv = pair.Split(':');
                if (kv.Length != 2) { Console.WriteLine("Неверный формат пары"); return; }
                int pid; int qty;
                if (!int.TryParse(kv[0].Trim(), out pid) || !int.TryParse(kv[1].Trim(), out qty)) { Console.WriteLine("Неверные числа"); return; }
                po.Items[pid] = qty;
            }

            Console.Write("Через сколько клиентов доставить? (по умолчанию 2): ");
            var s = Console.ReadLine(); int arr;
            if (!int.TryParse(s, out arr)) arr = 2;
            po.ArrivesAfterClients = arr;

            // Попробуем разместить заказ
            svc.PlacePurchaseOrder(po);
            Console.WriteLine("Заказ у поставщика размещён.");
        }

        private static void ShowIncomingPurchases(AutoService svc)
        {
            Console.WriteLine("Ожидаемые поставки:");
            foreach (var po in svc.IncomingPurchases)
            {
                Console.WriteLine($"PO Id={po.Id} PlacedAt={po.PlacedAtClientCounter} ArrivesAfter={po.ArrivesAfterClients} Delivered={po.IsDelivered} TotalCost={po.TotalCost}");
            }
        }
    }
}
