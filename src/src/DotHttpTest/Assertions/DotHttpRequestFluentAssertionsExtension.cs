using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Assertions
{
    public static class DotHttpRequestFluentAssertionsExtension
    {
        public static DotHttpRequestFluentAssertions Should(this DotHttpRequest instance)
        {
            return new DotHttpRequestFluentAssertions(instance);
        }
    }
}
