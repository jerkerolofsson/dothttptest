using DotHttpTest.Metrics;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    /// <summary>
    /// Measurements and result for a one request/response
    /// </summary>
    public class HttpRequestMetrics
    {
        private ConcurrentBag<VerificationCheckResult> mFailedChecks = new();

        public ConcurrentBag<VerificationCheckResult> FailedChecks => mFailedChecks;

        /// <summary>
        /// Timestamp when the metrics was created
        /// </summary>
        public DateTime Created { get; set; }
        public Uri? RequestUri { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public LongCounter ChecksPassed { get; set; } = new("checks_passed", "#");
        public LongCounter ChecksFailed { get; set; } = new("checks_failed", "#");

        public Counter HttpRequestSending { get; set; } = new("http_req_sending", "s");
        public Counter HttpRequestReceiving { get; set; } = new("http_req_receiving", "s");

        /// <summary>
        /// Time from request until the response headers and content has been received
        /// </summary>
        public Counter HttpRequestDuration { get; set; } = new("http_req_duration", "s");
        public LongCounter HttpBytesSent { get; set; } = new("http_bytes_sent", "B");
        public LongCounter HttpBytesReceived { get; set; } = new("http_bytes_received", "B");

        [JsonIgnore]
        public DotHttpRequest? Request { get; internal set; }

        internal void AddCheck(VerificationCheckResult check)
        {
            if(!check.IsSuccess)
            {
                mFailedChecks.Add(check);
            }
        }
    }
}
