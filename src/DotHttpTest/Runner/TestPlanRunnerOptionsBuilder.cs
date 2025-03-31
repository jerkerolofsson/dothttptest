using DotHttpTest.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner
{
    public class TestPlanRunnerOptionsBuilder
    {
        private TestPlanRunnerOptions mRunnerOptions = new TestPlanRunnerOptions();
        private TestPlanBuilder mTestPlanBuilder = new TestPlanBuilder();
        private ClientOptions mClientOptions = ClientOptions.DefaultOptions();
        private ClientOptionsBuilder mClientOptionsBuilder = new ClientOptionsBuilder();

        public TestPlanRunnerOptionsBuilder ConfigureClientOptions(Action<ClientOptionsBuilder> builder)
        {
            builder(mClientOptionsBuilder);
            mClientOptions = mClientOptionsBuilder.Build();
            return this;
        }

        public TestPlanRunnerOptionsBuilder AddCallback(ITestPlanRunnerProgressHandler callback)
        {
            mRunnerOptions.AddCallback(callback);
            return this;
        }

        public TestPlanRunnerOptionsBuilder LoadHttpFile(string httpFilePath)
        {
            return ConfigureTestPlan((builder, options) => {
                builder.LoadHttpFile(httpFilePath, options);
            });
        }
        /// <summary>
        /// Loads all requests and stages from the .http file, configuring each request with the builder
        /// </summary>
        /// <param name="httpFilePath"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public TestPlanRunnerOptionsBuilder LoadHttpFile(string httpFilePath, Action<DotHttpRequestBuilder> configureTest)
        {
            return ConfigureTestPlan((builder, options) => {
                builder.LoadHttpFile(httpFilePath, configureTest, options);
            });
        }
     

        public TestPlanRunnerOptionsBuilder ConfigureTestPlan(Action<TestPlanBuilder, ClientOptions> testPlanConfigurator)
        {
            testPlanConfigurator(mTestPlanBuilder, mClientOptions);
            return this;
        }

        public TestPlanRunner Build()
        {
            mRunnerOptions.ClientOptions = mClientOptions;
            var runner = new TestPlanRunner(mTestPlanBuilder.Build(), mRunnerOptions);
            return runner;
        }

    }
}
