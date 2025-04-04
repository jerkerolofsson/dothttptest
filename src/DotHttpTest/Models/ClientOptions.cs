﻿using DotHttpTest.Builders;
using DotHttpTest.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class ClientOptions
    {
        /// <summary>
        /// If false, and exception is thrown if the variable is missing
        /// </summary>
        public bool IgnoreMissingVariables { get; set; } = false;

        /// <summary>
        /// Variable providers used
        /// </summary>
        public IReadOnlyList<IVariableProvider>? VariableProviders { get; internal set; }

        /// <summary>
        /// Verifiers used
        /// </summary>
        public IReadOnlyList<IVerifier>? Verifiers { get; internal set; }

        public RequestOptions Request { get; set; } = new();

        public Func<HttpClient>? HttpClientFactory { get; set; } = null;

        public HttpClient CreateHttpClient()
        {
            if(HttpClientFactory != null)
            {
                return HttpClientFactory();
            }

            return new HttpClient();
        }

        /// <summary>
        /// Creates default client options
        /// </summary>
        /// <returns></returns>
        public static ClientOptions DefaultOptions()
        {
            return new ClientOptionsBuilder()
                .ClearVariableProviders()
                .UseHttpVerifier()
                .UseJsonVerifier()
                .ClearVariableProviders()
                .UseDefaultVariableProvider()
                .UseDynamicVariableProvider()
                .UseJsonVariableProvider()
                .UseEnvironmentVariablesProviders()
                .WithRequestTimeout(TimeSpan.FromSeconds(30))
                .Build();
        }
    }
}
