using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner.Models
{
    /// <summary>
    /// This represent the status of a single test runner
    /// </summary>
    public record struct StageWorkerState(int LoopCount);
}
