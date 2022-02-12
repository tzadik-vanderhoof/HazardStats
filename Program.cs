using System.Collections.Generic;
using System.Linq;

namespace HazardStats
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(7);
            game.OutStats();
        }
    }
}
