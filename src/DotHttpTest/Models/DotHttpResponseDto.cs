using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class DotHttpResponseDto
    {
        public required Version Version { get; set; }
        public required List<KeyValuePair<string, List<string>>> Headers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public byte[]? ContentBytes { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public string? ReasonPhrase { get; set; }
    }
}
