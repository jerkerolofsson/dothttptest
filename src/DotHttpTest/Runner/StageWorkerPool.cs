﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner
{
    public class StageWorkerPool : IDisposable
    {
        private SemaphoreSlim mSemaphore = new SemaphoreSlim(1);
        private readonly TestStatus mTestStatus;
        private readonly ClientOptions mOptions;
        private List<DotHttpRequest> mRequests = new();
        private readonly IReadOnlyList<ITestPlanRunnerProgressHandler> mCallbacks;
        private readonly Stopwatch mTestStopwatch;
        private readonly CancellationToken mStoppingToken;
        private List<StageWorker> mActiveWorkers = new List<StageWorker>();
        private List<StageWorker> mIdlePool = new List<StageWorker>();
        private TestPlanStage? mCurrentStage;

        public int VusMax => mActiveWorkers.Count + mIdlePool.Count;

        public StageWorkerPool(
            TestStatus testStatus,
            ClientOptions options,
            IReadOnlyList<ITestPlanRunnerProgressHandler> callbacks,
            Stopwatch testStopwatch,
            CancellationToken stoppingToken) 
        {
            mTestStatus = testStatus;
            mOptions = options;
            mCallbacks = callbacks;
            mTestStopwatch = testStopwatch;
            mStoppingToken = stoppingToken;
        }

        public async Task SetRequestsAsync(List<DotHttpRequest> requests)
        {
            await mSemaphore.WaitAsync();
            try
            {
                mRequests = requests;
                foreach (var worker in mActiveWorkers)
                {
                    worker.SetRequests(requests);
                }
            }
            finally
            {
                mSemaphore.Release();
            }
        }

        public void Dispose()
        {
            foreach (var worker in mActiveWorkers)
            {
                worker.Dispose();
            }
            foreach (var worker in mIdlePool)
            {
                worker.Dispose();
            }
        }

        internal async Task<int> GetNumberOfActiveWorkersAsync()
        {
            await mSemaphore.WaitAsync();
            try
            {
                return mActiveWorkers.Count;
            }
            finally
            {
                mSemaphore.Release();
            }
        }

        public async Task<bool> HasAllWorkersFinishedAsync()
        {
            bool res = false;

            await mSemaphore.WaitAsync();
            try
            {
                foreach(var worker in mActiveWorkers)
                {
                    if(worker.IsStageCompleted)
                    {
                        res = true;
                    }
                }
            }
            finally
            {
                mSemaphore.Release();
            }

            return res;
        }

        public async Task ResizeAsync(int capacity)
        {
            await mSemaphore.WaitAsync();
            try
            {
                ResizePool(capacity);
            }
            finally
            {
                mSemaphore.Release();
            }
        }

        private void ResizePool(int capacity) 
        { 
            if(mCurrentStage == null)
            {
                return;
            }

            var countAdd = capacity - mActiveWorkers.Count;
            var countRemove = mActiveWorkers.Count - capacity;

            if(countRemove > 0)
            {
                for(int i=0; i< countRemove; i++)
                {
                    var index = mActiveWorkers.Count - 1;
                    var worker = mActiveWorkers[index];

                    worker.SetRequests(new()); // Clear the requests

                    // Add to idle-pool and remove from active pool
                    mIdlePool.Add(worker);
                    mActiveWorkers.RemoveAt(index);
                }
            }
            else if(countAdd > 0)
            {
                for (int i = 0; i < countAdd; i++)
                {
                    // Get a worker from the pool if available
                    StageWorker? worker = null;
                    if (mIdlePool.Count > 0)
                    {
                        worker = mIdlePool[0];
                        mIdlePool.RemoveAt(0);
                    }
                    else
                    {
                        worker = new StageWorker(mTestStatus, mOptions, mCallbacks, mCurrentStage, mTestStopwatch, mStoppingToken);
                    }
                    worker.SetRequests(mRequests);
                    worker.Start();
                    mActiveWorkers.Add(worker);
                }
            }
        }

        internal async Task OnStageStartedAsync(TestPlanStage stage)
        {
            await mSemaphore.WaitAsync();
            try
            {
                mCurrentStage = stage;
                foreach(var worker in mActiveWorkers)
                {
                    worker.OnStageStarted(stage);
                }
            }
            finally
            {
                mSemaphore.Release();
            }
        }

    }
}
