using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner
{
    internal class StageOrchestrator
    {
        private readonly int mPrevTargetCount;
        private readonly StageAttributes mStage;
        private readonly int mTotalStageCount;
        private readonly StageWorkerPool mPool;
        private readonly Stopwatch mStopwatch;

        /// <summary>
        /// Returns true if the stage is completed.
        /// 
        /// Either the target duration has passed, or the number of iterations per worker has completed
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsCompletedAsync()
        {
            bool isCompleted = false;
            if (mStage.Duration != TimeSpan.Zero)
            {
                if (mStopwatch.Elapsed >= mStage.Duration)
                {
                    isCompleted = true;
                }
            }

            if (mStage.Iterations != null)
            {
                // If we have no workers HasAllWorkersFinishedAsync will return true
                // But as the number of workers is dynamic, we will only check if the target number of workers is reached
                if (mStage.Target == await mPool.GetNumberOfActiveWorkersAsync())
                {
                    if (await mPool.HasAllWorkersFinishedAsync())
                    {
                        isCompleted = true;
                    }
                }
            }
                
            return isCompleted;
        }

        public StageOrchestrator(int prevTargetCount, StageAttributes stage, int totalStageCount, StageWorkerPool pool)
        {
            mPrevTargetCount = prevTargetCount;
            mStage = stage;
            mTotalStageCount = totalStageCount;
            mPool = pool;
            mStopwatch = Stopwatch.StartNew();
        }

        public int GetWantedUserCount()
        {
            if(mTotalStageCount == 1)
            {
                return mStage.Target;
            }

            var from = mPrevTargetCount;
            var target = mStage.Target;
            var range = target - from;

            var progress = mStopwatch.Elapsed.TotalSeconds / mStage.Duration.TotalSeconds;
            if(progress > 1.0)
            {
                return mStage.Target;
            }

            var userCount = (int)Math.Round(mPrevTargetCount + (progress * range));
            return userCount;
        }
    }
}
