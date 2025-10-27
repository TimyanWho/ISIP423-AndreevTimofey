using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoServicePr7
{
    #region Перечисления
    public enum PartCategory
    {
        Engine,
        Transmission,
        Electrical,
        Suspension,
        Brake,
        Body,
        Other
    }

    public enum RepairStatus
    {
        Pending,
        InProgress,
        Completed,
        Refused,
        Failed
    }
    #endregion

    #region Модели / Сущности
    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PartCategory Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public bool IsAvailable(int needed) => Quantity >= needed;

        // Конструкторы по умолчанию допустимы
    }

    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public List<Car> Cars { get; set; } = new List<Car>();
    }

    public class Car
    {
        public int Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public int Year { get; set; }

        public int ClientId { get; set; }
        public Client Owner { get; set; }
    }

    public class Mechanic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }

        public void StartRepair(RepairOrder order) { }
        public void FinishRepair(RepairOrder order) { }
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
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }

        public Dictionary<int, int> OrderedItems { get; set; } = new Dictionary<int, int>();

        public int ArrivesAfterClients { get; set; }

        public int PlacedAtClientCounter { get; set; }

        public bool IsDelivered { get; set; }
    }

    #endregion

    #region Логика сервиса (каркас)

    public class AutoService
    {
        public decimal Balance { get; private set; }

        public List<Part> Inventory { get; private set; } = new List<Part>();

        public Queue<RepairOrder> ClientQueue { get; private set; } = new Queue<RepairOrder>();

        public List<RepairOrder> OrdersHistory { get; private set; } = new List<RepairOrder>();

        public List<PurchaseOrder> IncomingPurchases { get; private set; } = new List<PurchaseOrder>();

        public int ServedClientsCounter { get; private set; }

        public AutoService(decimal startingBalance)
        {
            Balance = startingBalance;
        }

        public void EnqueueClient(RepairOrder order)
        {
            // Добавить проверку корректности order
            // ClientQueue.Enqueue(order);
        }

        // Обработать следующего клиента в очереди
        public void ProcessNextClient()
        {
            // Попытка выполнить ремонт, проверка наличия детали, списание/пополнение баланса,
            // начисление штрафа при неправильной замене и т.д.
        }

        // Покупка деталей у поставщика
        public void PlacePurchaseOrder(PurchaseOrder po)
        {
            // Списать деньги с баланса, сохранить заказ в IncomingPurchases
            // и отметить PlacedAtClientCounter = ServedClientsCounter
        }

        // Проверить и доставить пришедшие поставки (вызывать после обслуживания клиента)
        public void CheckAndDeliverPurchases()
        {
            // Перебрать IncomingPurchases и доставить, если условие выполнено
        }

        // Проверка наличия требуемой детали
        public bool HasPart(int partId, int quantity = 1)
        {
            return Inventory.Any(p => p.Id == partId && p.Quantity >= quantity);
        }

        // Списать деталь со склада
        public void ConsumePart(int partId, int quantity = 1)
        {
            // Найти в Inventory и уменьшить Quantity
        }

        // Приобрести (пополнить) склад напрямую (внутренний метод, не то же самое что PlacePurchaseOrder)
        public void ReceiveParts(int partId, int quantity)
        {
            // Добавить в Inventory
        }

        // Отказать клиенту (и начислить штраф)
        public void RefuseClient(RepairOrder order)
        {
            // Логика штрафа
        }

        // Вспомогательная: валидация вводимых данных
        public static void ValidateNotEmpty(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{fieldName} не может быть пустым");
        }

        public static void ValidatePositive(decimal number, string fieldName)
        {
            if (number < 0) throw new ArgumentException($"{fieldName} не может быть отрицательным");
        }
    }

    #endregion

    #region Слой доступа к данным (шаблон для EF)

    // Пример простого DbContext для Entity Framework 6 / EF Core
    // В реальном проекте вы подставите своё имя контекста и строку подключения

    // Если у вас .NET Framework + EF6, используйте System.Data.Entity.DbContext
    // Для EF Core корректный using и пакеты отличаются.

    // Ниже — универсальный шаблон. Подставьте нужную реализацию в зависимости
    // от выбранного варианта (Database First или Code First).

    // Приведённый класс — чистый шаблон и служит для декомпозиции.
    public class AutoServiceContext /* : DbContext */
    {
        // public DbSet<Part> Parts { get; set; }
        // public DbSet<Client> Clients { get; set; }
        // public DbSet<Car> Cars { get; set; }
        // public DbSet<RepairOrder> RepairOrders { get; set; }
        // public DbSet<Mechanic> Mechanics { get; set; }
        // public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        // Конструктор контекста и прочие настройки подключения к БД
        // public AutoServiceContext() : base("name=YourConnectionStringName") { }
    }

    // Класс Core, который удобно использовать как глобальный доступ к контексту
    public static class Core
    {
        // В зависимости от выбранного подхода к БД раскомментируйте нужную строку.
        // Примеры из методички:
        // public static BDGorlanov719Entities Context = new BDGorlanov719Entities(); // Database First (EF6)
        // public static BDGorlanov719Context Context = new BDGorlanov719Context(); // EF Core

        // Здесь — шаблон, чтобы вы добавили своё имя контекста
        // public static AutoServiceContext Context = new AutoServiceContext();
    }

    #endregion

    #region Рекомендации по коммитам (коротко)
    // 1) "Декомпозиция: Добавлены сущности Part, Client, Car" — отправить после создания файлов моделей
    // 2) "Декомпозиция: Добавлен AutoService каркас и методы" — после добавления логики игры (скелет)
    // 3) "Добавлен Core и заготовка DbContext" — после добавления слоя доступа к данным
    // 4) Далее по фичам: "Реализована покупка деталей", "Реализован процесс ремонта" и т.д.
    #endregion
}


/*
    5 вопросов декомпозиции (коротко):

    1) Какие существительные в задаче? -> классы/сущности
       Student/Teacher/Course (по примеру в задании), а для этой игры:
       Client, Car, Part (запчасть), RepairOrder (заказ на ремонт), Mechanic,
       AutoService (сам сервис), PurchaseOrder (заказ на поставку).

    2) Какие глаголы в задаче? -> методы/операции
       принять клиента, починить (заменить деталь), отказать, купить детали,
       доставить заказ (через 2 машины), начислить штраф, начислить оплату.

    3) Какие данные всегда вместе? -> группировка в классы
       у клиента: имя + контакты + его машина(ы);
       у детали: имя + цена + количество;
       у заказа: клиент + машина + требуемая деталь + стоимость работ.

    4) Что может существовать отдельно? -> отдельные классы
       Курс может существовать без студентов — аналогично, PurchaseOrder
       (заказ у поставщика) может быть вне очереди клиентов.

    5) Что повторяется в разных местах? -> вынести в отдельный метод
       проверка корректности ввода (валидация), подсчёт итоговой стоимости,
       проверка наличия детали на складе.
   */
