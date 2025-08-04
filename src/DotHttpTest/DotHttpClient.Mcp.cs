using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotHttpTest
{
    public partial class DotHttpClient
    {
        private async Task<DotHttpResponse> SendMcpAsync(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState, CancellationToken cancellationToken)
        {
            if (request.Method is null)
            {
                throw new ArgumentNullException("Method cannot be null");
            }

            var method = request.Method.ToString(status, stageWorkerState);
            switch (method)
            {
                case "CALL":
                    return await InvokeMcpAsync(request, status, stageWorkerState, cancellationToken);
                case "LIST":
                    return await ListMcpAsync(request, status, stageWorkerState, cancellationToken);
                default:
                    throw new Exception($"Unknown method: '{method}' for protocol 'MCP'");
            }
        }

        private async Task<DotHttpResponse> InvokeMcpAsync(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState, CancellationToken cancellationToken)
        {
            if (request.Url is null)
            {
                throw new ArgumentNullException("Method cannot be null");
            }
            var endpoint = request.Url.ToString(status, stageWorkerState);

            if (!endpoint.Contains('#'))
            {
                throw new ArgumentException("Expected tool name to be specified in request URL, e.g. http://mcpendpoint/mcp#name_of_tool");
            }
            var endpointComponents = endpoint.Split('#');
            var url = endpointComponents[0];
            var tool = endpointComponents[1];

            IMcpClient client = await CreateMcpClientAsync(request, status, stageWorkerState);

            var metrics = new HttpRequestMetrics();
            IReadOnlyDictionary<string, object?>? arguments = ParseArguments(request);

            var result = await client.CallToolAsync(tool, arguments, null, ModelContextProtocol.McpJsonUtilities.DefaultOptions, cancellationToken);

            // Wrap in HTTP response
            var httpResponse = new HttpResponseMessage()
            {
                StatusCode = result.IsError switch
                {
                    false => HttpStatusCode.OK,
                    _ => HttpStatusCode.BadGateway,
                }
            };
            return new DotHttpResponse(httpResponse, metrics)
            {
                CallToolResult = result,
                Request = request
            };
        }

        private IReadOnlyDictionary<string, object?>? ParseArguments(DotHttpRequest request)
        {
            var json = request.GetBodyAsText();
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(json);
        }

        private async Task<DotHttpResponse> ListMcpAsync(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState, CancellationToken cancellationToken)
        {
            IMcpClient client = await CreateMcpClientAsync(request, status, stageWorkerState);
            var metrics = new HttpRequestMetrics();

            // Print the list of tools available from the server.
            List<Tool> tools = [];
            var timestamp = Stopwatch.GetTimestamp();
            foreach (var tool in await client.ListToolsAsync())
            {
                tools.Add(tool.ProtocolTool);
            }
            metrics.HttpRequestDuration.SetValue(Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds);

            string json = JsonSerializer.Serialize(tools, ModelContextProtocol.McpJsonUtilities.DefaultOptions);

            var httpResponse = new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return new DotHttpResponse(httpResponse, metrics)
            {
                ContentBytes = Encoding.UTF8.GetBytes(json),
                Request = request
            };

        }

        private async Task<IMcpClient> CreateMcpClientAsync(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState)
        {
            if (request.Url is null)
            {
                throw new ArgumentNullException("Method cannot be null");
            }

            Dictionary<string, string> headers = GetHttpHeaders(request, status, stageWorkerState);
            HttpTransportMode mode = GetMcpHttpTransportMode(request, status, stageWorkerState);

            var endpoint = request.Url.ToString(status, stageWorkerState);
            var clientTransport = new SseClientTransport(new SseClientTransportOptions
            {
                Endpoint = new Uri(endpoint),
                AdditionalHeaders = headers,
                TransportMode = mode,
            });

            var client = await McpClientFactory.CreateAsync(clientTransport);
            return client;
        }

        private static Dictionary<string, string> GetHttpHeaders(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState)
        {
            Dictionary<string, string> headers = [];
            foreach (var header in request.Headers)
            {
                var name = header.ToString(status, stageWorkerState).TrimEnd('\r');
                var p = name.IndexOf(':');
                if (p > 0)
                {
                    var key = name[0..p];
                    var val = name[(p + 1)..];
                    headers.Add(key, val);
                }
            }

            return headers;
        }

        private static HttpTransportMode GetMcpHttpTransportMode(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState)
        {
            HttpTransportMode mode = HttpTransportMode.AutoDetect;
            if (request.Version is not null)
            {
                var version = request.Version.ToString(status, stageWorkerState);
                if (version.StartsWith("MCP/"))
                {
                    version = version[4..];
                    if (version.StartsWith("SSE"))
                    {
                        mode = HttpTransportMode.Sse;
                    }
                    if (version.StartsWith("HTTP"))
                    {
                        mode = HttpTransportMode.StreamableHttp;
                    }
                }
            }

            return mode;
        }
    }
}
