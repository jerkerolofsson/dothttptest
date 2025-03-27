using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class HeadersCollection : List<Header>
    {
        public HeadersCollection() { }
        public HeadersCollection(HttpResponseMessage message)
        {
            if (message.Headers != null)
            {
                foreach (var header in message.Headers)
                {
                    Add(new Header(header.Key, header.Value.ToList()));
                }
            }
            if (message.Content?.Headers != null)
            {
                foreach (var header in message.Content.Headers)
                {
                    Add(new Header(header.Key, header.Value.ToList()));
                }
            }
        }
    }
}
