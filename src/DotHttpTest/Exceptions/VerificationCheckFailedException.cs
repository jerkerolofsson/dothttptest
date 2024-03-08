using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Exceptions
{
    public class VerificationCheckFailedException : Exception
    {
        public VerificationCheckFailedException(string msg) : base(msg) { }
        public VerificationCheckFailedException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}
