using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Parser.Models
{
    public enum TokenizerState
    {
        ExpectedRequestHeader,
        ExpectHeaders,
        ExpectBody
    }
}
