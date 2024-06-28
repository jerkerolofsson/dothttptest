using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification.Models
{
    public enum VerificationOperation
    {
        Exists,
        NotExists,

        DateEquals,
        DateNotEquals,
        Equals,
        NotEquals,

        GreaterOrEquals,
        LessOrEquals,
        Less,
        Greater,

        RegexMatch
    }
}
