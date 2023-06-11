using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner
{
    public interface ITestPlanRunnerProgressHandler
    {
        Task OnTestCompletedAsync(TestStatus state)
        {
            return Task.CompletedTask;
        }
        Task OnStageStartedAsync(TestPlanStage stage, TestStatus currentState)
        {
            return Task.CompletedTask;
        }
        Task OnStageCompletedAsync(TestPlanStage stage, TestStatus currentState)
        {
            return Task.CompletedTask;
        }

        Task OnRequestCompletedAsync(DotHttpResponse response, TestStatus currentState)
        {
            return Task.CompletedTask;
        }
        Task OnRequestFailedAsync(DotHttpRequest request, Exception ex)
        {
            return Task.CompletedTask;
        }
    }
}
