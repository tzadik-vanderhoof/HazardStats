using System;

namespace HazardStats
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var mainNumber = int.Parse(args[0]);
                var odds = args[1];
                var game = new Game(mainNumber, odds);
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
