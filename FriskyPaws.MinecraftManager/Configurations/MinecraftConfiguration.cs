namespace FriskyPaws.MinecraftManager.Configurations
{
    public class MinecraftConfiguration
    {
        public string MinecraftJar { get; set; }

        public string ManifestLocation { get; set; }

        public ushort RconPort { get; set; }

        public string RconIp { get; set; }

        public string RconPassword { get; set; }

        public string ShutdownCommand { get; set; }

        public string StartCommand { get; set; }
    }
}