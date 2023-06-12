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
    }
}
