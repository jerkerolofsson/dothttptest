using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Metrics
{
    public class Trend : BaseMetric
    {
        public double Sum => mHistory.Sum();
        public double Average
        {
            get
            {
                lock (mHistory)
                {
                    if (mHistory.Count == 0) return 0;
                    return mHistory.Average();
                }
            }
        }

        public IEnumerable<double> History
        {
            get
            {
                lock (mHistory)
                {
                    return mHistory.ToList();
                }
            }
        }


        public double Median => CalculateP(0.5);

        private double CalculateP(double p)
        {
            lock (mHistory)
            {
                if (mHistorySortedByValues.Count == 0)
                {
                    mHistorySortedByValues.AddRange(mHistory);
                    mHistorySortedByValues.Sort();
                }

                if (mHistorySortedByValues.Count > 0)
                {
                    var index = (int)Math.Floor(mHistorySortedByValues.Count * p);
                    return mHistorySortedByValues[index];
                }

                return 0;
            }
        }

        public double P90 => CalculateP(0.95);
        public double P95 => CalculateP(0.95);
        public double MaxValue { get; protected set; } = double.MinValue;
        public double MinValue { get; protected set; } = double.MaxValue;
        public double Latest { get; protected set; } = double.MaxValue;
        public int MaxHistoryCount { get; }

        private LinkedList<double> mHistory = new LinkedList<double>();
        private List<double> mHistorySortedByValues = new List<double>();

        public Trend(string name, string unit, int maxHistoryCount) : base(name, unit)
        {
            MaxHistoryCount = maxHistoryCount;
        }

        public void Log(double value)
        {
            MaxValue = Math.Max(value, MaxValue);
            MinValue = Math.Min(value, MinValue);
            Latest = value;

            LogToHistoryCollection(value);
        }

        private void LogToHistoryCollection(double value)
        {
            lock (mHistory)
            {
                mHistory.AddLast(value);
                mHistorySortedByValues.Clear();
                while (mHistory.Count > MaxHistoryCount)
                {
                    mHistory.RemoveFirst();
                }
            }
        }
    }
}
