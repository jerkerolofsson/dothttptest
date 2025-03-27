using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Metrics
{
    public class IntCounter : BaseMetric
    {
        private int mValue;

        public int Value => mValue;

        public IntCounter(string name, string unit) : base(name, unit)
        {
        }

        public void SetValue(int val)
        {
            Interlocked.Exchange(ref mValue, val);
        }
        public void Increment(int val)
        {
            Interlocked.Add(ref mValue, val);
        }
    }
}
