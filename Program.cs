using System;
using System.Linq;

namespace HazardStats
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var intArgs = args.Select(int.Parse).ToArray();
                var game = new Game(intArgs[0], intArgs[1]);
                game.OutStats();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.ExitCode = 1;
            }
        }
    }
}
