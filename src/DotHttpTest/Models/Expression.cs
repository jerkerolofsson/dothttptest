using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public abstract class Expression
    {
        public abstract string? ToString(TestStatus? status);
        public virtual IEnumerable<byte> ToByteArray(Encoding encoding, TestStatus? status)
        {
            var str = this.ToString(status);
            if(str != null)
            {
                foreach(var b in encoding.GetBytes(str))
                {
                    yield return b;
                }
            }
        }
    }
}
