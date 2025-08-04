using DotHttpTest.Runner.Utils;
using System.Diagnostics;

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
            return await RunAsync(null, cancellationToken);
        }

        public async Task<TestStatus> VerifyAsync(CancellationToken cancellationToken = default)
        {
            var result = await RunAsync(null, cancellationToken);

            if(result is null)
            {
                throw new VerificationCheckFailedException("RunAsync returned null result");
            }
            if (result.FailedChecks.Count > 0)
            {
                var failures = string.Join(",", result.FailedChecks.Select(x => x.Error));
                throw new VerificationCheckFailedException($"{result.FailedChecks.Count} failed checks: {failures}");
            }
            return result;
        }
        public async Task<TestStatus> RunAsync(Action<TestStatus>? testStatusCreatedCallback, CancellationToken cancellationToken = default)
        {
            var testReport = new TestReport(mTestPlan);
            var testStatus = new TestStatus(testReport);
            var options = mTestPlanOptions.ClientOptions;
            if(testStatusCreatedCallback != null)
            {
                testStatusCreatedCallback(testStatus);
            }

            // Initialize variable providers
            if (mTestPlanOptions.ClientOptions?.VariableProviders is not null)
            {
                foreach (var variableProvider in mTestPlanOptions.ClientOptions.VariableProviders)
                {
                    await variableProvider.InitAsync();
                }
            }

            using var defaultClient = new DotHttpClient(options);
            var currentClientCount = 0;
            var testStopwatch = Stopwatch.StartNew();

            //Thread.Sleep(8000);

            // Initial metrics
            testStatus.UserCount.Log(0);
            testStatus.UserMaxCount.Set(0);

            // Thread pool for all stages for this request
            using var pool = new StageWorkerPool(testStatus, options, mTestPlanOptions.Callbacks, testStopwatch, cancellationToken);

            var stages = mTestPlan.Stages.ToList();
            int stageIndex = 0;
            foreach (var stage in stages)
            {
                stage.StageIndex = stageIndex;
                await pool.OnStageStartedAsync(stage);

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
                    // Single user
                    testStatus.UserCount.Log(1);
                    testStatus.UserMaxCount.Set(1);
                    StageWorkerState stageWorkerState = new();

                    foreach (var request in stage.Requests)
                    {
                        var basicRequestStage = new TestPlanStage(new StageAttributes()
                        {
                            Name = request.RequestName,
                            TestId = request.TestId
                        });
                        mTestPlan.Stages.Add(basicRequestStage);
                        testStatus.CurrentStage = basicRequestStage;

                        // If there is no stage, just send a single message
                        await RunnerUtils.ProcessRequestAsync(defaultClient, request, testStatus, stageWorkerState, mTestPlanOptions.Callbacks, testStopwatch);
                    }
                }
                else
                {
                    await pool.SetRequestsAsync(stage.Requests);

                    var orchestrator = new StageOrchestrator(currentClientCount, stage.Attributes, stages.Count, pool);
                    while (!await orchestrator.IsCompletedAsync() && !cancellationToken.IsCancellationRequested)
                    {
                        // Refresh the user count at the current time
                        var targetUserCount = orchestrator.GetWantedUserCount();

                        // Change the size of the pool of VUs
                        // The number of VUs will linearly scale between stages.
                        // If only a single stage is set, the VU count will be the target count for the whole stage.
                        await pool.ResizeAsync(targetUserCount);

                        // Resize the worker pool size for the current number of users
                        currentClientCount = targetUserCount;
                        testStatus.ElapsedSeconds.SetValue(testStopwatch.Elapsed.TotalSeconds);
                        UpdateProgress(testStatus);

                        testStatus.UserCount.Log(currentClientCount);
                        testStatus.UserMaxCount.Set(pool.VusMax);

                        // Wait and then check the pool size again
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    }
                }
                testStatus.ElapsedSeconds.SetValue(testStopwatch.Elapsed.TotalSeconds);
                UpdateProgress(testStatus);
                testStatus.CurrentStage = null;
                foreach (var callback in mTestPlanOptions.Callbacks)
                {
                    await callback.OnStageCompletedAsync(stage, testStatus);
                }
                currentClientCount = stage.Attributes.Target;
            }

            // Final elapsed timestamp
            testStatus.ElapsedSeconds.SetValue(testStopwatch.Elapsed.TotalSeconds);
            foreach (var callback in mTestPlanOptions.Callbacks)
            {
                await callback.OnTestCompletedAsync(testStatus);
            }

            testStatus.IsCompleted = true;

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
            else
            {
                // Loop counts?
                var total = mTestPlan.ExpectedIterations;
                if (total > 0)
                {
                    var progress = testStatus.Iterations.Value * 100.0 / total;
                    testStatus.ProgressPercent.SetValue(progress);
                }
                else
                {
                    testStatus.ProgressPercent.SetValue(100);
                }
            }
        }
    }
}
