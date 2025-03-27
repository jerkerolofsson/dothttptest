using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Metrics
{
    public class CalculatedMetric : BaseMetric
    {
        public CalculatedMetric(string name, string unit) : base(name, unit)
        {
        }

        /// <summary>
        /// Value when bucket was crated
        /// </summary>
        public DateTime Timestamp { get; set; }

        public double Average { get; internal set; }
        public double Median { get; internal set; }
        public int Count { get; internal set; }
        public double Sum { get; internal set; }
        public double Q1 { get; internal set; }
        public double Q3 { get; internal set; }
        public double P90 { get; internal set; }
        public double P95 { get; internal set; }
        public double P5 { get; internal set; }
    }
}
