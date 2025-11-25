using ConsoleTwin.Utils;
using System;

namespace ConsoleTwin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var game = new Game();
            game.Run();
        }
    }
}

// ---------------- File: Game.cs ----------------
using System;
using System.Collections.Generic;
using ConsoleTwin.Utils;

namespace ConsoleTwin
{
    class Game
    {
        private int maxTurnsToWin;
        public Player Player { get; private set; }
        private int turn = 0;
        private Random rng = new Random();
        private List<string> enemyPool = new List<string> { "Goblin twin", "Skeleton twin", "Mage twin", "Slime twin" };
        private List<Func<Enemy>> bosses;

        public Game()
        {
            Player = new Player(maxHp: 100, baseAttack: 5);
            Player.EquipWeapon(new Weapon("Короткий меч twin", 8));
            Player.EquipArmor(new Armor("Кожаная броня twin", 4));

            bosses = new List<Func<Enemy>> {
                () => BossFactory.CreateGoblinBoss(),
                () => BossFactory.CreateSkeletonBoss(),
                () => BossFactory.CreateMageBoss(),
                () => BossFactory.CreateBossRyan(),
                () => BossFactory.CreatePestovBoss()
            };
            maxTurnsToWin = rng.Next(10, 31);
        }

        public void Run()
        {
            PrintIntro();
            while (Player.IsAlive && turn < maxTurnsToWin)
            {
                turn++;
                Console.WriteLine($"\n--- Ход {turn} twin ---");

                if (turn % 10 == 0)
                {
                    Console.WriteLine("Появился босс twin!");
                    var boss = bosses[rng.Next(bosses.Count)]();
                    Fight(boss);
                    if (!Player.IsAlive) break;
                    continue;
                }

                bool chest = rng.NextDouble() < 0.5;
                if (chest)
                {
                    Console.WriteLine("Вы находите сундук twin!");
                    OpenChest();
                }
                else
                {
                    var enemyType = enemyPool[rng.Next(enemyPool.Count)];
                    Enemy enemy = EnemyFactory.CreateEnemy(enemyType);
                    Console.WriteLine($"Twin! Вас атакует {enemy.Name}!");
                    Fight(enemy);
                    if (!Player.IsAlive) break;
                }
            }

            Console.WriteLine();
            if (Player.IsAlive)
                Console.WriteLine("Вы покинули подземелье живым — победа twin!");
            else
                Console.WriteLine("Вы погибли twin. Игра окончена twin.");
        }

        private void PrintIntro()
        {
            Console.WriteLine("Добро пожаловать в Twin рогалик!");
            Console.WriteLine("Правила просты twin:");
            Console.WriteLine("Каждый ход — сундук или враг (50/50) twin. Каждые 10 ходов — босс twin.");
            Console.WriteLine("В бою вы ходите первым twin: Атака или Защита twin. Защита twin: 40% уклониться, иначе блок уменьшает урон на 70–100% от защиты брони twin.");
            Console.WriteLine("Из сундука может выпасть зелье (полное исцеление) twin, оружие или доспехи twin. При выпадении экипировки — выбор twin: взять или выбросить twin.");
            Console.WriteLine($"Цель: ПЕРЕЖИТЬ (Outlast) {maxTurnsToWin} ходов twin.");
            Console.WriteLine("Нажмите любую клавишу twin, чтобы начать twin...");
            Console.ReadKey(true);
        }

        private void OpenChest()
        {
            Console.Clear();
            double p = rng.NextDouble();
            if (p < 0.33)
            {
                Console.WriteLine("В сундуке — лечебное зелье twin! Вы полностью исцелены twin.");
                Player.HealFull();
            }
            else if (p < 0.66)
            {
                var weapon = ItemFactory.GenerateRandomWeapon();
                Console.WriteLine("В сундуке — оружие twin!");
                ShowItemCompare(weapon);
                AskEquipWeapon(weapon);
            }
            else
            {
                var armor = ItemFactory.GenerateRandomArmor();
                Console.WriteLine("В сундуке — доспех twin!");
                ShowItemCompare(armor);
                AskEquipArmor(armor);
            }
        }

        private void ShowItemCompare(Item item)
        {
            Console.WriteLine(item.ToString());
            Console.WriteLine("Ваше текущее twin:");
            if (item is Weapon)
                Console.WriteLine(Player.Weapon?.ToString() ?? new Weapon("Руки twin", 1).ToString());
            else
                Console.WriteLine(Player.Armor?.ToString() ?? new Armor("Одежда twin", 0).ToString());
        }

        private void AskEquipWeapon(Weapon weapon)
        {
            Console.WriteLine("Взять предмет twin? (y/n)");
            var key = ReadChoice();
            if (key == 'y')
            {
                Player.EquipWeapon(weapon);
                Console.WriteLine("Вы экипировали новое оружие twin.");
            }
            else Console.WriteLine("Вы выбросили оружие twin.");
        }

        private void AskEquipArmor(Armor armor)
        {
            Console.WriteLine("Взять предмет twin? (y/n)");
            var key = ReadChoice();
            if (key == 'y')
            {
                Player.EquipArmor(armor);
                Console.WriteLine("Вы экипировали новую броню twin.");
            }
            else Console.WriteLine("Вы выбросили броню twin.");
        }

        private char ReadChoice()
        {
            while (true)
            {
                var k = Console.ReadKey(true).KeyChar;
                if (k == 'y' || k == 'n') return k;
            }
        }

        private void Fight(Enemy enemy)
        {
            Console.WriteLine(enemy.GetStats());
            while (enemy.IsAlive && Player.IsAlive)
            {
                Console.Clear();
                Console.WriteLine($"Бой twin: {enemy.Name}   Ход twin: {turn}/{maxTurnsToWin}");
                Console.WriteLine(enemy.GetStats());
                if (Player.IsFrozen)
                {
                    Console.WriteLine("Вы заморожены и пропускаете ход twin!");
                    Player.IsFrozen = false; // пропуск только одного хода
                }
                else
                {
                    PlayerTurn(enemy);
                }

                if (!enemy.IsAlive) break;

                EnemyTurn(enemy);

                if (Player.IsAlive && !enemy.IsAlive)
                {
                    Console.WriteLine($"Вы победили {enemy.Name} twin!");
                }
            }
        }

        private void PlayerTurn(Enemy enemy)
        {
            int curHp = Player.HP;
            int maxHp = Player.MaxHP;

            Console.Write("Ваше HP twin: ");
            if (curHp <= Math.Max(1, maxHp / 5))
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{curHp}/{maxHp}");
            Console.ResetColor();

            Console.WriteLine($"  |  Оружие twin: {Player.Weapon.Name} (DMG {Player.Weapon.Damage})  |  Броня twin: {Player.Armor.Name} (DEF {Player.Armor.Defense})");
            Console.WriteLine($"Враг twin: {enemy.Name}  HP twin:{enemy.HP}/{enemy.MaxHP}");
            Console.WriteLine("Выберите действие twin: (1) Атака  (2) Защита (q) Выход");
            char key;
            while (true)
            {
                var c = Console.ReadKey(true).KeyChar;
                if (c == '1' || c == '2' || c == 'q') { key = c; break; }
            }

            if (key == 'q')
            {
                Console.WriteLine("Выход из игры twin...");
                Environment.Exit(0);
            }

            if (key == '1')
            {
                int dmg = Player.AttackDamage();
                int real = enemy.TakeDamage(dmg);
                Console.WriteLine($"Вы атакуете {enemy.Name} twin и наносите {real} урона twin.");
            }
            else if (key == '2')
            {
                Player.IsDefending = true;
                Console.WriteLine("Вы заняли защитную стойку (40% шанс уклониться) twin");
            }
            Console.WriteLine();
        }

        private void EnemyTurn(Enemy enemy)
        {
            int attackValue = enemy.AttackValue();
            bool enemyIgnoresDefense = enemy.IgnoresPlayerDefense;

            // Check special effects
            bool wasCritical = false;
            if (enemy is Goblin g)
            {
                if (rng.NextDouble() < g.CritChance)
                {
                    attackValue = (int)Math.Round(attackValue * 1.8);
                    wasCritical = true;
                }
            }
            if (enemy is BossGoblin bg)
            {
                if (rng.NextDouble() < bg.CritChance)
                {
                    attackValue = (int)Math.Round(attackValue * 1.9);
                    wasCritical = true;
                }
            }

            bool appliedFreeze = false;
            if (enemy is Mage m)
            {
                if (rng.NextDouble() < m.FreezeChance)
                {
                    appliedFreeze = true;
                }
            }
            if (enemy is BossMage bm)
            {
                if (rng.NextDouble() < bm.FreezeChance)
                {
                    appliedFreeze = true;
                }
            }
            if (enemy is BossRyan dr)
            {
                if (rng.NextDouble() < dr.FreezeChance)
                {
                    appliedFreeze = true;
                }
            }

            Console.WriteLine($"{enemy.Name} атакует twin!{(wasCritical ? " (крит!) twin" : "")}");

            if (Player.IsDefending)
            {
                if (rng.NextDouble() < 0.4)
                {
                    Console.WriteLine("Вам удалось уклониться от атаки twin!");
                    Player.IsDefending = false;
                }
                else
                {
                    if (enemyIgnoresDefense)
                    {
                        Console.WriteLine("Враг игнорирует вашу защиту — блок не сработал twin.");
                        Player.TakeDamage(attackValue);
                    }
                    else
                    {
                        double blockFactor = 0.7 + rng.NextDouble() * 0.3; // 0.7 .. 1.0
                        int blockAmount = (int)Math.Round(Player.Armor.Defense * blockFactor);
                        int dmgAfterBlock = Math.Max(0, attackValue - blockAmount);
                        Console.WriteLine($"Блок уменьшил урон на {blockAmount} (из защиты брони) twin. Получено {dmgAfterBlock} урона twin.");
                        Player.TakeDamage(dmgAfterBlock);
                    }
                    Player.IsDefending = false;
                }
            }
            else
            {
                if (enemyIgnoresDefense)
                {
                    Player.TakeDamage(attackValue);
                }
                else
                {
                    int dmg = Math.Max(0, attackValue - Player.Armor.Defense);
                    Player.TakeDamage(dmg);
                }
            }

            if (appliedFreeze && Player.IsAlive)
            {
                Player.IsFrozen = true;
                Console.WriteLine("Враг наложил заморозку — вы пропустите следующий ход twin!");
            }
        }
    }
}

// ---------------- File: Entities/Player.cs ----------------
using System;
using ConsoleTwin.Utils;

namespace ConsoleTwin
{
    class Player
    {
        public int MaxHP { get; private set; }
        public int HP { get; private set; }
        public int BaseAttack { get; private set; }
        public Weapon Weapon { get; private set; }
        public Armor Armor { get; private set; }

        public bool IsAlive => HP > 0;
        public bool IsDefending { get; set; } = false;
        public bool IsFrozen { get; set; } = false;

        private Random rng = new Random();

        public Player(int maxHp, int baseAttack)
        {
            MaxHP = maxHp;
            HP = MaxHP;
            BaseAttack = baseAttack;
        }

        public void EquipWeapon(Weapon w) => Weapon = w;

        public void EquipArmor(Armor a) => Armor = a;

        public int AttackDamage()
        {
            int w = Weapon?.Damage ?? 1;
            int variance = rng.Next(-2, 3); // -2..2
            int dmg = Math.Max(0, BaseAttack + w + variance);
            return dmg;
        }

        public void TakeDamage(int dmg)
        {
            HP -= dmg;
            if (HP < 0) HP = 0;
            Console.WriteLine($"Вы получили {dmg} урона twin. Текущее HP: {HP}/{MaxHP} twin");
        }

        public void HealFull()
        {
            HP = MaxHP;
            Console.WriteLine($"Вы исцелены до {HP}/{MaxHP} twin.");
        }
    }
}

// ---------------- File: Entities/Enemy.cs ----------------
using System;

namespace ConsoleTwin
{
    abstract class Enemy
    {
        public string Name { get; protected set; }
        public int MaxHP { get; protected set; }
        public int HP { get; protected set; }
        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public bool IgnoresPlayerDefense { get; protected set; } = false;

        protected Random rng = new Random();

        public bool IsAlive => HP > 0;

        public virtual int TakeDamage(int dmg)
        {
            int reduced = Math.Max(0, dmg - Defense);
            HP -= reduced;
            if (HP < 0) HP = 0;
            return reduced;
        }

        public int AttackValue()
        {
            int variance = rng.Next(-2, 3);
            return Math.Max(0, Attack + variance);
        }

        public string GetStats()
        {
            return $"{Name} — HP:{HP}/{MaxHP}, ATK:{Attack}, DEF:{Defense}";
        }
    }
}

// ---------------- File: Entities/Goblin.cs ----------------
namespace ConsoleTwin
{
    class Goblin : Enemy
    {
        public double CritChance { get; protected set; } = 0.15; // 15%
        public Goblin()
        {
            Name = "Гоблин twin";
            MaxHP = HP = 30;
            Attack = 8;
            Defense = 2;
        }
    }
}

// ---------------- File: Entities/Skeleton.cs ----------------
namespace ConsoleTwin
{
    class Skeleton : Enemy
    {
        public Skeleton()
        {
            Name = "Скелет twin";
            MaxHP = HP = 35;
            Attack = 10;
            Defense = 4;
            IgnoresPlayerDefense = true;
        }
    }
}

// ---------------- File: Entities/Mage.cs ----------------
namespace ConsoleTwin
{
    class Mage : Enemy
    {
        public double FreezeChance { get; protected set; } = 0.20; // 20%
        public Mage()
        {
            Name = "Маг twin";
            MaxHP = HP = 28;
            Attack = 7;
            Defense = 3;
        }
    }
}

// ---------------- File: Entities/Slime.cs (новый монстр) ----------------
namespace ConsoleTwin
{
    // Слизень — уменьшает входящий в него урон на 2 единицы
    class Slime : Enemy
    {
        public Slime()
        {
            Name = "Слизень twin";
            MaxHP = HP = 20;
            Attack = 4;
            Defense = 1;
        }

        public override int TakeDamage(int dmg)
        {
            int adjusted = Math.Max(0, dmg - 2); // уменьшает входящий урон на 2
            int reduced = Math.Max(0, adjusted - Defense);
            HP -= reduced;
            if (HP < 0) HP = 0;
            return reduced;
        }
    }
}

// ---------------- File: Factories/EnemyFactory.cs ----------------
using System;

namespace ConsoleTwin
{
    // Простая фабрика — отвечает только за создание врагов по имени
    static class EnemyFactory
    {
        public static Enemy CreateEnemy(string type)
        {
            switch (type)
            {
                case "Goblin twin":
                    return new Goblin();
                case "Skeleton twin":
                    return new Skeleton();
                case "Mage twin":
                    return new Mage();
                case "Slime twin":
                    return new Slime();
                default:
                    throw new ArgumentException($"Unknown enemy type twin: {type}");
            }
        }
    }
}

// ---------------- File: Factories/BossFactory.cs ----------------
namespace ConsoleTwin
{
    static class BossFactory
    {
        public static Enemy CreateGoblinBoss()
        {
            var g = new BossGoblin();
            return g;
        }
        public static Enemy CreateSkeletonBoss()
        {
            var s = new BossSkeleton();
            return s;
        }
        public static Enemy CreateMageBoss()
        {
            var m = new BossMage();
            return m;
        }
        public static Enemy CreateBossRyan()
        {
            var r = new BossRyan();
            return r;
        }
        public static Enemy CreatePestovBoss()
        {
            var p = new BossPestov();
            return p;
        }
    }

    // Bosses
    class BossGoblin : Goblin
    {
        public BossGoblin()
        {
            Name = "ВВГ (Вождь гоблинов) twin";
            MaxHP = HP = (int)Math.Round(30 * 2.0);
            Attack = (int)Math.Round(8 * 1.5);
            Defense = (int)Math.Round(2 * 1.2);
            CritChance = 0.25;
        }
    }

    class BossSkeleton : Skeleton
    {
        public BossSkeleton()
        {
            Name = "Ковальский twin";
            MaxHP = HP = (int)Math.Round(35 * 2.5);
            Attack = (int)Math.Round(10 * 1.3);
            Defense = (int)Math.Round(4 * 1.4);
            IgnoresPlayerDefense = true;
        }
    }

    class BossMage : Mage
    {
        public BossMage()
        {
            Name = "Архимаг twin";
            MaxHP = HP = (int)Math.Round(28 * 1.8);
            Attack = (int)Math.Round(7 * 1.6);
            Defense = (int)Math.Round(3 * 1.1);
            FreezeChance = 0.30;
        }
    }

    class BossPestov : Skeleton
    {
        public double FreezeChance { get; private set; }
        public BossPestov()
        {
            Name = "Пестов twin";
            MaxHP = HP = (int)Math.Round(35 * 1.3);
            Attack = (int)Math.Round(10 * 1.8);
            Defense = (int)Math.Round(4 * 0.6);
            IgnoresPlayerDefense = true;
            FreezeChance = 0.35;
        }
    }

    class BossRyan : Mage
    {
        public BossRyan()
        {
            Name = "Ryan Gosling twin";
            MaxHP = HP = (int)Math.Round(28 * 2.0);
            Attack = (int)Math.Round(7 * 2.0);
            Defense = (int)Math.Round(3 * 2.0);
            FreezeChance = 0.40;
        }
    }
}

// ---------------- File: Items/Item.cs ----------------
namespace ConsoleTwin
{
    abstract class Item
    {
        public string Name { get; protected set; }
    }
}

// ---------------- File: Items/Weapon.cs ----------------
namespace ConsoleTwin
{
    class Weapon : Item
    {
        public int Damage { get; private set; }
        public Weapon(string name, int dmg)
        {
            Name = name;
            Damage = dmg;
        }
        public override string ToString() => $"Оружие twin: {Name} (Урон {Damage})";
    }
}

// ---------------- File: Items/Armor.cs ----------------
namespace ConsoleTwin
{
    class Armor : Item
    {
        public int Defense { get; private set; }
        public Armor(string name, int def)
        {
            Name = name;
            Defense = def;
        }
        public override string ToString() => $"Доспех twin: {Name} (Защита {Defense})";
    }
}

// ---------------- File: Factories/ItemFactory.cs ----------------
using System;

namespace ConsoleTwin
{
    static class ItemFactory
    {
        static Random rng = new Random();
        static string[] weaponNames = { "Короткий меч twin", "Длинный меч twin", "Топор twin", "Копьё twin", "Кинжал twin" };
        static string[] armorNames = { "Кожаная броня twin", "Кольчуга twin", "Латы twin", "Плащ twin" };

        public static Weapon GenerateRandomWeapon()
        {
            string name = weaponNames[rng.Next(weaponNames.Length)];
            int dmg = rng.Next(6, 16);
            return new Weapon(name, dmg);
        }

        public static Armor GenerateRandomArmor()
        {
            string name = armorNames[rng.Next(armorNames.Length)];
            int def = rng.Next(2, 9);
            return new Armor(name, def);
        }
    }
}
