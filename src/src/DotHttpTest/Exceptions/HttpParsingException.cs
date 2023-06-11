using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Exceptions
{
    public class HttpParsingException : Exception
    {
        public int LineNumber { get; }

        public HttpParsingException(int lineNumber, string? message) : base(message)
        {
            LineNumber = lineNumber;
        }
        public HttpParsingException(string? message) : base(message)
        {
        }
    }
}
