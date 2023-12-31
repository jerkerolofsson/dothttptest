﻿using DotHttpTest.Runner.Models;
using DotHttpTest.Runner.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner
{
    /// <summary>
    /// This runs a .http file using stage configurations if present
    /// </summary>
    public class TestPlanRunner
    {
        private readonly TestPlan mTestPlan;
        private readonly TestPlanRunnerOptions mTestPlanOptions;

        public TestPlanRunner(TestPlan testPlan, TestPlanRunnerOptions options)
        {
            mTestPlan = testPlan;
            mTestPlanOptions = options;
        }

        public async Task<TestStatus> RunAsync(CancellationToken cancellationToken = default)
        { 
            var testReport = new TestReport(mTestPlan);
            var testStatus = new TestStatus(testReport);
            var options = mTestPlanOptions.ClientOptions;

            using var defaultClient = new DotHttpClient(options);
            var currentClientCount = 0;
            var stopwatch = Stopwatch.StartNew();

            // Initial metrics
            testStatus.UserCount.Log(0);
            testStatus.UserMaxCount.Set(0);

            // Thread pool for all stages for this request
            using var pool = new StageWorkerPool(testStatus, options, mTestPlanOptions.Callbacks, cancellationToken);

            var stages = mTestPlan.Stages.ToList();
            foreach (var stage in stages)
            {
                // Early abort if possible
                cancellationToken.ThrowIfCancellationRequested();

                // Notify that a stage is starting
                testStatus.CurrentStage = stage;
                foreach (var callback in mTestPlanOptions.Callbacks)
                {
                    await callback.OnStageStartedAsync(stage, testStatus);
                }

                if (!stage.Attributes.HasDurationOrLoopAttributes)
                {
                    foreach (var request in stage.Requests)
                    {
                        var basicRequestStage = new TestPlanStage(new StageAttributes()
                        {
                            Name = request.RequestName
                        });
                        mTestPlan.Stages.Add(basicRequestStage);
                        testStatus.CurrentStage = basicRequestStage;

                        // If there is no stage, just send a single message
                        await RunnerUtils.ProcessRequestAsync(defaultClient, request, testStatus, mTestPlanOptions.Callbacks);
                    }
                }
                else
                {
                    await pool.SetRequestsAsync(stage.Requests);

                    var orchestrator = new StageOrchestrator(currentClientCount, stage.Attributes);
                    while (!orchestrator.IsCompleted && !cancellationToken.IsCancellationRequested)
                    {
                        // Refresh the user count at the current time
                        var targetUserCount = orchestrator.GetWantedUserCount();

                        // Change the size of the pool
                        await pool.ResizeAsync(targetUserCount);

                        // Resize the worker pool size for the current number of users
                        currentClientCount = targetUserCount;
                        testStatus.ElapsedSeconds.SetValue(stopwatch.Elapsed.TotalSeconds);
                        UpdateProgress(testStatus);

                        testStatus.UserCount.Log(currentClientCount);
                        testStatus.UserMaxCount.Set(pool.VusMax);

                        // Wait and then check the pool size again
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    }
                }

                testStatus.CurrentStage = null;
                foreach (var callback in mTestPlanOptions.Callbacks)
                {
                    await callback.OnStageCompletedAsync(stage, testStatus);
                }
                currentClientCount = stage.Attributes.Target;
            }

            // Final elapsed timestamp
            testStatus.ElapsedSeconds.SetValue(stopwatch.Elapsed.TotalSeconds);
            foreach (var callback in mTestPlanOptions.Callbacks)
            {
                await callback.OnTestCompletedAsync(testStatus);
            }

            return testStatus;
        }

        private void UpdateProgress(TestStatus testStatus)
        {
            if (mTestPlan.Duration.TotalSeconds > 0)
            {
                var progress = testStatus.ElapsedSeconds.Value / mTestPlan.Duration.TotalSeconds;
                if (progress > 1)
                {
                    progress = 1;
                }
                testStatus.ProgressPercent.SetValue(100 * progress);
            }
        }
    }
}
