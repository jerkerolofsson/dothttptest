using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Integration.Models
{

    public class HttpBinPostResponse
    {
        public Args? args { get; set; }
        public string? data { get; set; }
        public Files? files { get; set; }
        public Form? form { get; set; }
        public Headers? headers { get; set; }
        public Dictionary<string, string>? json { get; set; }
        public string? origin { get; set; }
        public string? url { get; set; }
    }

    public class Args
    {
    }

    public class Files
    {
    }

    public class Form
    {
    }

    public class Headers
    {
        public string? ContentLength { get; set; }
        public string? ContentType { get; set; }
        public string? Host { get; set; }
    }

}
