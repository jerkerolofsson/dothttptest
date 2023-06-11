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

namespace dothttp.Commands
{
    internal class RunCommand : AsyncCommand<RunCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[Path]")]
            public string? Path { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            AnsiConsole.MarkupLine($"[#52b788]Loading[/]: {settings.Path}..");

            var resultPath = Path.GetDirectoryName(settings.Path) ?? ".";
            var junitXmlPath = Path.Combine(resultPath, Path.GetFileNameWithoutExtension(settings.Path) + ".result.xml");

            using var ui = new ConsoleDotHttpRunnerProgressHandler();
            var runner = new TestPlanRunnerOptionsBuilder()
                .AddCallback(ui)
                .AddCallback(new JUnitXmlWriter(junitXmlPath))
                .LoadHttpFile(settings.Path!)
                .Build();

            var testStatus = await runner.RunAsync();
            ui.Stop();

            AnsiConsole.Clear();
            AnsiConsole.Write(ui.CreateMetricTable(testStatus));
            AnsiConsole.Write(ui.CreateResponseCodePanel(testStatus));
            AnsiConsole.Write(ui.CreateTestPassratePanel(testStatus));
            AnsiConsole.WriteLine();

            AnsiConsole.WriteLine($"{testStatus.FailedChecks.Count} failed checks:");
            foreach (var failedCheck in testStatus.FailedChecks)
            {
                //AnsiConsole.WriteLine($"{failedCheck.Check.VerifierId} {failedCheck.Check.PropertyId} was {failedCheck.ExpectedValue}, expected {failedCheck.Check.ExpectedValue ?? ""} for {failedCheck.Request.RequestName} ({failedCheck.Request.Url})");
                AnsiConsole.WriteLine(failedCheck.Error??"unknown error");
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
