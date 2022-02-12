using System;

namespace HazardStats
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var game = new Game(int.Parse(args[0]));
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
