using ClickViewPracticalLibrary.Model;
using System.Text.Json;

namespace ClickViewPracticalLibrary.Service
{
    public class PlaylistLoader
    {
        private readonly ILoaderConfigManager _config;

        public PlaylistVideoDB PlaylistVideoDB {get;set;}

        public PlaylistLoader(ILoaderConfigManager config)
        {
            _config = config;

            PlaylistVideoDB = new PlaylistVideoDB();
        }

        public async Task LoadDataIfNeedToDbJsonFile()
        {
            if (!File.Exists(_config.DBJsonPath))
            {
                if (!File.Exists(_config.PlaylistPath))
                {
                    throw new FileNotFoundException($"Playlist File not found {_config.PlaylistPath}");
                }

                if (!File.Exists(_config.VideoPath))
                {
                    throw new FileNotFoundException($"Video File not found {_config.VideoPath}");
                }

                var jsonString = await File.ReadAllTextAsync(_config.PlaylistPath);
                PlaylistVideoDB.Playlists = JsonSerializer.Deserialize<List<Playlist>>(jsonString) ?? throw new Exception($"Playlists failed to deserialise. Please investigate json file at {_config.PlaylistPath}");
                jsonString = await File.ReadAllTextAsync(_config.VideoPath);
                PlaylistVideoDB.Videos = JsonSerializer.Deserialize<List<Video>>(jsonString) ?? throw new Exception($"Videos failed to deserialise. Please investigate json file at {_config.VideoPath}");

                await File.WriteAllTextAsync(_config.DBJsonPath, JsonSerializer.Serialize(PlaylistVideoDB));
            }
        }
    }
}
