using System.Threading.Tasks;
using MyLab.ApiClient;
using MyLab.ConfigServer.Shared;

namespace MyLab.ConfigServer.Client
{
    [Api("api/v2", Key = "api")]
    public interface IConfigServiceV2
    {
        [Get("clients")]
        Task<ClientsStorageViewModel> GetClientList();

        [Get("configs")]
        Task<ConfigStorageViewModel> GetConfigList();

        [Get("configs/{id}")]
        Task<ConfigViewModel> GetConfigDetails([Path]string id);

        [Get("includes")]
        Task<ConfigStorageViewModel> GetIncludeList();

        [Get("includes/{id}")]
        Task<ConfigViewModel> GetIncludeDetails([Path] string id);

        [Get("overrides")]
        Task<ConfigStorageViewModel> GetOverrideList();

        [Get("overrides/{id}")]
        Task<ConfigViewModel> GetOverrideDetails([Path] string id);

        [Get("base-configs")]
        Task<ConfigStorageViewModel> GetBaseConfigList();

        [Get("base-configs/{id}")]
        Task<ConfigViewModel> GetBaseConfigDetails([Path] string id);
    }
}