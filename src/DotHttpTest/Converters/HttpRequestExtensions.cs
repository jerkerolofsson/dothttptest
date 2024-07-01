using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Converters
{
    public static class HttpRequestExtensions
    {
        public static DotHttpRequestDto ToDto(this HttpRequestMessage message)
        {
            DotHttpRequestDto dto = new DotHttpRequestDto
            {
                Method = message.Method.ToString(),
                RequestUri = message.RequestUri,
                Version = message.Version,
                Headers = MapHeaders(message.Headers),
            };
            if(message.Content?.Headers is not null)
            {
                foreach(var header in MapHeaders(message.Content.Headers))
                {
                    dto.Headers.Add(header);
                }
            }
            return dto;
        }

        public static List<KeyValuePair<string, List<string>>> MapHeaders(HttpHeaders headers)
        {
            List<KeyValuePair<string, List<string>>> output = new List<KeyValuePair<string, List<string>>>();
            foreach(var header in headers)
            {
                List<string> values = new List<string>(header.Value);
                var outputHeader = new KeyValuePair<string, List<string>>(header.Key, values);
                output.Add(outputHeader);

            }
            return output;
        }
    }
}
