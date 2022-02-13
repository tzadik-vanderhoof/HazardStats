using System;
using System.Collections.Generic;
using System.Linq;

//TODO: odds, Don'ts, adjust Value on 12, other bets

namespace HazardStats
{
    class Game
    {
        private readonly int _mainNumber;

        public Game(int mainNumber)
        {
            if (mainNumber < 5 || mainNumber > 9) throw new ArgumentException($"Invalid {nameof(mainNumber)}: {mainNumber}");

            _mainNumber = mainNumber;
        }

        private static IEnumerable<int> Rolls => Enumerable.Range(2, 11);

        private static IEnumerable<Outcome> Outcomes => new[] { Outcome.Win, Outcome.Loss };

        private static int Ways(int roll) => 
            !Rolls.Contains(roll) ? throw new ArgumentException($"Invalid {nameof(roll)}: {roll}") :
            roll <= 7 ? roll - 1 : 13 - roll;

        private static double Probability(int roll) => (double)Ways(roll) / 36;

        private double Probability(int roll, Outcome outcome) =>
            Category(roll) switch
            {
                RollCategory.Craps => outcome == Outcome.Loss ? Probability(roll) : 0,
                RollCategory.Natural => outcome == Outcome.Win ? Probability(roll) : 0,
                RollCategory.Point => Probability(roll) * PointProbability(roll, outcome),
                _  =>  throw new ApplicationException("Unexpected category")
            };
    
        private double PointProbability(int roll, Outcome outcome)
        {
            var winWays = Ways(roll);
            var lossWays = Ways(_mainNumber);
            var outcomeWays = outcome switch
            {
                Outcome.Win => winWays,
                Outcome.Loss => lossWays,
                _ => 0
            };
            return (double)outcomeWays / (winWays + lossWays);
        }

        private RollCategory Category(int roll)
            => roll switch
            {
                2 or 3 => RollCategory.Craps,

                11 => _mainNumber switch {
                    7 => RollCategory.Natural,
                    _ => RollCategory.Craps,
                },

                12 => _mainNumber switch
                {
                    6 or 8 => RollCategory.Natural,
                    _ => RollCategory.Craps
                },

                var c when c == _mainNumber => RollCategory.Natural,

                _ => RollCategory.Point
            };

        private double Value(int roll, Outcome outcome) =>
            outcome == Outcome.Win ?
            (
                Category(roll) switch {
                    RollCategory.Point => UnitsRisked(roll) + 1 + (double)XOdds(roll) * Ways(_mainNumber) / Ways(roll),
                    RollCategory.Natural => UnitsRisked(roll) + 1,
                    RollCategory.Push => UnitsRisked(roll),
                    _ => 0
                }
            ) :
            0;

        private int UnitsRisked(int roll) =>
            Category(roll) switch
            {
                RollCategory.Point => 1 + XOdds(roll),
                _ => 1
            };

        private int XOdds(int roll) =>
            Category(roll) switch
            {
                RollCategory.Point => 2,
                _ => throw new ArgumentException($"Invalid roll for odds: {roll}")
            };

        private static void Out(string? s = null) => Console.WriteLine(s);

        private static double Round(double n) => Math.Round(n, 2);
        private static string Pct(double n) => $"{Round(n * 100)}%";

        private string? ProbMessage(Stat stat) => 
            stat.Probability == 0 ? null :
            $"{stat.Outcome.ToString()[0]}: {Pct(stat.Probability)}";

        private IEnumerable<Stat> Stats()
            => Rolls.SelectMany(roll =>
                Outcomes.Select(outcome =>
                    new Stat { 
                        Roll = roll,
                        Outcome = outcome,
                        Probability = Probability(roll, outcome),
                        UnitsRisked = UnitsRisked(roll),
                        Value = Value(roll, outcome)
                    }));

        private string StatLine(IGrouping<int, Stat> group)
        {
            var probMsg = string.Join("; ", group.Select(ProbMessage).Where(m => m != null));
            var value = group.Sum(stat => Round(stat.Value * stat.Probability));
            var unitsRisked = group.Sum(stat => stat.UnitsRisked * stat.Probability);
            return $"{group.Key,2}: {probMsg}; Value: {Round(value/unitsRisked)}";
        }

        public void OutStats()
        {
            Out($"Main: {_mainNumber}");
            Out();

            var stats = Stats().ToArray();

            // stats by roll
            var lines = stats.GroupBy(s => s.Roll).Select(StatLine);
            foreach (var line in lines) Out(line);
            Out();

            // stats by outcome
            var outcomeStats = stats.GroupBy(stat => stat.Outcome)
                .Select(g => new Stat
                {
                    Outcome = g.Key,
                    Probability = g.Sum(stat => stat.Probability)
                });

            foreach (var stat in outcomeStats) Out($"{stat.Outcome,4}: {Pct(stat.Probability)}");
            Out();

            // house edge
            const int scale = 1980;
            var totalRisk = stats.Sum(stat => stat.Probability * stat.UnitsRisked) * scale;
            Out($"Total risk: ${Round(totalRisk)}");

            var totalValue = stats.Sum(stat => stat.Probability * stat.Value) * scale;
            Out($"Total value: ${Round(totalValue)}");

            var edge = (totalRisk - totalValue) / totalRisk;
            Out($"Edge: {Pct(edge)}");

        }
    }
}
