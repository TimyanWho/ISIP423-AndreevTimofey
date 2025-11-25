using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP423_AndreevTimofey
{
    namespace ConsoleTwin
    {
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
    }
}
