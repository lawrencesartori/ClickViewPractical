using ClickViewPracticalLibrary.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ClickViewPracticalLibrary.Service
{
    public class PlaylistStore : IPlaylistStore
    {
        private readonly string _dbJsonConnectionString;
        private readonly ILogger<PlaylistStore> _log;

        public PlaylistStore(IConfiguration config, ILogger<PlaylistStore> logger)
        {
            _dbJsonConnectionString = config[Constants.AppSettingKeys.DBJsonPath];
            _log = logger;
        }

        public List<Playlist> GetPlaylists(PlaylistFilter filter)
        {
            return GetPlaylistsAsQueryable(filter).ToList();
        }

        public List<Video> GetVideos(VideoPlaylistFilter filter)
        {
            return GetVideosAsQueryable(filter).ToList();
        }

        private IQueryable<Video> GetVideosAsQueryable(VideoPlaylistFilter filter)
        {
            var vids = TryGetDBFromFile().Videos.AsQueryable();
            if (filter.Id != null && filter.Id > 0)
            {
                vids = vids.Where(o => o.Id == filter.Id);
            }
            if (filter.Ids != null && filter.Ids.Any())
            {
                vids = vids.Where(o => filter.Ids.Contains(o.Id));
            }
            return vids;
        }

        private IQueryable<Playlist> GetPlaylistsAsQueryable(PlaylistFilter filter)
        {
            var playLists = TryGetDBFromFile().Playlists.AsQueryable();
            if (filter.Id != null && filter.Id > 0)
            {
                playLists = playLists.Where(o => o.ID == filter.Id);
            }
            
            if (filter.Ids != null && filter.Ids.Any())
            {
                playLists = playLists.Where(o => filter.Ids.Contains(o.ID));
            }
            
            if(filter.VideoId != null && filter.VideoId > 0)
            {
                playLists = playLists.Where(o => o.VideoIds.Contains(filter.VideoId.Value));
            }

            return playLists;
        }

        public Playlist? GetPlaylistIfExists(int id)
        {
            return GetPlaylistsAsQueryable(new PlaylistFilter { Id = id }).FirstOrDefault();
        }

        public Video? GetVideoIfExists(int id)
        {
            return GetVideosAsQueryable(new VideoPlaylistFilter { Id = id }).FirstOrDefault();
        }

        public async Task<HttpStatusCode> AddPlaylistAsync(Playlist playlist)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null) 
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                playlist.ID = db.Playlists.Max(o => o.ID) + 1;
                db.Playlists.Add(playlist);
                await SaveDBAsync(fs, db);
                return HttpStatusCode.OK;
            }
            fs?.Dispose();
            return HttpStatusCode.InternalServerError;
        }

        public async Task<HttpStatusCode> AddVideoToPlaylistAsync(int videoId, int playlistId)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null)
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                var relPlaylist = db.Playlists.FirstOrDefault(o => o.ID == playlistId);
                if(relPlaylist != null)
                {
                    relPlaylist.VideoIds.Add(videoId);
                    await SaveDBAsync(fs, db);
                    return HttpStatusCode.OK;
                }
            }
            fs?.Dispose();
            return HttpStatusCode.InternalServerError;
        }

        public async Task<HttpStatusCode> UpdatePlaylistAsync(Playlist playlist)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null)
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                var existingPlaylist = db.Playlists.FirstOrDefault(o => o.ID == playlist.ID);
                if (existingPlaylist != null)
                {
                    existingPlaylist.Name = playlist.Name;
                    existingPlaylist.Description = playlist.Description;
                    existingPlaylist.VideoIds = playlist.VideoIds;
                    await SaveDBAsync(fs, db);
                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.NotFound;
            }
            fs?.Dispose();
            return HttpStatusCode.InternalServerError;
        }

        public async Task<HttpStatusCode> RemovePlaylistAsync(int playlistId)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null)
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                var relPlaylist = db.Playlists.FirstOrDefault(o => o.ID == playlistId);
                if(relPlaylist != null)
                {
                    db.Playlists.Remove(relPlaylist);
                    await SaveDBAsync(fs, db);
                    return HttpStatusCode.OK;
                }
            }
            fs?.Dispose();
            return HttpStatusCode.InternalServerError;
        }

        public async Task<HttpStatusCode> RemoveVideoFromPlaylistAsync(int videoId, int playlistId)
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs != null)
            {
                var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
                var relPlaylist = db.Playlists.FirstOrDefault(o => o.ID == playlistId);
                if (relPlaylist != null)
                {
                    var relVideoId = relPlaylist.VideoIds.FirstOrDefault(o => o == videoId);
                    if(relVideoId > 0)
                    {
                        relPlaylist.VideoIds.Remove(relVideoId);
                        await SaveDBAsync(fs, db);
                        return HttpStatusCode.OK;
                    }
                }
            }
            fs?.Dispose();
            return HttpStatusCode.InternalServerError;
        }

        private async Task SaveDBAsync(FileStream fs, PlaylistVideoDB db)
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(db));
            fs.Position = 0;
            fs.SetLength(0);
            await fs.WriteAsync(data, 0, data.Length);
            fs.Dispose();
        }

        private PlaylistVideoDB TryGetDBFromFile()
        {
            var fs = GetVideoFileStreamFromFile();
            if (fs == null)
            {
                _log.LogError("Json DB File is in use");
                return null;
            }
            
            var db = JsonSerializer.Deserialize<PlaylistVideoDB>(fs);
            fs.Dispose();
            return db;
        }

        private FileStream GetVideoFileStreamFromFile()
        {
            return WaitForFile(_dbJsonConnectionString, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }

        //https://stackoverflow.com/questions/50744/wait-until-file-is-unlocked-in-net/50800#50800
        private FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
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
