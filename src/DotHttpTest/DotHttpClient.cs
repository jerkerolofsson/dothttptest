using DotHttpTest.Converters;
using DotHttpTest.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHttpTest
{
    public class DotHttpClient : IDisposable
    {
        private readonly HttpClient mClient;
        private readonly ClientOptions mOptions;
        private VerifierFactory? mVerifierFactory;

        public DotHttpClient(ClientOptions? options = null)
        {
            mOptions = options ?? ClientOptions.DefaultOptions();
            mClient = mOptions.CreateHttpClient();
            ConfigureHttpClient(mClient, (client) =>
            {
                ConfigureHttpClientWithOptions(client);
            });
        }
        public DotHttpClient(HttpClient httpClient, ClientOptions? options = null)
        {
            mClient = httpClient;
            mOptions = options ?? ClientOptions.DefaultOptions();
            ConfigureHttpClient(mClient, (client) =>
            {
                ConfigureHttpClientWithOptions(client);
            });
        }
        public DotHttpClient(IHttpClientFactory httpClientFactory, string clientName, ClientOptions? options = null)
        {
            mClient = httpClientFactory.CreateClient(clientName);
            mOptions = options ?? ClientOptions.DefaultOptions();
            ConfigureHttpClient(mClient, (client) =>
            {
                ConfigureHttpClientWithOptions(client);
            });
        }

        private void ConfigureHttpClientWithOptions(HttpClient httpClient)
        {
            httpClient.Timeout = mOptions.Request.Timeout;
        }

        private void ConfigureHttpClient(HttpClient httpClient, Action<HttpClient> clientBuilder)
        {
            mVerifierFactory = new VerifierFactory(mOptions);
            clientBuilder(httpClient);
        }

        public List<DotHttpRequest> LoadFile(string filename)
        {
            return DotHttpRequest.FromFile(filename);
        }

        public async Task<DotHttpResponse> SendAsync(DotHttpRequest request, TestStatus? status, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var httpRequestMessage = request.ToHttpRequestMessage(status);

            long requestContentLength = 0;
            if(httpRequestMessage.Content?.Headers?.ContentLength != null)
            {
                requestContentLength = httpRequestMessage.Content.Headers.ContentLength.Value;
            }

            // Create new HTTP metrics
            var metrics = new HttpRequestMetrics()
            {
                RequestUri = httpRequestMessage.RequestUri,
                Created = DateTime.UtcNow,
                Request = request
            };
            var stopwatch = Stopwatch.StartNew();

            // Send the request
            var httpResponse = await mClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var sendingElapsed = stopwatch.Elapsed.TotalSeconds;
            metrics.HttpRequestSending.SetValue(sendingElapsed);
            metrics.StatusCode = httpResponse.StatusCode;

            var response = new DotHttpResponse(httpResponse, metrics);
            response.RequestUri = httpRequestMessage.RequestUri;
            response.Request = request;
            metrics.HttpBytesSent.Increment(EstimateHttpProtocolByteLength(httpRequestMessage));
            metrics.HttpBytesSent.Increment(requestContentLength);

            // Estimate the header byte size
            metrics.HttpBytesReceived.Increment(EstimateHttpProtocolByteLength(response));
            if (mOptions.Request.ReadContent)
            {
                var bytes = await httpResponse.Content.ReadAsByteArrayAsync();
                var elapsed = stopwatch.Elapsed.TotalSeconds;
                metrics.HttpBytesReceived.Increment(bytes.LongLength);

                metrics.HttpRequestReceiving.SetValue(elapsed - sendingElapsed);
                metrics.HttpRequestDuration.SetValue(elapsed);

                response.ContentBytes = bytes;
            }

            mVerifierFactory?.Verify(response);

            return response;
        }

        private long EstimateHttpProtocolByteLength(HttpRequestMessage request)
        {
            long size = 0;
            size += request.Method.Method.Length + 1;
            if (request.RequestUri != null)
            {
                size += request.RequestUri.ToString().Length + 1;
            }
            size += 8; // HTTP/1.1 
            foreach (var header in request.Headers)
            {
                foreach (var val in header.Value)
                {
                    size += header.Key.Length + 2; // :+space
                    size += val.Length;
                    size++; // \n
                }
                size++; // Empty line before content
            }

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    foreach (var val in header.Value)
                    {
                        size += header.Key.Length + 2; // :+space
                        size += val.Length;
                        size++; // \n
                    }
                    size++; // Empty line before content
                }
            }
            return size;
        }

        private long EstimateHttpProtocolByteLength(DotHttpResponse response)
        {
            long size = 0;
            size += 9; // HTTP1.1 + space
            size += 4; // Response code + space
            size += response.ReasonPhrase?.Length ?? 0;
            size++; // \n
            foreach (var header in response.HttpResponse.Headers)
            {
                foreach (var val in header.Value)
                {
                    size += header.Key.Length + 2; // :+space
                    size += val.Length;
                    size++; // \n
                }
                size++; // Empty line before content
            }
            if (response.HttpResponse.Content != null)
            {
                foreach (var header in response.HttpResponse.Content.Headers)
                {
                    foreach (var val in header.Value)
                    {
                        size += header.Key.Length + 2; // :+space
                        size += val.Length;
                        size++; // \n
                    }
                    size++; // Empty line before content
                }
            }

            return size;
        }

        public void Dispose()
        {
            mClient.Dispose();
        }
    }
}
