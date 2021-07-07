namespace MyLab.ConfigServer.Shared
{
    public class ConfigInfo
    {
        public string Content { get; set; }
        public ConfigSecret[] Secrets { get; set; }
    }
}