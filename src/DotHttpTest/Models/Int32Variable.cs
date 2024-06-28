using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class Int32Variable : Variable
    {
        public int? Value { get; set; }

        public override string? ToString()
        {
            return Value?.ToString();
        }

        public override string? ToString(TestStatus? status, StageWorkerState? stageWorkerState)
        {
            return Value?.ToString();
        }

    }
}
