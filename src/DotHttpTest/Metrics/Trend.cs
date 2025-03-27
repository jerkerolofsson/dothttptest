using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Metrics
{
    public class Trend : BaseMetric
    {
        private LinkedList<CalculatedMetric> mHistoryBuckets = new();
        private int mSampleCountForNextBucket = 0;

        private LinkedList<double> mHistory = new LinkedList<double>();
        private List<double> mHistorySortedByValues = new List<double>();

        public double Sum
        {
            get
            {
                lock (mHistory)
                {
                    return mHistory.Sum();
                }
            }
        }
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
        public double Median => CalculateP(0.5);

        public List<CalculatedMetric> HistoryBuckets
        {
            get
            {
                lock(mHistoryBuckets)
                {
                    return mHistoryBuckets.ToList();
                }
            }
        }

        /// <summary>
        /// Last historical values, limited to MaxHistoryCount
        /// </summary>
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

        public double P5 => CalculateP(0.05);
        public double P90 => CalculateP(0.90);
        public double P95 => CalculateP(0.95);
        public double Q3 => CalculateP(0.75);
        public double Q1 => CalculateP(0.25);
        public double MaxValue { get; protected set; } = double.MinValue;
        public double MinValue { get; protected set; } = double.MaxValue;
        public double Latest { get; protected set; } = double.MaxValue;
        public int MaxHistoryCount { get; }
        public int MaxBucketHistoryCount { get; } = 10;

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

        private void LogToHistoryBuckets()
        {
            mSampleCountForNextBucket++;
            if(mSampleCountForNextBucket >= MaxHistoryCount)
            {
                // Create a bucket and save calculated values for all data in the history
                CreateNewHistoryBucket();
                mSampleCountForNextBucket = 0;
            }
        }

        private void CreateNewHistoryBucket()
        {
            ClearTempHistoryCache();

            var bucket = new CalculatedMetric(this.Name, this.Unit)
            {
                Timestamp = DateTime.UtcNow,
                Average = this.Average,
                Median = this.Median,
                P95 = this.P95,
                P90 = this.P90,
                P5 = this.P5,
                Q1 = this.Q1,
                Q3 = this.Q3,
                Sum = this.Sum,
                Count = mHistory.Count
            };
            lock (mHistoryBuckets)
            {
                mHistoryBuckets.AddLast(bucket);
                if(mHistoryBuckets.Count > MaxBucketHistoryCount)
                {
                    mHistoryBuckets.RemoveFirst();
                }
            }

            ClearTempHistoryCache();
        }

        private void ClearTempHistoryCache()
        {
            lock (mHistory)
            {
                mHistorySortedByValues.Clear();
            }
        }

        private void LogToHistoryCollection(double value)
        {
            lock (mHistory)
            {
                mHistory.AddLast(value);
                mHistorySortedByValues.Clear();

                if (mHistory.Count > MaxHistoryCount)
                {
                    LogToHistoryBuckets();
                    while (mHistory.Count > MaxHistoryCount)
                    {
                        mHistory.RemoveFirst();
                    }
                }
            }
        }
    }
}
