using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP423_AndreevTimofey
{
    using global::ConsoleTwin;

    using System;

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
}
