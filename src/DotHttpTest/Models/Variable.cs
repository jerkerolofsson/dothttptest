using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public abstract class Variable : Expression
    {
        public override string? ToString()
        {
            return base.ToString();
        }
    }
}
