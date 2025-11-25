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
}
