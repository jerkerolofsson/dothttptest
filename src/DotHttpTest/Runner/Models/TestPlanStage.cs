using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner.Models
{
    /// <summary>
    /// This is stage which is created from a .http file.
    /// </summary>
    public class TestPlanStage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Attributes that define the user count and duration for this stage
        /// </summary>
        public StageAttributes Attributes { get; }

        /// <summary>
        /// List of all requests that will be performed within this stage
        /// </summary>
        public List<DotHttpRequest> Requests { get; set; } = new();
        public int StageIndex { get; internal set; }

        public TestPlanStage(StageAttributes stage)
        {
            this.Attributes = stage;
        }
    }
}
