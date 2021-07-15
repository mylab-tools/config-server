namespace MyLab.ConfigServer.Server.Services
{
    public interface IResourcePathProvider
    {
        string Provide();
    }

    class ResourcePathProvider : IResourcePathProvider
    {
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of <see cref="ResourcePathProvider"/>
        /// </summary>
        public ResourcePathProvider(string path)
        {
            _path = path;
        }

        public string Provide()
        {
            return _path;
        }

    }
}
