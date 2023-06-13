using DotHttpTest.Metrics;
using DotHttpTest.Models;
using DotHttpTest.Runner;
using DotHttpTest.Runner.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text = Spectre.Console.Text;

namespace dothttp.Handlers
{
    internal class ConsoleDotHttpRunnerProgressHandler : ITestPlanRunnerProgressHandler, IDisposable
    {
        private Color AccentBlue => new Color(50, 200, 255);
        private Color AccentPurple => new Color(170, 50, 225);


        private DateTime mTimeLastReport = DateTime.MinValue;
        private CancellationTokenSource mCancellationTokenSource = new CancellationTokenSource();
        private ManualResetEventSlim mRefreshEvent = new ManualResetEventSlim();

        public Layout Layout { get; }

        private Thread mRenderThread;

        public ConsoleDotHttpRunnerProgressHandler()
        {
            var layout = new Layout("Root")
            .SplitColumns(
                new Layout("Left")
                    .SplitRows(
                            new Layout("Progress"),
                            new Layout("Stage"),
                            new Layout("Users"),
                            new Layout("ResponseStatusCodes"),
                            new Layout("RequestsPerSecond")
                    ),
                new Layout("Metrics")
                );
            this.Layout = layout;

            mRenderThread = new Thread(() =>
            {
                try
                {
                    AnsiConsole.Live(layout).Start(ctx =>
                    {
                        while (!mCancellationTokenSource.IsCancellationRequested)
                        {
                            ctx.Refresh();
                            mRefreshEvent.Wait(mCancellationTokenSource.Token);
                            mRefreshEvent.Reset();
                        }
                    });
                }
                catch (Exception) { }
            });
            mRenderThread.IsBackground = true;
            mRenderThread.Start();

        }
        public Task OnStageStartedAsync(TestPlanStage stage, TestStatus currentState)
        {
            AnsiConsole.MarkupLine($"Starting stage [cyan]{stage.Attributes.Name}[/] ({stage.Requests.Count} requests)");
            var table = new Table();
            table.Expand();
            table.AddColumn("Name");
            table.AddColumn("Method");
            table.AddColumn("URL");
            foreach (var request in stage.Requests)
            {
                try
                {
                    var url = request.Url == null ? "" : request.Url.ToString(currentState);
                    table.AddRow(request.RequestName ?? "", request.Method.ToString(currentState), url);
                }
                catch (InvalidOperationException) 
                { 
                    // If a variable is part of the request URL/Method but we don't yet know the value,
                    // the URL cannot be converted to string..
                }
            }

            Layout["Stage"].Update(table);
            mRefreshEvent.Set();

            return Task.CompletedTask;
        }
        public Task OnRequestCompletedAsync(DotHttpResponse response, TestStatus testStatus)
        {
            var elapsed = DateTime.Now - mTimeLastReport;
            if(elapsed > TimeSpan.FromSeconds(1))
            {
                mTimeLastReport = DateTime.Now;

                Table table = CreateMetricTable(testStatus);

                Layout["Metrics"].Update(table);

                // Requests / Second
                Panel panelRps = CreateRequestsPerSecondPanel(testStatus);
                Layout["RequestsPerSecond"].Update(panelRps);

                // Requests / Second
                var panelUsers = new Panel(new ColumnChart(4)
                    .WithFillColor(AccentBlue)
                    .SetValues(
                    testStatus.UserCount.History
                    ));
                panelUsers.Header = new PanelHeader($"Virtual Users ({testStatus.UserCount.Latest})");
                Layout["Users"].Update(panelUsers);

                // 200/404 etc
                Panel panel = CreateResponseCodePanel(testStatus);
                Layout["ResponseStatusCodes"].Update(panel);

                // Test Progress
                Panel panelProgress = CreateProgressPanel(testStatus);
                Layout["Progress"].Update(panelProgress);

                // Trigger repaint
                mRefreshEvent.Set();
            }
            return Task.CompletedTask;
        }

        internal Table CreateMetricTable(TestStatus testStatus)
        {
            var table = new Table();
            table.Expand();
            table.AddColumn("Metric");
            table.AddColumn("Latest");
            table.AddColumn("Min");
            table.AddColumn("Avg");
            table.AddColumn("Max");

            AddRow(table, testStatus.ProgressPercent);
            AddRow(table, testStatus.ElapsedSeconds);

            AddRow(table, testStatus.TestsPassed);
            AddRow(table, testStatus.TestsFailed);
            AddRow(table, testStatus.ChecksPassed);
            AddRow(table, testStatus.ChecksFailed);

            AddRow(table, testStatus.UserCount);
            AddRow(table, testStatus.UserMaxCount);

            AddRow(table, testStatus.Iterations);
            AddRow(table, testStatus.IterationDuration);

            AddRow(table, testStatus.HttpRequests);
            AddRow(table, testStatus.HttpRequestFails);
            AddRow(table, testStatus.HttpRequestsPerSecond);
            AddRow(table, testStatus.HttpRequestDuration);
            foreach (var pair in testStatus.HttpResponseStatusCodes)
            {
                AddRow(table, pair.Value);
            }

            return table;
        }

        internal Panel CreateResponseCodePanel(TestStatus testStatus)
        {
            BreakdownChart responseCodeChart = CreateResponseCodeChart(testStatus);
            var panel = new Panel(responseCodeChart);
            panel.Header = new PanelHeader("Response Status Codes");
            return panel;
        }
        internal Panel CreateTestPassratePanel(TestStatus testStatus)
        {
            var panelProgress = new Panel(
                new BreakdownChart()
                    .Width(60)
                    .FullSize()
                    .AddItem("Passed", Math.Round(testStatus.TestsPassed.Value, 1), Color.SpringGreen4)
                    .AddItem("Failed", Math.Round(testStatus.TestsFailed.Value, 1), Color.DarkRed));
            panelProgress.Header = new PanelHeader("Test Passrate");
            return panelProgress;
        }
        internal Panel CreateProgressPanel(TestStatus testStatus)
        {
            var panelProgress = new Panel(
                new BreakdownChart()
                    .Width(60)
                    .Expand()
                    .FullSize().ShowPercentage()
                    .AddItem("Completed", Math.Round(testStatus.ProgressPercent.Value, 1), AccentBlue)
                    .AddItem("Remaining", Math.Round(100 - testStatus.ProgressPercent.Value, 1), AccentPurple));
            panelProgress.Header = new PanelHeader("Test Progress");
            return panelProgress;
        }

        private Panel CreateRequestsPerSecondPanel(TestStatus testStatus)
        {
            var panelRps = new Panel(new ColumnChart(4)
                .WithFillColor(AccentBlue)
                .SetValues(
                testStatus.HttpRequestsPerSecond.History
                ));
            panelRps.Header = new PanelHeader("Requests / Second");
            return panelRps;
        }

        private BreakdownChart CreateResponseCodeChart(TestStatus testStatus)
        {
            var responseCodeChart = new BreakdownChart()
                .Expand()
                .Width(60)
                .FullSize();
            foreach (var pair in testStatus.HttpResponseStatusCodes)
            {
                var code = pair.Key.ToString();
                Color color = AccentBlue;
                if (pair.Key > System.Net.HttpStatusCode.InternalServerError)
                {
                    color = Color.DarkRed;
                }
                else if (pair.Key > System.Net.HttpStatusCode.BadRequest)
                {
                    color = AccentPurple;
                }
                responseCodeChart.AddItem(code, pair.Value.Value, color);
            }

            return responseCodeChart;
        }

        private void AddRow(Table table, Trend trend)
        {
            if (trend.Unit == "s")
            {
                table.AddRow(
                    new Text(trend.Name),
                    new Text(FormatSeconds(trend.Latest)),
                    new Markup("[green]" + FormatSeconds(trend.MinValue) + "[/]"),
                    new Markup("[cyan]" + FormatSeconds(trend.Average) + "[/]"),
                    new Markup("[red]" + FormatSeconds(trend.MaxValue) + "[/]")
                    );
            }
            else
            {
                table.AddRow(
                    new Text(trend.Name),
                    new Text(trend.Latest.ToString("#.##", CultureInfo.InvariantCulture)),
                    new Markup("[green]" + trend.MinValue.ToString("#.##", CultureInfo.InvariantCulture) + "[/]"),
                    new Markup("[cyan]" + trend.Average.ToString("#.##", CultureInfo.InvariantCulture) + "[/]"),
                    new Markup("[red]" + trend.MaxValue.ToString("#.##", CultureInfo.InvariantCulture) + "[/]")
                    );
            }
        }


        private static void AddRow(Table table, Gauge gauge)
        {
            table.AddRow(
                new Text(gauge.Name),
                new Text(gauge.Latest.ToString()),
                new Markup("[green]" + gauge.MinValue.ToString() + "[/]"),
                new Text(""),
                new Markup("[red]" + gauge.MaxValue.ToString() + "[/]")
                );
        }
        private void AddRow(Table table, Counter counter)
        {
            if (counter.Unit == "s")
            {
                table.AddRow(
                    counter.Name,
                    FormatSeconds(counter.Value),
                    "",
                    "",
                    ""
                    );
            }
            else
            {
                table.AddRow(
                    counter.Name,
                    counter.Value.ToString("#.##", CultureInfo.InvariantCulture),
                    "",
                    "",
                    ""
                    );
            }
        }


        private string FormatSeconds(double seconds)
        {
            if(seconds == double.MinValue || seconds == double.MaxValue)
            {
                return "";
            }
            var timespan = TimeSpan.FromSeconds(seconds);
            var sb = new StringBuilder();

            if (timespan.TotalSeconds >= 1)
            {
                if (timespan.Hours > 0)
                {
                    sb.Append($"{timespan.Hours}h");
                }
                if (timespan.Minutes > 0)
                {
                    sb.Append($"{timespan.Minutes}m");
                }
                if (timespan.Seconds > 0)
                {
                    sb.Append($"{timespan.Seconds}s");
                }
            }
            else
            {
                sb.Append($"{(int)timespan.TotalMilliseconds}ms");
            }

            return sb.ToString();
        }

        public void Stop()
        {
            if (mRenderThread != null)
            {
                mCancellationTokenSource.Cancel();
                mRenderThread.Join();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
