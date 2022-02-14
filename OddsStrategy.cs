using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazardStats
{
    class OddsStrategy
    {
        public StrategyType StrategyType { get; set; }
        public int NumericOdds { get; set; }
    }

    enum StrategyType
    {
        Numeric = 1,
        UniformPayout
    }
}
