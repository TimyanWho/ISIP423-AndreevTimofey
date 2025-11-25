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
        static class EnemyFactory
        {
            public static Enemy CreateEnemy(string type)
            {
                switch (type)
                {
                    case "Goblin twin":
                        return new Goblin();
                    case "Skeleton twin":
                        return new Skeleton();
                    case "Mage twin":
                        return new Mage();
                    case "Slime twin":
                        return new Slime();
                    default:
                        throw new ArgumentException($"Unknown enemy type twin: {type}");
                }
            }
        }
    }
}
