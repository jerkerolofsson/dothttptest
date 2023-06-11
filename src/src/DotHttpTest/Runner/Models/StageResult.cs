using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner.Models
{
    /// <summary>
    /// This is the result of a TestStage, part of the report
    /// </summary>
    public class StageResult
    {
        private List<HttpRequestMetrics> mResults = new();

        public TestPlanStage PlannedStage { get; init; }

        public IReadOnlyList<HttpRequestMetrics> Results => mResults;

        public StageResult(TestPlanStage plannedStage)
        {
            PlannedStage = plannedStage;
        }

        public int PassCount
        {
            get => mResults.Where(x => x.ChecksFailed.Value == 0).Count();
        }
        public int FailCount
        {
            get => mResults.Where(x => x.ChecksFailed.Value > 0).Count();
        }

        internal void AddRequestMetrics(HttpRequestMetrics metrics)
        {
            mResults.Add(metrics);
        }
    }
}
