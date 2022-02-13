﻿using System;
using System.Collections.Generic;
using System.Linq;

//TODO: use Value, Don'ts, adjust Value on 12, odds, other bets

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

        private static int Value(int roll, Outcome outcome) => outcome == Outcome.Win ? 1 : -1;  

        private static int UnitsRisked(int roll) => 1;

        private static void Out(string? s = null) => Console.WriteLine(s);

        private static double Round(double probability) => Math.Round(probability, 3);
        private static double RoundProbability(Stat stat) => Round(stat.Probability);

        private string? Message(Stat stat)
            => stat.Probability == 0 ? null : $"{stat.Outcome.ToString()[0]}: {RoundProbability(stat)}";

        private IEnumerable<Stat> Stats()
            => Rolls.SelectMany(roll => Outcomes.Select(outcome =>
                new Stat { Roll = roll, Outcome = outcome, Probability = Probability(roll, outcome) }));

        private string StatLine(IGrouping<int, Stat> group)
        {
            var msgs = group.Select(Message).Where(m => m != null);
            var msg = string.Join("; ", msgs);
            return $"{group.Key,2}: {msg}";
        }

        public void OutStats()
        {
            Out($"Main: {_mainNumber}");
            Out();

            var stats = Stats().ToArray();
            var lines = stats.GroupBy(s => s.Roll).Select(StatLine);
            foreach (var line in lines) Out(line);
            Out();

            var outcomeStats = Outcomes.Select(outcome => new Stat
            {
                Outcome = outcome,
                Probability = stats.Where(stat => stat.Outcome == outcome).Sum(stat => stat.Probability)
            });

            foreach (var stat in outcomeStats) Out($"{stat.Outcome,4}: {RoundProbability(stat)}");

            var edge = outcomeStats.Single(stat => stat.Outcome == Outcome.Loss).Probability -
                outcomeStats.Single(stat => stat.Outcome == Outcome.Win).Probability;

            Out($"Edge: {Round(edge * 100)}%");
        }
    }
}
