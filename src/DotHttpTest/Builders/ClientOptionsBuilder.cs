using DotHttpTest.Providers;
using DotHttpTest.Providers.Json;
using DotHttpTest.Providers.Random;
using DotHttpTest.Providers.State;
using DotHttpTest.Verification.Http;
using DotHttpTest.Verification.Json;
using DotHttpTest.Verification.Mcp;

namespace DotHttpTest.Builders
{
    public class ClientOptionsBuilder
    {
        private readonly List<IVariableProvider> mVariableProviders = new List<IVariableProvider>();
        private readonly List<IVerifier> mVerifiers = new List<IVerifier>();
        private VariableCollection? mVariableCollection = null;
        internal ClientOptions mOptions = new ClientOptions();

        public ClientOptionsBuilder()
        {
            UseMcpVerifiers();
            UseJsonVerifier();
            UseHttpVerifier();
        }

        public ClientOptions Build()
        {
            mOptions.VariableProviders = mVariableProviders.ToList();
            mOptions.Verifiers = mVerifiers.ToList();
            return mOptions;
        }

        public ClientOptionsBuilder WithHttpClientFactory(Func<HttpClient> factory)
        {
            mOptions.HttpClientFactory = factory;
            return this;
        }

        public ClientOptionsBuilder ClearVerifiers()
        {
            mVerifiers.Clear();
            return this;
        }
        public ClientOptionsBuilder WithVerifiers(IVerifier verifier)
        {
            mVerifiers.Add(verifier);
            return this;
        }

        public ClientOptionsBuilder UseMcpVerifiers()
        {
            mVerifiers.Add(new McpVerifier());
            mVerifiers.Add(new McpToolVerifier());
            return this;
        }

        public ClientOptionsBuilder UseJsonVerifier()
        {
            mVerifiers.Add(new JsonVerifier());
            return this;
        }

        public ClientOptionsBuilder UseHttpVerifier()
        {
            mVerifiers.Add(new HttpVerifier());
            mVerifiers.Add(new HttpHeaderVerifier());
            return this;
        }

        public ClientOptionsBuilder ClearVariableProviders()
        {
            mVariableProviders.Clear();
            return this;
        }
        public ClientOptionsBuilder UseDefaultVariableProvider()
        {
            mVariableProviders.Insert(0, new DefaultVariableProvider());
            mVariableProviders.Insert(0, new StateVariableProvider());
            return this;
        }

        public ClientOptionsBuilder WithVariableProvider(IVariableProvider provider)
        {
            mVariableProviders.Insert(0, provider);
            return this;
        }

        public ClientOptionsBuilder UseJsonVariableProvider()
        {
            mVariableProviders.Insert(0, new JsonVariableProvider());
            return this;
        }
        public ClientOptionsBuilder UseDynamicVariableProvider()
        {
            mVariableProviders.Insert(0, new DynamicVariableProvider());
            mVariableProviders.Insert(0, new RandomBytesProvider());
            return this;
        }
        public ClientOptionsBuilder UseEnvironmentVariablesProviders(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            mVariableProviders.Insert(0, new EnvironmentVariableProvider(target));
            return this;
        }
        public ClientOptionsBuilder WithVariable(string key, string val)
        {
            if(mVariableCollection == null)
            {
                mVariableCollection = new();
                mVariableProviders.Insert(0, mVariableCollection);
            }
            mVariableCollection.SetVariableValue(key, val);
            return this;
        }

        public ClientOptionsBuilder WithRequestTimeout(TimeSpan timeout)
        {
            mOptions.Request.Timeout = timeout;
            return this;
        }
    }
}
