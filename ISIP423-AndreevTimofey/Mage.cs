using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP423_AndreevTimofey
{
    namespace ConsoleTwin
    {
        class Mage : Enemy
        {
            public double FreezeChance { get; protected set; } = 0.20; // 20%
            public Mage()
            {
                Name = "Маг twin";
                MaxHP = HP = 28;
                Attack = 7;
                Defense = 3;
            }
        }
    }
}
