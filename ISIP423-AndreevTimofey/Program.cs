using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoService
{
    public enum PartCondition
    {
        New,
        Used
    }

    public enum RepairStatus
    {
        Pending,
        InProgress,
        Completed,
        Refused,
        Failed
    }

    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public List<Car> Cars { get; set; } = new List<Car>();
    }

    public class Car
    {
        public int Id { get; set; }
        public string VIN { get; set; } = "";
        public string Model { get; set; } = "";
        public int Year { get; set; }
        public int ClientId { get; set; } // FK
    }

    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; } // price for repair part cost calculation
        public string Manufacturer { get; set; } = "";
        public PartCondition Condition { get; set; } = PartCondition.New;
    }

    public class InventoryItem
    {
        public int Id { get; set; }
        public int PartId { get; set; } // FK
        public int Quantity { get; set; }
        public DateTime LastRestockDate { get; set; }
    }

    public class RepairOrder
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int ClientId { get; set; }
        public int AssignedMechanicId { get; set; }
        public RepairStatus Status { get; set; } = RepairStatus.Pending;
        public decimal LaborCost { get; set; }
        public decimal TotalCost { get; set; } // computed: part price + labor
        public DateTime CreatedAt { get; set; }
        public List<RepairOrderPart> Parts { get; set; } = new List<RepairOrderPart>();
    }

    public class RepairOrderPart
    {
        public int Id { get; set; }
        public int RepairOrderId { get; set; }
        public int PartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // snapshot of part price at time of repair
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public List<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
        public int ExpectedArrivalAfterVehicles { get; set; } // store delay: arrives after N vehicles
    }

    public enum PurchaseOrderStatus
    {
        Ordered,
        Arrived,
        Cancelled
    }

    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int PartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Role { get; set; } = ""; // e.g. "Mechanic", "Manager"
    }

    public class TransactionRecord
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public decimal Amount { get; set; } // positive = income, negative = expense
        public string Description { get; set; } = "";
    }

    public class InventoryManager
    {
        public InventoryManager() { }

        public int GetQuantity(int partId)
        {
            return 0;
        }

        public void RemovePart(int partId, int count)
        {
        }

        public void AddPart(int partId, int count)
        {
        }

        public bool HasPart(int partId, int requiredQuantity)
        {
            return false;
        }
    }

    public class RepairService
    {
        public RepairService() { }

        public RepairOrder CreateRepairOrder(int clientId, int carId, int brokenPartId, decimal laborCost)
        {
            return new RepairOrder();
        }

        public void PerformRepair(RepairOrder order)
        {
        }

        public void RefuseRepair(RepairOrder order)
        {
        }
    }

    public class PurchaseService
    {
        public PurchaseService() { }

        // Создать заказ на покупку запчастей — детали начислятся с задержкой (через N машин)
        public PurchaseOrder CreatePurchaseOrder(List<PurchaseOrderItem> items, int arriveAfterVehicles)
        {
            // пустой
            return new PurchaseOrder();
        }

        // Обработать приход (меняется статус и добавляются в Inventory)
        public void ProcessArrival(PurchaseOrder po)
        {
            // пустой
        }
    }

    public class FinanceService
    {
        public decimal Balance { get; private set; }

        public FinanceService(decimal initial)
        {
            Balance = initial;
        }

        public void AddIncome(decimal amount, string description)
        {
            // пустой
        }

        public void AddExpense(decimal amount, string description)
        {
            // пустой
        }
    }

    // ========== Репозитории (Абстракции) ==========
    public interface IRepository<T>
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    // Пример репозитория для Part (реализацию позже)
    public class PartRepository : IRepository<Part>
    {
        public void Add(Part item) { }
        public void Delete(int id) { }
        public IEnumerable<Part> GetAll() { return Enumerable.Empty<Part>(); }
        public Part GetById(int id) { return null; }
        public void Update(Part item) { }
    }

    // ========== Утилиты валидации ==========
    public static class Validator
    {
        public static bool IsValidString(string s) => !string.IsNullOrWhiteSpace(s);

        public static bool IsPositive(decimal value) => value >= 0m;

        public static bool IsPositiveInt(int value) => value >= 0;
    }

    // ========== Игровой движок (управление потоком) ==========
    public class GameEngine
    {
        public InventoryManager Inventory { get; private set; }
        public RepairService RepairService { get; private set; }
        public PurchaseService PurchaseService { get; private set; }
        public FinanceService Finance { get; private set; }

        public GameEngine()
        {
            Inventory = new InventoryManager();
            RepairService = new RepairService();
            PurchaseService = new PurchaseService();
            Finance = new FinanceService(10000m); // пример начального баланса
        }

        public void Run()
        {
            // главный цикл игры — реализуется позже
        }
    }

    // ========== Program ==========
    class Program
    {
        static void Main(string[] args)
        {
            // Инициализация и запуск движка
            var engine = new GameEngine();
            engine.Run();
        }
    }
}
