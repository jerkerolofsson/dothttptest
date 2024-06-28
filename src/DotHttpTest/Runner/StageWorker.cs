using DotHttpTest.Runner.Models;
using DotHttpTest.Runner.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHttpTest.Runner
{
    internal class StageWorker : IDisposable
    {
        private readonly TestStatus mTestStatus;
        private readonly DotHttpClient mClient;
        private List<DotHttpRequest> mRequests = new();
        private readonly IReadOnlyList<ITestPlanRunnerProgressHandler> mCallbacks;
        private TestPlanStage? mActiveStage;
        private readonly Stopwatch mTestStopwatch;
        private readonly CancellationTokenSource mCancellationTokenSource;
        private readonly CancellationToken mCancellationToken;
        private readonly ManualResetEventSlim mRunEvent = new ManualResetEventSlim(true);
        private Thread? mThread;
        private StageWorkerState mState;

        public bool IsStageCompleted { get; set; } = false;

        public StageWorker(
            TestStatus testStatus, 
            ClientOptions options,
            IReadOnlyList<ITestPlanRunnerProgressHandler> callbacks,
            TestPlanStage activeStage,
            Stopwatch testStopwatch,
            CancellationToken stoppingToken)
        {
            mTestStatus = testStatus;
            mClient = new DotHttpClient(options);
            mCallbacks = callbacks;
            mActiveStage = activeStage;
            mTestStopwatch = testStopwatch;
            mCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            mCancellationToken = mCancellationTokenSource.Token;
        }

        public void SetRequests(List<DotHttpRequest> requests)
        {
            mRequests = requests;
            if(mRequests.Count == 0)
            {
                mRunEvent.Reset();
            }
            else
            {
                mRunEvent.Set();
            }
        }

        public void Start()
        {
            if (mThread == null)
            {
                mThread = new Thread(this.Run)
                {
                    IsBackground = true,
                    Name = "DotHttp.StageWorker"
                };
                mThread.Start();
            }
        }

        private void Run(object? obj)
        {
            try
            {
                RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception) { }
        }

        public void Stop()
        {
            mCancellationTokenSource.Cancel();
            Join();
            mThread = null;
        }

        public void Join()
        {
            if(mThread != null)
            {
                mThread.Join();
                mThread = null;
            }
        }

        public async Task RunAsync()
        {
            while(!mCancellationToken.IsCancellationRequested)
            {
                if ((mActiveStage?.Attributes?.Iterations != null && mActiveStage.Attributes.Iterations <= mState.LoopCount))
                {
                    // Wait for next stage
                    IsStageCompleted = true;
                    await Task.Delay(TimeSpan.FromSeconds(0.1), mCancellationToken);
                }

                if (!IsStageCompleted)
                {
                    await RunOneIterationAsync();
                    mState.LoopCount++;
                }

                mRunEvent.Wait(mCancellationToken);

                // Just for testing
                if (mRequests.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.1), mCancellationToken);
                }
            }
        }
        

        public async Task RunOneIterationAsync()
        {
            var testStatus = mTestStatus;
            await RunnerUtils.RunOneIterationAsync(mClient, mRequests, testStatus, mState, mCallbacks, mTestStopwatch);
        }

        public void Dispose()
        {
            Stop();
            mClient.Dispose();
        }

        internal void OnStageStarted(TestPlanStage stage)
        {
            // A new stage has started
            mState.LoopCount = 0;
            mActiveStage = stage;
            IsStageCompleted = false;
        }
    }
}
