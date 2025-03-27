using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Metrics
{
    public class LongCounter : BaseMetric
    {
        private long mValue;

        public long Value => mValue;

        public LongCounter(string name, string unit) : base(name, unit)
        {
        }

        public void SetValue(long val)
        {
            Interlocked.Exchange(ref mValue, val);
        }
        public void Increment(long val)
        {
            Interlocked.Add(ref mValue, val);
        }
    }
}
