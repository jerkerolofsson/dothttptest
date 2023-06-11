using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner
{
    public class TestPlanRunnerOptions
    {
        private List<ITestPlanRunnerProgressHandler> mCallbacks = new List<ITestPlanRunnerProgressHandler>();

        internal ClientOptions ClientOptions { get; set; } = ClientOptions.DefaultOptions();
        internal IReadOnlyList<ITestPlanRunnerProgressHandler> Callbacks => mCallbacks;

        public TestPlanRunnerOptions AddCallback(ITestPlanRunnerProgressHandler callback)
        {
            mCallbacks.Add(callback);   
            return this;
        }
    }
}
