using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class DotHttpResponse
    {
        internal HttpResponseMessage HttpResponse { get; set; }

        public DotHttpRequest? Request { get; internal set; }

        public HttpStatusCode StatusCode => HttpResponse.StatusCode;
        public bool IsSuccessStatusCode => HttpResponse.IsSuccessStatusCode;
        public string? ReasonPhrase => HttpResponse.ReasonPhrase;
        public HttpContent Content => HttpResponse.Content;
        public byte[]? ContentBytes { get; internal set; }
        public HttpRequestMetrics Metrics { get; init; }
        public List<VerificationCheckResult> Results { get; internal set; }
        public Uri? RequestUri { get; internal set; }

        internal DotHttpResponse(HttpResponseMessage httpResponse, HttpRequestMetrics metrics)        
        {
            Metrics = metrics;
            HttpResponse = httpResponse;
            Results = new();
        }

        public HttpResponseMessage AsHttpResponseMessage()
        {
            return HttpResponse;
        }
    }
}
