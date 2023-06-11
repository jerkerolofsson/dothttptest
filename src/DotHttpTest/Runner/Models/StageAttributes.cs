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
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public int Target { get; set; } = 1;
    }
}
