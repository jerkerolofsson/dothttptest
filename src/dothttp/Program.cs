using dothttp.Commands;

namespace dothttp;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddCommand<RunCommand>("run");
        });

        await app.RunAsync(args);
    }
}