using DotHttpTest.Builders;
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
        public IReadOnlyList<IVariableProvider>? VariableProviders { get; internal set; }

        public RequestOptions Request { get; set; } = new();

        /// <summary>
        /// Creates default client options
        /// </summary>
        /// <returns></returns>
        public static ClientOptions DefaultOptions()
        {
            return new ClientOptionsBuilder()
                .ClearVariableProviders()
                .UseDefaultVariableProvider()
                .UseDynamicVariableProvider()
                .UseEnvironmentVariablesProviders()
                .WithRequestTimeout(TimeSpan.FromSeconds(30))
                .Build();
        }
    }
}
