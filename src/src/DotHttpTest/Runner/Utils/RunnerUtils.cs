﻿using DotHttpTest.Runner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner.Utils
{
    internal class RunnerUtils
    {
        internal static async Task RunOneIterationAsync(DotHttpClient client,
            IReadOnlyList<DotHttpRequest> requests,
            TestStatus testStatus,
            IReadOnlyList<ITestPlanRunnerProgressHandler> callbacks)
        {
            // Send a single report and log it
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var request in requests)
            {
                await ProcessRequestAsync(client, request, testStatus, callbacks);
            }
            testStatus.Iterations.Increment(1);
            testStatus.IterationDuration.Log(stopwatch.Elapsed.TotalSeconds);
        }
        internal static async Task ProcessRequestAsync(
            DotHttpClient client,
            DotHttpRequest request,
            TestStatus testStatus,
            IReadOnlyList<ITestPlanRunnerProgressHandler> callbacks)
        {
            try
            {
                var response = await client.SendAsync(request, CancellationToken.None);
                foreach (var check in response.Results)
                {
                    response.Metrics.AddCheck(check);
                    testStatus.AddResult(check);
                }
                
                // Log response metrics
                testStatus.AddRequestMetrics(response.Metrics);
                foreach (var callback in callbacks)
                {
                    await callback.OnRequestCompletedAsync(response, testStatus);
                }

                // Delay if configured for the request
                if(request.DelayAfterRequest > TimeSpan.Zero)
                {
                    await Task.Delay(request.DelayAfterRequest);
                }
            }
            catch (Exception ex)
            {
                var check = new VerificationCheck("http_runner", "request_success", VerificationOperation.Exists, "");
                testStatus.AddResult(new VerificationCheckResult(request, check)
                {
                    IsSuccess = false,
                    Error = $"Request failed: {ex.Message}"
                });
                testStatus.TestsFailed.Increment(1);
                testStatus.HttpRequestFails.Increment(1);

                foreach (var callback in callbacks)
                {
                    await callback.OnRequestFailedAsync(request, ex);
                }
            }
        }
    }
}
