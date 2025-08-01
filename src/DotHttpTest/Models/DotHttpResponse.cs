using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace DotHttpTest.Models
{
    public class DotHttpResponse
    {
        internal HttpResponseMessage HttpResponse { get; set; }

        [JsonIgnore]
        public DotHttpRequest? Request { get; internal set; }

        public HttpStatusCode StatusCode => HttpResponse.StatusCode;
        public bool IsSuccessStatusCode => HttpResponse.IsSuccessStatusCode;
        public string? ReasonPhrase => HttpResponse.ReasonPhrase;

        [JsonIgnore]
        public HttpContent Content => HttpResponse.Content;

        /// <summary>
        /// Response headers
        /// </summary>
        public HeadersCollection Headers
        {
            get;
            set;
        }

        /// <summary>
        /// Bytes from the response
        /// </summary>
        public byte[]? ContentBytes { get; internal set; }

        /// <summary>
        /// Measurements
        /// </summary>
        public HttpRequestMetrics Metrics { get; init; }

        /// <summary>
        /// Results from checks
        /// </summary>
        public ConcurrentBag<VerificationCheckResult> Results { get; internal set; }

        /// <summary>
        /// The request URI
        /// </summary>
        public Uri? RequestUri { get; internal set; }

        internal DotHttpResponse(HttpResponseMessage httpResponse, HttpRequestMetrics metrics)        
        {
            Metrics = metrics;
            HttpResponse = httpResponse;
            Results = new();
            Headers = new HeadersCollection(httpResponse);
        }

        public HttpResponseMessage AsHttpResponseMessage()
        {
            return HttpResponse;
        }
    }
}
