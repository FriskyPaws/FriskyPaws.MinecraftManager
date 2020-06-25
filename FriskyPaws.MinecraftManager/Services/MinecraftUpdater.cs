namespace FriskyPaws.MinecraftManager.Services
{
    using System.Threading.Tasks;
    using FriskyPaws.MinecraftManager.Configurations;
    using Flurl.Http;
    using FriskyPaws.MinecraftManager.Services.Updater.Models;
    using Flurl;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.IO;
    using System.Text.Json;
    using System.Linq;
    using CoreRCON;
    using System.Net;
    using System.Diagnostics;
    using System;

    public class MinecraftUpdater
    {
        public MinecraftUpdater(
            ILogger<MinecraftUpdater> logger,
            IOptions<MinecraftConfiguration> configuration
        )
        {
            Configuration = configuration.Value;
            Logger = logger;
        }

        private MinecraftConfiguration Configuration { get; }

        private ILogger Logger { get; }

        private static Url ManifestUrl { get; } = new Url("https://launchermeta.mojang.com/mc/game/version_manifest.json");

        private string GetManifestLocation => $"{Configuration.ManifestLocation}/minecraftmanager.json";

        public async Task CheckForUpdates()
        {
            var manifest = await ManifestUrl.GetJsonAsync<VersionManifest>();
            Logger.LogDebug("Update manifest: {0}", manifest);

            var currentVersion = await GetCurrentManifest();
            Logger.LogInformation("Current Version: {0}, latest version: {1}", currentVersion?.Id, manifest.Latest.Release);
            if (currentVersion == null || currentVersion.Id != manifest.Latest.Release)
            {
                await Update(manifest);
            }
        }

        private async Task<MinecraftVersion> GetCurrentManifest()
        {
            if (!File.Exists(GetManifestLocation))
            {
                return null;
            }

            return JsonSerializer.Deserialize<MinecraftVersion>(File.ReadAllText(GetManifestLocation));
        }

        private async Task Update(VersionManifest versionManifest)
        {
            var latest = versionManifest.Versions.FirstOrDefault(v => v.Id == versionManifest.Latest.Release);
            await Announce(latest.Id);
            Shutdown();
            await new Url(latest.Url).DownloadFileAsync(
                Path.GetDirectoryName(Configuration.MinecraftJar),
                Path.GetFileName(Configuration.MinecraftJar));
            WriteManifest(latest);
            Startup();

        }

        private async Task Announce(string version)
        {
            var duration = 5;
            var rcon = new RCON(IPAddress.Parse(Configuration.RconIp), Configuration.RconPort, Configuration.RconPassword);
            try
            {
                await rcon.ConnectAsync();
                await rcon.SendCommandAsync($"say Minecraft v{version} is available, server will go down for updates in {duration} minutes");
                await Task.Delay((duration - 1) * 1000 * 60);
                await rcon.SendCommandAsync($"say Minecraft v{version} is available, server will go down for updates in 1 minute");
                await Task.Delay(1 * 1000 * 60);
            }
            catch (Exception exception)
            {
                Logger.LogWarning("Could not connect to rcon to warn of server shutdown: {0}", exception.Message);
            }
        }

        private void Startup()
        {
            Logger.LogDebug("Running command {0}", Configuration.StartCommand);
            Execute(Configuration.StartCommand);
        }

        private void Shutdown()
        {
            Logger.LogDebug("Running command {0}", Configuration.ShutdownCommand);
            Execute(Configuration.ShutdownCommand);
        }

        private void Execute(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Logger.LogError("Unexpected exit code {0}, {1}", process.ExitCode, process.StandardError.ReadToEnd());
            }
        }

        private void WriteManifest(MinecraftVersion version)
        {
            File.WriteAllText(GetManifestLocation, JsonSerializer.Serialize(version));
        }
    }
}