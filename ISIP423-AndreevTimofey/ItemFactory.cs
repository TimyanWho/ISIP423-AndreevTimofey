using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP423_AndreevTimofey
{
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
}
