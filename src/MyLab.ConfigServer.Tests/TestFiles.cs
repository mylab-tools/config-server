using System;
using System.IO;

namespace MyLab.ConfigServer.Tests
{

    static class TestFiles
    {
        private static readonly Lazy<string> _origin;
        private static readonly Lazy<string> _override;
        private static readonly Lazy<string> _include1;
        private static readonly Lazy<string> _include2;

        public static string Origin => _origin.Value;
        public static string Override => _override.Value;
        public static string Include1 => _include1.Value;
        public static string Include2 => _include2.Value;

        static TestFiles()
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            _origin = new Lazy<string>( () => File.ReadAllText(Path.Combine(basePath, "foo.json")));
            _override = new Lazy<string>( () => File.ReadAllText(Path.Combine(basePath, "foo-override.json")));
            _include1 = new Lazy<string>( () => File.ReadAllText(Path.Combine(basePath, "include-1.json")));
            _include2 = new Lazy<string>( () => File.ReadAllText(Path.Combine(basePath, "include-2.json")));
        }
    }
}
