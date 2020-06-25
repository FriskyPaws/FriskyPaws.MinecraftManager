namespace FriskyPaws.MinecraftManager.Console
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using CommandLine;
    using FriskyPaws.MinecraftManager.Configurations;
    using FriskyPaws.MinecraftManager.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    class Program
    {
        static Task Main(string[] args) =>
            Parser.Default
                .ParseArguments<ProgramArguments>(args)
                .WithParsedAsync<ProgramArguments>(async o => await Run(o));

        static IConfiguration Configuration { get; set; }

        static IServiceProvider ServiceProvider { get; set; }

        static async Task Run(ProgramArguments arguments)
        {
            Configuration = BuildConfiguration();
            ServiceProvider = Services();
            var updater = ServiceProvider.GetRequiredService<MinecraftUpdater>();
            await updater.CheckForUpdates();
        }

        static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile(Path.GetFullPath(@"appsettings.json"), true)
                .AddJsonFile(Path.GetFullPath(@"appsettings.local.json"), false)
                .Build();
        }

        static IServiceProvider Services()
        {
            return new ServiceCollection()
                .AddLogging(opt =>
                    opt
                        .AddConfiguration(Configuration.GetSection("Logging"))
                        .AddConsole()
                )
                .AddOptions()
                .Configure<MinecraftConfiguration>(o => Configuration.GetSection("Minecraft").Bind(o))
                .AddSingleton<MinecraftUpdater>()
                .BuildServiceProvider();
        }

    }
}
