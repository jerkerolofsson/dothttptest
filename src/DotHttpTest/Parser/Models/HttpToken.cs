using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Parser.Models
{
    internal class HttpToken
    {
        public HttpTokenType Type { get; }
        public string Data { get; }

        public HttpToken(HttpTokenType type, string data)
        {
            Type = type;
            Data = data;
        }
    }
}
