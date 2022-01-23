using ClickViewPracticalLibrary.Model;
using Microsoft.Extensions.Configuration;

namespace ClickViewPracticalLibrary.Service
{
    public class LoaderConfigManager : ILoaderConfigManager
    {
        private readonly IConfiguration _configuration;
        public LoaderConfigManager(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string PlaylistPath => _configuration[Constants.AppSettingKeys.PlaylistsPath];
        public string VideoPath => _configuration[Constants.AppSettingKeys.VideosPath];

        public string DBJsonPath => _configuration[Constants.AppSettingKeys.DBJsonPath];
    }
}
