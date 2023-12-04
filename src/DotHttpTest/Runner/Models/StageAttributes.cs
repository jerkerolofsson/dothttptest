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
        /// This is the time the stage is active. 
        /// The number of VUs with linearly increase to the target during this time
        /// </summary>
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Number of VUs
        /// </summary>
        public int Target { get; set; } = 1;

        public bool HasDurationOrLoopAttributes
        {
            get
            {
                return Duration > TimeSpan.Zero;
            }
        }
    }
}
