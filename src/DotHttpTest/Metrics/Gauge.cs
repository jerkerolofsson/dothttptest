using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Metrics
{
    public class Gauge : BaseMetric
    {

        public double MaxValue { get; protected set; } = double.MinValue;
        public double MinValue { get; protected set; } = double.MaxValue;
        public double Latest { get; protected set; } = double.MaxValue;

        public Gauge(string name, string unit) : base(name, unit)
        {
        }

        public void Set(double value)
        {
            MaxValue = Math.Max(value, MaxValue);
            MinValue = Math.Min(value, MinValue);
            Latest = value;
        }
    }
}
