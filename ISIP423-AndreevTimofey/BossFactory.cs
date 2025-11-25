using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP423_AndreevTimofey
{

        class BossSkeleton : Skeleton
        {
            public BossSkeleton()
            {
                Name = "Ковальский twin";
                MaxHP = HP = (int)Math.Round(35 * 2.5);
                Attack = (int)Math.Round(10 * 1.3);
                Defense = (int)Math.Round(4 * 1.4);
                IgnoresPlayerDefense = true;
            }
        }

        class BossMage : Mage
        {
            public BossMage()
            {
                Name = "Архимаг twin";
                MaxHP = HP = (int)Math.Round(28 * 1.8);
                Attack = (int)Math.Round(7 * 1.6);
                Defense = (int)Math.Round(3 * 1.1);
                FreezeChance = 0.30;
            }
        }


        class BossPestov : Skeleton
        {
            public double FreezeChance { get; private set; }
            public BossPestov()
            {
                Name = "Пестов twin";
                MaxHP = HP = (int)Math.Round(35 * 1.3);
                Attack = (int)Math.Round(10 * 1.8);
                Defense = (int)Math.Round(4 * 0.6);
                IgnoresPlayerDefense = true;
                FreezeChance = 0.35;
            }
        }


        class BossRyan : Mage
        {
            public BossRyan()
            {
                Name = "Ryan Gosling twin";
                MaxHP = HP = (int)Math.Round(28 * 2.0);
                Attack = (int)Math.Round(7 * 2.0);
                Defense = (int)Math.Round(3 * 2.0);
                FreezeChance = 0.40;
            }
        }
    }
}
