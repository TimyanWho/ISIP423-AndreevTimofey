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
            static string[] weaponNames = { "Короткий меч", "Длинный меч", "Топор", "Копьё", "Кинжал" };
            static string[] armorNames = { "Кожаная броня", "Кольчуга", "Латы", "Плащ" };


            public static Weapon GenerateRandomWeapon()
            {
                string name = weaponNames[RandomProvider.Instance.Next(weaponNames.Length)];
                int dmg = RandomProvider.Instance.Next(6, 16);
                return new Weapon(name, dmg);
            }


            public static Armor GenerateRandomArmor()
            {
                string name = armorNames[RandomProvider.Instance.Next(armorNames.Length)];
                int def = RandomProvider.Instance.Next(2, 9);
                return new Armor(name, def);
            }
        }
    }
}
