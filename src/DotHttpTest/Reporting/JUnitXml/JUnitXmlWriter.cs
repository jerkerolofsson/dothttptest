using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DotHttpTest.Reporting.JUnitXml
{
    /// <summary>
    /// Creates a JUnitXml report
    /// </summary>
    public class JUnitXmlWriter : ITestPlanRunnerProgressHandler
    {
        private readonly string mPath;

        public JUnitXmlWriter(string path)
        {
            mPath = path;
        }

        public async Task OnTestCompletedAsync(TestStatus state)
        {
            var xml = GetXml(state);
            await File.WriteAllTextAsync(mPath, xml);
        }

        public string GetXml(TestStatus status)
        {
            var doc = new XmlDocument();

            var testSuites = doc.CreateElement("testsuites");
            doc.AppendChild(testSuites);

            var report = status.TestReport;
            var plan = status.TestReport.TestPlan;

            //testSuites.SetAttribute("id", campaignId);
            //testSuites.SetAttribute("name", campaignName);
            var testCount = report.Stages.Count;
            var failures = report.Stages.Where(x=>x.FailCount > 0).Count();
            testSuites.SetAttribute("tests", testCount.ToString());
            testSuites.SetAttribute("failures", failures.ToString());
            testSuites.SetAttribute("name", plan.Name);
            testSuites.SetAttribute("id", plan.Name);
            testSuites.SetAttribute("time", FormatTime(TimeSpan.FromSeconds(status.ElapsedSeconds.Value)));

            var testSuite = doc.CreateElement("testsuite");
            testSuites.AppendChild(testSuite);
            testSuite.SetAttribute("name", plan.Name);
            testSuite.SetAttribute("id", plan.Name);
            testSuite.SetAttribute("tests", testCount.ToString());
            testSuite.SetAttribute("failures", failures.ToString());

            foreach (var stage in report.Stages)
            {
                var passed = stage.FailCount > 0 ? 0 : 1;
                var failed = stage.FailCount > 0 ? 1 : 0;

                var testCase = doc.CreateElement("testcase");
                testCase.SetAttribute("name", stage.PlannedStage.Attributes.Name);
                if (stage.PlannedStage.Attributes.TestId is not null)
                {
                    testCase.SetAttribute("id", stage.PlannedStage.Attributes.TestId);
                }
                else
                {
                    testCase.SetAttribute("id", stage.PlannedStage.Attributes.Name);
                }
                testSuite.AppendChild(testCase);

                var failedChecks = new List<VerificationCheckResult>();
                foreach (var result in stage.Results)
                {
                    failedChecks.AddRange(result.FailedChecks);
                }
                if (failedChecks.Count > 0)
                {
                    var failure = doc.CreateElement("failure");
                    failure.SetAttribute("type", "failure");

                    var sb = new StringBuilder();
                    foreach (var check in failedChecks)
                    {
                        sb.AppendLine(check.Error);
                    }
                    failure.SetAttribute("message", sb.ToString());
                    testCase.AppendChild(failure);
                }
            }

            return doc.OuterXml;
        }
        private string FormatTime(TimeSpan duration)
        {
            var numberFormat = new System.Globalization.NumberFormatInfo();
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.NumberGroupSeparator = "";

            var seconds = duration.TotalSeconds;
            return Convert.ToString(seconds, numberFormat);
        }
    }
}
