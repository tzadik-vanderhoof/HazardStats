using System;
using System.Collections.Generic;
using System.Linq;

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

        private static int Ways(int roll) => 
            !Rolls.Contains(roll) ? throw new ArgumentException($"Invalid {nameof(roll)}: {roll}") :
            roll <= 7 ? roll - 1 : 13 - roll;

        private static double Probability(int roll) => (double)Ways(roll) / 36;

        private double Probability(int roll, Outcome outcome) =>
            IsCraps(roll) ? (outcome == Outcome.Loss ? Probability(roll) : 0) :
            IsPoint(roll) ? Probability(roll) * PointProbability(roll, outcome) :
            outcome == Outcome.Win ? Probability(roll) : 0;

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

        private bool IsCraps(int roll) => CrapsRolls().Contains(roll);

        private IEnumerable<int> CrapsRolls()
        {
            yield return 2;
            yield return 3;

            if ((new[] { 5, 6, 8, 9 }).Contains(_mainNumber)) yield return 11;
            if ((new[] { 5, 7, 9 }).Contains(_mainNumber)) yield return 12;
        }

        private bool IsPoint(int roll) => roll >= 4 && roll <= 10 && roll != _mainNumber;

        private static int Value(int roll, Outcome outcome) => outcome == Outcome.Win ? 1 : -1;  

        private static int UnitsRisked(int roll) => 1;

        private static void Out(string? s = null) => Console.WriteLine(s);

        private string? Message(double prob, Outcome outcome)
            => prob == 0 ? null : $"{outcome.ToString()[0]}: {Math.Round(prob, 3)}";
        

        public void OutStats()
        {
            Out($"Main: {_mainNumber}");
            Out();

            var total = 0.0;
            foreach (var roll in Rolls)
            {
                var stats = (new[] { Outcome.Win, Outcome.Loss })
                    .Select(outcome =>
                    {
                        var prob = Probability(roll, outcome);
                        return new { Prob = prob, Msg = Message(prob, outcome) };
                    });

                total += stats.Sum(s => s.Prob);
                var msgs = stats.Select(p => p.Msg).Where(m => m != null);
                   
                var msg = string.Join("; ", msgs);
                Out($"{roll}: {msg}");
            }

            Out();
            Out($"Total: {total}");
        }
    }
}
