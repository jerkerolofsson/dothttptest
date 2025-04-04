﻿using DotHttpTest.Metrics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner.Models
{
    public class TestStatus
    {
        private List<VerificationCheckResult> mFailedChecks = new List<VerificationCheckResult>();

        public IReadOnlyList<VerificationCheckResult> FailedChecks => mFailedChecks;

        public bool IsCompleted { get; set; } = false;

        public TestPlanStage? CurrentStage { get; internal set; }
        public TestReport TestReport { get; }
        public IntCounter Iterations { get; set; } = new("iterations", "#");
        public Trend IterationDuration { get; set; } = new("iteration_duration", "s", 1000);
        public Counter ElapsedSeconds { get; set; } = new("test_elapsed", "s");
        public Counter ProgressPercent { get; set; } = new("test_progress", "%");
        public Trend UserCount { get; set; } = new("vus", "#", 50);
        public Gauge UserMaxCount { get; set; } = new("vus_max", "#");

        public LongCounter HttpBytesSent { get; set; } = new("http_bytes_sent", "B");
        public LongCounter HttpBytesReceived { get; set; } = new("http_bytes_recv", "B");
        public LongCounter HttpRequests { get; set; } = new("http_reqs", "#");
        public LongCounter HttpRequestFails { get; set; } = new("http_req_failed", "#");
        public Trend HttpRequestDuration { get; set; } = new("http_req_duration", "s", 5000);
        public Trend HttpRequestsPerSecond { get; set; } = new("http_reqs_per_sec", "r/s", 5000);

        public LongCounter ChecksPassed { get; set; } = new("checks_passed", "#");
        public LongCounter ChecksFailed { get; set; } = new("checks_failed", "#");
        public LongCounter TestsPassed { get; set; } = new("tests_passed", "#");
        public LongCounter TestsFailed { get; set; } = new("tests_failed", "#");

        public IReadOnlyDictionary<System.Net.HttpStatusCode, Counter> HttpResponseStatusCodes
        {
            get
            {
                return mHttpResponseStatusCodes;
            }
        }
        private ConcurrentDictionary<System.Net.HttpStatusCode, Counter> mHttpResponseStatusCodes { get; set; } = new();

        /// <summary>
        /// The last processed response 
        /// </summary>
        public DotHttpResponseDto? PreviousResponse { get; set; }

        /// <summary>
        /// The last processed request
        /// </summary>
        public DotHttpRequestDto? PreviousRequest { get; set; }
        public TestStatus(TestReport testReport)
        {
            TestReport = testReport;
        }
        internal void AddRequestMetrics(HttpRequestMetrics metrics)
        {
            HttpRequests.Increment(1);
            HttpRequestDuration.Log(metrics.HttpRequestDuration.Value);

            var seconds = this.ElapsedSeconds.Value;
            var requests = this.HttpRequests.Value;
            if (seconds > 0)
            {
                HttpRequestsPerSecond.Log(requests / seconds);
            }

            if (!mHttpResponseStatusCodes.ContainsKey(metrics.StatusCode))
            {
                mHttpResponseStatusCodes[metrics.StatusCode] = new Counter($"http_resp_{(int)metrics.StatusCode}", "#");
            }
            mHttpResponseStatusCodes[metrics.StatusCode].Increment(1);

            // Checks/Tests
            var passed = metrics.ChecksPassed.Value;
            var failed = metrics.ChecksFailed.Value;

            HttpBytesSent.Increment(metrics.HttpBytesSent.Value);
            HttpBytesReceived.Increment(metrics.HttpBytesReceived.Value);

            ChecksPassed.Increment(passed);
            ChecksFailed.Increment(failed);

            if (failed > 0)
            {
                TestsFailed.Increment(1);
            }
            else if (passed > 0)
            {
                TestsPassed.Increment(1);
            }


            // Add to stage
            if (CurrentStage != null)
            {
                var stageResult = GetStageResult(CurrentStage.Id);

                // Update the test stage
                stageResult.AddRequestMetrics(metrics);
            }
        }

        internal void AddResult(VerificationCheckResult check)
        {
            if(!check.IsSuccess)
            {
                mFailedChecks.Add(check);
            }
        }

        private StageResult GetStageResult(string id)
        {
            lock (this)
            {
                var plannedStage = TestReport.TestPlan.Stages.Where(x => x.Id == id).FirstOrDefault();
                if (plannedStage == null)
                {
                    throw new Exception($"Did not find planned stage with id '{id}'");
                }

                var testStage = TestReport.Stages.Where(x => x.PlannedStage.Id == plannedStage.Id).FirstOrDefault();
                if (testStage == null)
                {
                    // New stage
                    testStage = new StageResult(plannedStage);
                    TestReport.Stages.Add(testStage);
                }
                return testStage;
            }
        }
    }
}
