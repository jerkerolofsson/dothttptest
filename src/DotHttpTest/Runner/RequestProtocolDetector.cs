using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner
{
    public static class RequestProtocolDetector
    {
        public static RequestProtocol Detect(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState)
        {
            RequestProtocol protocol = RequestProtocol.Http;
            if (request.Version is not null)
            {
                var version = request.Version.ToString(status, stageWorkerState);
                if (version is not null)
                {
                    if (version.StartsWith("MCP"))
                    {
                        protocol = RequestProtocol.Mcp;
                    }
                }
            }
            return protocol;
        }
    }
}
