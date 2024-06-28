using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers.State
{
    /// <summary>
    /// The actual loop count for a stage worker (thread)
    /// </summary>
    internal class StageWorkerLoopCountVariable : Variable
    {
        public override string? ToString(TestStatus? status, StageWorkerState? stageWorkerState)
        {
            if(stageWorkerState is not null)
            {
                return stageWorkerState.Value.LoopCount.ToString();
            }
            return "0";
        }
    }
}
