namespace MyLab.ConfigServer.Shared
{
    public class ConfigSecret
    {
        public string FieldPath { get; set; }
        public string SecretKey { get; set; }
        public bool Resolved { get; set; }
    }
}