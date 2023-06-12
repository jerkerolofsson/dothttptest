using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Parser
{
    internal class HttpHeaderParser
    {
        internal static void ParseHeader(DotHttpRequest request, ExpressionList headerLine)
        {
            request.Headers.Add(headerLine);
        }
    }
}
