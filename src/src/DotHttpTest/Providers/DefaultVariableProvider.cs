using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers
{
    public class DefaultVariableProvider : VariableCollection
    {
        public DefaultVariableProvider()
        {
            SetVariableValue("host", "localhost");
            SetVariableValue("port", "80");
        }
    }
}
