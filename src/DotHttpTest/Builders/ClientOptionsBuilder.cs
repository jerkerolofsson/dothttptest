using DotHttpTest.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Builders
{
    public class ClientOptionsBuilder
    {
        private readonly List<IVariableProvider> mVariableProviders = new List<IVariableProvider>();
        private VariableCollection? mVariableCollection = null;
        internal ClientOptions mOptions = new ClientOptions();

        public ClientOptionsBuilder()
        {
        }

        public ClientOptions Build()
        {
            mOptions.VariableProviders = mVariableProviders.ToList();
            return mOptions;
        }

        public ClientOptionsBuilder ClearVariableProviders()
        {
            mVariableProviders.Clear();
            return this;
        }
        public ClientOptionsBuilder UseDefaultVariableProvider()
        {
            mVariableProviders.Insert(0, new DefaultVariableProvider());
            return this;
        }
        public ClientOptionsBuilder UseDynamicVariableProvider()
        {
            mVariableProviders.Insert(0, new DynamicVariableProvider());
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
