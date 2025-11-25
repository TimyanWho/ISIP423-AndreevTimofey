using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISIP423_AndreevTimofey.ConsoleTwin;
using ConsoleTwin;

namespace ISIP423_AndreevTimofey
{

    using global::ConsoleTwin;
    using System;
    using System.Collections.Generic;

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

}