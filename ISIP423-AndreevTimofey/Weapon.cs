using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP423_AndreevTimofey
{
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
}
