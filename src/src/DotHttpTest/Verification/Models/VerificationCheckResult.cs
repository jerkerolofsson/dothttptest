using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification.Models
{
    public class VerificationCheckResult
    {
        public DotHttpRequest Request { get; set; }

        /// <summary>
        /// The actual value from the response
        /// </summary>
        public string? ActualValue { get; internal set; }

        public bool IsSuccess { get; set; } = false;
        public string? Error { get; set; }
        public VerificationCheck Check { get; set; }
        public VerificationCheckResult(DotHttpRequest request, VerificationCheck check)
        {
            Request = request;
            Check = check;
        }
    }
}
