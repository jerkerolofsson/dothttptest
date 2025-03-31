using DotHttpTest;
using DotHttpTest.Reporting.JUnitXml;
using DotHttpTest.Runner;
using dothttp.Handlers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Spectre.Console;

namespace dothttp.Commands
{
    internal class RunCommand : AsyncCommand<RunCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[Path]")]
            public string? Path { get; set; }

            [Description("Sets name of the test plan")]
            [CommandOption("-n|--name")]
            public string? PlanName { get; set; }

            [Description("Specify path to the report file (default junit xml format)")]
            [CommandOption("-r|--report")]
            public string? ReportPath { get; set; }

            [Description("Runs without writing progress to stdout")]
            [DefaultValue(false)]
            [CommandOption("-q|--no-progress")]
            public bool Quiet { get; set; } = false;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            if (!settings.Quiet)
            {
                AnsiConsole.MarkupLine($"[#52b788]Loading[/]: {settings.Path}..");
            }

            //var resultPath = Path.GetDirectoryName(settings.Path) ?? ".";
            //var junitXmlPath = Path.Combine(resultPath, Path.GetFileNameWithoutExtension(settings.Path) + ".result.xml");

            ConsoleDotHttpRunnerProgressHandler? ui = null;
            var builder = new TestPlanRunnerOptionsBuilder()
                .LoadHttpFile(settings.Path!)
                .ConfigureTestPlan((plan, options) =>
                {
                    if (settings.PlanName is not null)
                    {
                        plan.SetName(settings.PlanName);
                    }
                });

            if (!settings.Quiet)
            {
                ui = new ConsoleDotHttpRunnerProgressHandler();
                builder.AddCallback(ui);
            }

            if (settings.ReportPath is not null)
            {
                builder.AddCallback(new JUnitXmlWriter(settings.ReportPath));
            }

            var runner = builder.Build();

            var testStatus = await runner.RunAsync();
            if (ui is not null)
            {
                ui?.Stop();
            }


            if (ui is not null)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(ui.CreateMetricTable(testStatus));
                AnsiConsole.Write(ui.CreateResponseCodePanel(testStatus));
                AnsiConsole.Write(ui.CreateTestPassratePanel(testStatus));
                AnsiConsole.WriteLine();
            }

            var report = testStatus.TestReport;
            int stagePassCount = 0;
            int stageFailCount = 0;
            foreach (var stage in report.Stages)
            {
                if(stage.FailCount > 0)
                {
                    stageFailCount++;
                    AnsiConsole.MarkupLine($"[red]X[/] {stage.PlannedStage.Attributes.Name}");
                }
                else
                {
                    stagePassCount++;
                    AnsiConsole.MarkupLine($"[green]✓[/] {stage.PlannedStage.Attributes.Name}");

                }
            }

            // Summary
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Passed: {stagePassCount}, Failed: {stageFailCount}");
            AnsiConsole.WriteLine();


            if (testStatus.FailedChecks.Count > 0)
            {
                AnsiConsole.MarkupLine($"{testStatus.FailedChecks.Count} failed checks:");
                foreach (var failedCheck in testStatus.FailedChecks)
                {
                    AnsiConsole.Markup("[red]X[/] ");
                    AnsiConsole.WriteLine(failedCheck.Error ?? "unknown error");
                }
            }
            if (testStatus.HttpRequestFails.Value > 0)
            {
                return -1;
            }

            return 0;
        }

        public override ValidationResult Validate(CommandContext context, Settings settings)
        {
            if (!File.Exists(settings.Path))
            {
                return ValidationResult.Error($"Path not found - {settings.Path}");
            }

            return base.Validate(context, settings);
        }
    }
}
