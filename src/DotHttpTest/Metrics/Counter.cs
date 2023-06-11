using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Metrics
{
    public class Counter : BaseMetric
    {
        public double Value { get; protected set; } = 0;

        public Counter(string name, string unit) : base(name, unit)
        {
        }

        public void SetValue(double val)
        {
            Value = val;
        }
        public void Increment(double val)
        {
            Value += val;
        }
    }
}
