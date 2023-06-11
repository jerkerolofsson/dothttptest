
namespace DotHttpTest.Runner.Models
{
    public class TestReport
    {
        public TestPlan TestPlan { get; }

        public List<StageResult> Stages { get; set; } = new();

        public TestReport(TestPlan testPlan)
        {
            TestPlan = testPlan;
        }

    }
}
