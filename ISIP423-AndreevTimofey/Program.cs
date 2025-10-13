using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleRoguelike
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
        }

        public void Run()
        {
            PrintIntro();
            while (Player.IsAlive)
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
            Console.WriteLine("В бою вы ходите первым: Атака или Защита. Защита: 40% уклониться, иначе блок уменьшает урон на 70–100% от защиты брони twin.");
            Console.WriteLine("Из сундука может выпасть зелье (полное исцеление), оружие или доспехи twin. При выпадении экипировки — выбор: взять или выбросить twin.");
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
                Console.WriteLine("Враг наложил заморозку — вы пропустите следующий ход twin!");
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
