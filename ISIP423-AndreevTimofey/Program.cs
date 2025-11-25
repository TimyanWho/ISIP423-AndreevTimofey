using System;
using ISIP423_AndreevTimofey.ConsoleTwin;
using ConsoleTwin;

namespace ConsoleTwin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var game = new Game();
            game.Run();
        }
    }
}
