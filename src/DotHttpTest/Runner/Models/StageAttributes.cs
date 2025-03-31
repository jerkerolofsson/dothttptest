using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner.Models
{
    /// <summary>
    /// This POCO defines the stage properties assigned to a request
    /// </summary>
    public class StageAttributes
    {
        public string? Name { get; set; }

        /// <summary>
        /// Identifier for a test
        /// </summary>
        public string? TestId { get; set; }

        /// <summary>
        /// This is the time the stage is active. 
        /// The number of VUs with linearly increase to the target during this time
        /// </summary>
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// This is the target number of iterations before the stage stops.
        /// This count applies for each VU
        /// If both Duration and TargetRequestCount is set the stage will end when either of them evaluates to stop the stage
        /// </summary>
        public int? Iterations { get; set; }

        /// <summary>
        /// Number of VUs
        /// </summary>
        public int Target { get; set; } = 1;

        public bool HasDurationOrLoopAttributes
        {
            get
            {
                if(Duration > TimeSpan.Zero)
                {
                    return true;
                }
                if(Iterations != null && Iterations >= 1)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
