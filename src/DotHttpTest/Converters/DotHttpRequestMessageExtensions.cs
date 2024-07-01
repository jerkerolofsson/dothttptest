using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Converters
{
    public static class DotHttpRequestMessageExtensions
    {
        /// <summary>
        /// Returns the request as a System.Net.Http.HttpRequestMessage
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HttpRequestMessage ToHttpRequestMessage(this DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState)
        {
            ArgumentNullException.ThrowIfNull(request.Method);
            ArgumentNullException.ThrowIfNull(request.Url);
            ArgumentNullException.ThrowIfNull(request.Version);

            ByteArrayContent? content = null;
            if(request.Body != null)
            {
                var bytes = request.Body.ToByteArray(Encoding.UTF8, status, stageWorkerState);
                if (bytes.Length > 0)
                {
                    // Save the generated content
                    request.ContentBytes = bytes;
                    content = new ByteArrayContent(bytes);
                    content.Headers.ContentLength = bytes.Length;
                }
            }

            var httpVersionNumber = request.Version.ToString(status, stageWorkerState);
            if (httpVersionNumber.StartsWith("HTTP/"))
            {
                httpVersionNumber = httpVersionNumber[5..];
            }

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = new HttpMethod(request.Method.ToString(status, stageWorkerState)),
                RequestUri = new Uri(request.Url.ToString(status, stageWorkerState)),
                Version = new Version(httpVersionNumber),
                Content = content
            };
            foreach (var headerExpression in request.Headers)
            {
                var headerLine = headerExpression.ToString(status, stageWorkerState);
                var p = headerLine.IndexOf(':');
                var key = headerLine;
                var val = "";
                if(p != -1)
                {
                    key = headerLine.Substring(0, p).Trim();
                    val = headerLine.Substring(p+1).Trim();
                }

                if(!httpRequestMessage.Headers.TryAddWithoutValidation(key,val))
                {
                    if (httpRequestMessage.Content != null)
                    {
                        httpRequestMessage.Content.Headers.TryAddWithoutValidation(key, val);
                    }
                }
            }

            return httpRequestMessage;
        }
    }
}
