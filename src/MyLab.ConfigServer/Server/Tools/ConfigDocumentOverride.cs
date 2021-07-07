namespace MyLab.ConfigServer.Server.Tools
{
    class ConfigDocumentOverride
    {
        public string Path { get; }
        public string Value { get; }

        public ConfigDocumentOverride(string path, string value)
        {
            Path = path;
            Value = value;
        }
    }
}