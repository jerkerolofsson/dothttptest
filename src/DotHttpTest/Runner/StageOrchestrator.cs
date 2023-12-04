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
        private readonly Stopwatch mStopwatch;
        private int mLoopCounter = 0;

        public bool IsCompleted
        {
            get
            {
                return mStopwatch.Elapsed >= mStage.Duration;
            }
        }

        public StageOrchestrator(int prevTargetCount, StageAttributes stage)
        {
            mPrevTargetCount = prevTargetCount;
            mStage = stage;
            mStopwatch = Stopwatch.StartNew();
        }

        public int GetWantedUserCount()
        {
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
