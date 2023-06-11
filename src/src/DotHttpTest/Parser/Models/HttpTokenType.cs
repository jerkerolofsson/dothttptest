using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Parser.Models
{
    internal enum HttpTokenType
    {
        Unknown,

        Comment,
        BlankLine,

        VariableAssignmentLine,

        Method,
        Url,
        HttpVersion,
        HeaderLine,
        BodyLine,

        RequestSeparator
    }
}
