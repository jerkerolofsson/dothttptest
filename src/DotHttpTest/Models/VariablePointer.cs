using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class VariablePointer : Expression
    {
        private readonly Expression mOther;

        public VariablePointer(Expression other)
        {
            mOther = other;
        }

        public override IEnumerable<byte> ToByteArray(Encoding encoding, TestStatus? status)
        {
            return mOther.ToByteArray(encoding, status);
        }

        public override string? ToString()
        {
            return mOther.ToString();
        }

        public override string? ToString(TestStatus? status)
        {
            return mOther.ToString(status);
        }
    }
}
