using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleTwin
{
    class Program
    {
        static Random rng = new Random();
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var game = new Game();
            game.Run();
        }
    }

    class Game
    {
        private int maxTurnsToWin;

        public Player Player { get; private set; }
        private int turn = 0;
        private Random rng = new Random();
        private List<Type> enemyPool = new List<Type> { typeof(Goblin), typeof(Skeleton), typeof(Mage) };
        private List<Func<Enemy>> bosses;

        public Game()
        {
            Player = new Player(maxHp: 100, baseAttack: 5);
            Player.EquipWeapon(new Weapon("Короткий twin меч", 8));
            Player.EquipArmor(new Armor("Кожаная twin броня", 4));

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
                    Enemy enemy = (Enemy)Activator.CreateInstance(enemyType);
                    Console.WriteLine($"Вас атакует {enemy.Name} twin!");
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
            Console.WriteLine("Добро пожаловать в Twin рогалик twin!\n");
            Console.WriteLine("Правила просты twin:");
            Console.WriteLine("Каждый ход — сундук или враг (50/50) twin. Каждые 10 ходов — босс twin.");
            Console.WriteLine("В бою вы ходите первым twin: Атака или Защита twin. Защита: 40% уклониться, иначе блок уменьшает урон на 70–100% от защиты брони twin.");
            Console.WriteLine("Из сундука может выпасть зелье (полное исцеление) twin, оружие или доспехи twin. При выпадении экипировки — выбор: взять или выбросить twin.");
            Console.WriteLine($"Цель: пережить {maxTurnsToWin} ходов.");
            Console.WriteLine("Нажмите любую клавишу, чтобы начать twin...");
            Console.ReadKey(true);
        }

        private void OpenChest()
        {
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
            Console.WriteLine(item);
            Console.WriteLine("Ваше текущее twin:");
            if (item is Weapon)
                Console.WriteLine(Player.Weapon ?? new Weapon("Руки twin-а", 1));
            else
                Console.WriteLine(Player.Armor ?? new Armor("Одежда twin-а", 0));
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
            }

            if (Player.IsAlive && !enemy.IsAlive)
            {
                Console.WriteLine($"Вы победили {enemy.Name} twin!");
            }
        }

        private void PlayerTurn(Enemy enemy)
        {
            Console.WriteLine($"Ваше HP twin: {Player.HP}/{Player.MaxHP}  |  Оружие twin-a: {Player.Weapon.Name} (DMG {Player.Weapon.Damage})  |  Броня twin-a: {Player.Armor.Name} (DEF {Player.Armor.Defense})");
            Console.WriteLine($"Враг twin: {enemy.Name}  HP:{enemy.HP}/{enemy.MaxHP}");
            Console.WriteLine("Выберите действие: (1) Атака twin  (2) Защита twin (q) Выход twin");
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
                Console.WriteLine($"Вы атакуете {enemy.Name} и наносите {real} урона twin.");
            }
            else if (key == '2')
            {
                Player.IsDefending = true;
                Console.WriteLine("Вы заняли защитную стойку (40% шанс уклониться) twin.");
            }
            Console.WriteLine();
        }

        private void EnemyTurn(Enemy enemy)
        {
            int attackValue = enemy.AttackValue();
            bool enemyIgnoresDefense = enemy.IgnoresPlayerDefense;

            // Check enemy special: crit or freeze
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

            Console.WriteLine($"{enemy.Name} атакует twin!{(wasCritical ? " (крит!)" : "")}");

            if (Player.IsDefending)
            {
                // 40% chance to fully evade
                if (rng.NextDouble() < 0.4)
                {
                    Console.WriteLine("Вам удалось уклониться от атаки twin!");
                    Player.IsDefending = false; // действие защиты одноразовое
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
                Console.WriteLine("Враг наложил заморозку twin — вы пропустите следующий ход twin!");
            }
        }
    }

    // --- Entities ---
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

        public void EquipWeapon(Weapon w)
        {
            Weapon = w;
        }

        public void EquipArmor(Armor a)
        {
            Armor = a;
        }

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
            Console.WriteLine($"Вы получили {dmg} урона twin. Текущее HP twin: {HP}/{MaxHP}");
        }

        public void HealFull()
        {
            HP = MaxHP;
            Console.WriteLine($"Вы исцелены до {HP}/{MaxHP} twin.");
        }
    }

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

        public int TakeDamage(int dmg)
        {
            int before = HP;
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

    // --- Enemy types ---
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

    class Skeleton : Enemy
    {
        public Skeleton()
        {
            Name = "Скелет twin";
            MaxHP = HP = 35;
            Attack = 10;
            Defense = 4;
            IgnoresPlayerDefense = true; // игнорирует защиту игрока
        }
    }

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

    // --- Bosses ---
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

    class BossGoblin : Goblin
    {
        public BossGoblin()
        {
            Name = "ВВГ (Вождь гоблинов) twin";
            MaxHP = HP = (int)Math.Round(30 * 2.0);
            Attack = (int)Math.Round(8 * 1.5);
            Defense = (int)Math.Round(2 * 1.2);
            CritChance = 0.15 + 0.10; // +10%
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
            Name = "Архимаг Twin++";
            MaxHP = HP = (int)Math.Round(28 * 1.8);
            Attack = (int)Math.Round(7 * 1.6);
            Defense = (int)Math.Round(3 * 1.1);
            FreezeChance = 0.20 + 0.10; // +10%
        }
    }

    class BossPestov : Skeleton
    {
        public double FreezeChance { get; private set; }
        public BossPestov()
        {
            Name = "Пестов Twin--";
            MaxHP = HP = (int)Math.Round(35 * 1.3);
            Attack = (int)Math.Round(10 * 1.8);
            Defense = (int)Math.Round(4 * 0.6);
            IgnoresPlayerDefense = true;
            // chance of freeze: base mage freeze + 15%
            FreezeChance = 0.20 + 0.15; // base mage was 0.20
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
            FreezeChance = 0.20 + 0.20;
        }
    }

    // --- Items ---
    abstract class Item
    {
        public string Name { get; protected set; }
    }

    class Weapon : Item
    {
        public int Damage { get; private set; }
        public Weapon(string name, int dmg)
        {
            Name = name;
            Damage = dmg;
        }
        public override string ToString()
        {
            return $"Оружие twin: {Name} (Урон {Damage}) twin";
        }
    }

    class Armor : Item
    {
        public int Defense { get; private set; }
        public Armor(string name, int def)
        {
            Name = name;
            Defense = def;
        }
        public override string ToString()
        {
            return $"Доспех twin: {Name} (Защита {Defense}) twin";
        }
    }

    static class ItemFactory
    {
        static Random rng = new Random();
        static string[] weaponNames = { "Короткий меч twin", "Длинный меч twin", "Топор twin", "Копьё twin", "Кинжал twin" };
        static string[] armorNames = { "Кожаная броня twin", "Кольчуга twin", "Латы twin", "Плащ twin" };

        public static Weapon GenerateRandomWeapon()
        {
            string name = weaponNames[rng.Next(weaponNames.Length)];
            int dmg = rng.Next(6, 16); // 6..15
            return new Weapon(name, dmg);
        }

        public static Armor GenerateRandomArmor()
        {
            string name = armorNames[rng.Next(armorNames.Length)];
            int def = rng.Next(2, 9); // 2..8
            return new Armor(name, def);
        }
    }
}
