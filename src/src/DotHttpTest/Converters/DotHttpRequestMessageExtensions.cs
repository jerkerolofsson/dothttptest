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
        public static HttpRequestMessage ToHttpRequestMessage(this DotHttpRequest request)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = request.Request.Method,
                RequestUri = request.Request.RequestUri,
                Version = request.Request.Version
            };
            foreach (var v in request.Request.Headers)
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(v.Key, v.Value);
            }

            // Todo: Body

            return httpRequestMessage;
        }
    }
}
