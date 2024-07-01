using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class DotHttpRequestDto
    {
        public required string Method { get; set; }
        public required Uri? RequestUri { get; set; }
        public required Version Version { get; set; }
        public required List<KeyValuePair<string, List<string>>> Headers { get; set; }
        public byte[]? ContentBytes { get; set; }
    }
}
