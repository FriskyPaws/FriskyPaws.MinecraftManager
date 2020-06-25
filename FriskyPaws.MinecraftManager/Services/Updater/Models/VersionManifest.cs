using System;
using System.Text.Json;

namespace FriskyPaws.MinecraftManager.Services.Updater.Models
{
    public class VersionManifest
    {
        public Latest Latest { get; set; }

        public MinecraftVersion[] Versions { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class Latest
    {
        public string Release { get; set; }

        public string Snapshot { get; set; }
    }

    public class MinecraftVersion
    {
        public string Id { get; set; }

        public DateTime ReleaseTime { get; set; }

        public DateTime Time { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }
    }
}