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
        public static class RandomProvider
        {
            public static Random Instance { get; } = new Random();
        }
    }
}
