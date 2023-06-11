using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResponseVerifierAttribute : Attribute
    {
        public string Name { get; }

        public ResponseVerifierAttribute(string name)
        {
            Name = name;
        }
    }
}
