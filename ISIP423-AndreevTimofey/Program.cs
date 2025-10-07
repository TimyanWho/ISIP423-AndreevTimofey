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
