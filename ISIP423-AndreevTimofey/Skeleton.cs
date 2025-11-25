using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP423_AndreevTimofey
{
    namespace ConsoleTwin
    {
        class Skeleton : Enemy
        {
            public Skeleton()
            {
                Name = "Скелет twin";
                MaxHP = HP = 35;
                Attack = 10;
                Defense = 4;
                IgnoresPlayerDefense = true;
            }
        }
    }
}
