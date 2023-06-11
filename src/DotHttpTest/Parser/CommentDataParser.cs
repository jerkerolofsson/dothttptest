using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Parser
{
    internal static class CommentDataParser
    {
        internal static void ParseCommentMetadata(DotHttpRequest request, string data)
        {
            var valueAfter = TrimCommentHead(data.Trim());
            if (valueAfter != null)
            {
                if (valueAfter.StartsWith('@'))
                {
                    var p = valueAfter.IndexOf(' ');
                    if (p == -1)
                    {
                        request.SetMeta(valueAfter);
                    }
                    else
                    {
                        var name = valueAfter.Substring(1, p).Trim(); // skip @
                        var val = valueAfter.Substring(p + 1);
                        request.SetMeta(name, val);
                    }
                }
            }
        }

        private static string? TrimCommentHead(string commentLine)
        {
            if (commentLine.StartsWith("#"))
            {
                return commentLine.TrimStart('#').Trim();
            }
            if (commentLine.StartsWith("//"))
            {
                return commentLine.TrimStart('/').Trim();
            }
            return null;
        }
    }
}
