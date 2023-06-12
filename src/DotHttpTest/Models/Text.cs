using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class Text : Expression
    {
        public string Content { get; set; }
        public Text(string content)
        {

            Content = content;  
        }

        public override string ToString()
        {
            return Content;
        }

        public override string? ToString(TestStatus? status)
        {
            return Content;
        }
    }
}
