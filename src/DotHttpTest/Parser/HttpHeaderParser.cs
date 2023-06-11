using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Parser
{
    internal class HttpHeaderParser
    {
        internal static void ParseHeader(DotHttpRequest request, string data)
        {
            var p = data.IndexOf(':');
            if(p != -1)
            {
                var name = data.Substring(0, p).Trim();
                var value = data.Substring(p + 1).Trim();

                var httpRequest = request.Request;
                httpRequest.Headers.TryAddWithoutValidation(name, value);
            }
        }
    }
}
