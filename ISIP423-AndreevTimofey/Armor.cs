using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ISIP423_AndreevTimofey
{
    namespace ConsoleTwin
    {
        class Armor : Item
        {
            public int Defense { get; private set; }
            public Armor(string name, int def)
            {
                Name = name;
                Defense = def;
            }
            public override string ToString() => $"Броня twin: {Name} (Урон {Defense}) twin";
        }
    }
}
