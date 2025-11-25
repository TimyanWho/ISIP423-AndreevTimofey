using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP423_AndreevTimofey
{
    namespace ConsoleTwin
    {
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
                int adjusted = Math.Max(0, dmg - 2);
                int reduced = Math.Max(0, adjusted - Defense);
                HP -= reduced;
                if (HP < 0) HP = 0;
                return reduced;
            }
        }
    }
}
