using ClickViewPracticalLibrary.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using ClickViewPracticalLibrary.Model.Filters;

namespace ClickViewPracticalLibrary.Service
{
    public class PlaylistStore : IPlaylistStore
    {
        private readonly string _dbJsonConnectionString;
        private readonly ILogger<PlaylistStore> _log;
        private readonly MethodResultHelper _resultHelper;

        public PlaylistStore(IConfiguration config, ILogger<PlaylistStore> logger)
        {
            _dbJsonConnectionString = config[Constants.AppSettingKeys.DBJsonPath];
            _log = logger;
            _resultHelper = new MethodResultHelper(_log);
        }

        public async Task<List<Video>> GetVideos(VideoPlaylistFilter filter)
        {
            var vids = (await TryGetDBFromFile()).Videos;
            if (filter.Id != null && filter.Id > 0)
            {
                vids = vids.Where(o => o.Id == filter.Id).ToList();
            }
            if (filter.Ids.IsNotNullOrEmpty())
            {
                vids = vids.Where(o => filter.Ids.Contains(o.Id)).ToList();
            }
            return vids;
        }

        public async Task<List<Playlist>> GetPlaylists(PlaylistFilter filter)
        {
            var playLists = (await TryGetDBFromFile()).Playlists;
            if (filter.Id != null && filter.Id > 0)
            {
                playLists = playLists.Where(o => o.Id == filter.Id).ToList();
            }
            
            if (filter.Ids.IsNotNullOrEmpty())
            {
                playLists = playLists.Where(o => filter.Ids.Contains(o.Id)).ToList();
            }
            
            if(filter.VideoId != null && filter.VideoId > 0)
            {
                playLists = playLists.Where(o => o.VideoIds.IsNotNullOrEmpty() && o.VideoIds.Contains(filter.VideoId.Value)).ToList();
            }

            return playLists;
        }

        public async Task<List<SimplePlaylist>> GetSimplePlaylists(PlaylistFilter filter)
        {
            return (await GetPlaylists(filter)).Select(o => new SimplePlaylist(o)).ToList();
        }

        public async Task<Playlist?> GetPlaylistIfExists(int id)
        {
            return (await GetPlaylists(new PlaylistFilter { Id = id })).FirstOrDefault();
        }

        public async Task<SimplePlaylist?> GetSimplePlaylistIfExists(int id)
        {
            return (await GetSimplePlaylists(new PlaylistFilter { Id = id })).FirstOrDefault();
        }

        public async Task<Video?> GetVideoIfExists(int id)
        {
            return (await GetVideos(new VideoPlaylistFilter { Id = id })).FirstOrDefault();
        }

        public async Task<SimplePlaylist?> AddPlaylistAsync(Playlist playlist)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null) 
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                playlist.Id = db.Playlists.Max(o => o.Id) + 1;
                db.Playlists.Add(playlist);
                await SaveDBAsync(fs, db);
                await fs.DisposeAsync();
                return new SimplePlaylist(playlist);
            }
            return null;
        }

        public async Task<List<int>?> AddVideoToPlaylistAsync(int videoId, int playlistId)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null)
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                var relPlaylist = db.Playlists.FirstOrDefault(o=>o.Id == playlistId);
                if (relPlaylist != null)
                {
                    relPlaylist.VideoIds.Add(videoId);
                    await SaveDBAsync(fs, db);
                    return relPlaylist.VideoIds;
                }
                await fs.DisposeAsync();
            }
            return null;
        }

        public async Task<SimplePlaylist?> UpdatePlaylistAsync(SimplePlaylist playlist)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null)
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                var existingPlaylist = db.Playlists.FirstOrDefault(o => o.Id == playlist.Id);
                if (existingPlaylist != null)
                {
                    existingPlaylist.Name = playlist.Name;
                    existingPlaylist.Description = playlist.Description;
                    await SaveDBAsync(fs, db);

                    return playlist;
                }
                await fs.DisposeAsync();
            }
            return null;
        }

        public async Task<bool> RemovePlaylistAsync(int playlistId)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null)
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                var relPlaylist = db.Playlists.FirstOrDefault(o => o.Id == playlistId);
                if (relPlaylist != null)
                {
                    db.Playlists.Remove(relPlaylist);
                    await SaveDBAsync(fs, db);
                    return true;
                }
                await fs.DisposeAsync();
            }
            return false;
        }

        public async Task<List<int>?> RemoveVideoFromPlaylistAsync(int videoId, int playlistId)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null)
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                var relPlaylist = db.Playlists.FirstOrDefault(o => o.Id == playlistId);
                if (relPlaylist != null && relPlaylist.VideoIds.IsNotNullOrEmpty())
                {
                    var relVideoId = relPlaylist.VideoIds.FirstOrDefault(o => o == videoId);
                    if(relVideoId > 0)
                    {
                        relPlaylist.VideoIds.Remove(relVideoId);
                        await SaveDBAsync(fs, db);
                        return relPlaylist.VideoIds;
                    }
                }
                await fs.DisposeAsync();
            }
            return null;
        }

        private async Task SaveDBAsync(FileStream fs, PlaylistVideoDB db)
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(db));
            fs.Position = 0;
            fs.SetLength(0);
            await fs.WriteAsync(data, 0, data.Length);
            await fs.DisposeAsync();
        }

        private async Task<PlaylistVideoDB> TryGetDBFromFile()
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs == null)
            {
                _log.LogError("Json DB File is in use");
                return null;
            }
            
            var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
            await fs.DisposeAsync();
            return db;
        }

        private FileStream? GetVideoFileStreamFromFile()
        {
            return WaitForFile(_dbJsonConnectionString, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }

        //https://stackoverflow.com/questions/50744/wait-until-file-is-unlocked-in-net/50800#50800
        private FileStream? WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
        {
            for (int numTries = 0; numTries < Constants.DefaultValues.MaxFileTries; numTries++)
            {
                FileStream? fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access, share);
                    return fs;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    Thread.Sleep(50);
                }
            }

            return null;
        }

    }
}
