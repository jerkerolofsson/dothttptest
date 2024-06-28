using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers.State
{
    internal class StageIndexVariable : Variable
    {
        public override string? ToString(TestStatus? status, StageWorkerState? stageWorkerState)
        {
            if(status?.CurrentStage is not null)
            {
                return status.CurrentStage.StageIndex.ToString();
            }
            return "0";
        }
    }
}
