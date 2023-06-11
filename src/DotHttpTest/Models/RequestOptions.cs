using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class RequestOptions
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

        /// <summary>
        /// If true, the HTTP payload will be read
        /// </summary>
        public bool ReadContent { get; set; } = true;
    }
}
