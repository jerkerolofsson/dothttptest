using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Metrics
{
    public class BaseMetric
    {
        public string Name { get; }
        public string Unit { get; }

        public BaseMetric(string name, string unit)
        {
            Unit = unit;
            Name = name;
        }

    }
}
