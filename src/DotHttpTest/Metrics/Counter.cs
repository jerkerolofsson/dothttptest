using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Metrics
{
    public class Counter : BaseMetric
    {
        private double mValue;

        public double Value => mValue;

        public Counter(string name, string unit) : base(name, unit)
        {
        }

        public void SetValue(double val)
        {
            Interlocked.Exchange(ref mValue, val);
        }
        public void Increment(double val)
        {
            lock (this)
            {
                mValue = mValue + val;
            }
        }
    }
}
