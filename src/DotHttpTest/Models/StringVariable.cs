using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class StringVariable : Variable
    {
        public string? Value { get; set; }

        public override string? ToString()
        {
            return Value;
        }

        public override string? ToString(TestStatus? status, StageWorkerState? stageWorkerState)
        {
            return Value;
        }

    }
}
