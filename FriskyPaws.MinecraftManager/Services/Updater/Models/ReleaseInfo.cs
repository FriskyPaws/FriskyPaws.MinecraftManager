namespace FriskyPaws.MinecraftManager.Services.Updater.Models
{
    public class ReleaseInfo
    {
        public Downloads Downloads { get; set; }
    }

    public class Downloads
    {
        public DownloadInfo Server { get; set; }
    }

    public class DownloadInfo
    {
        public string Sha1 { get; set; }

        public long Size { get; set; }

        public string Url { get; set; }
    }
}