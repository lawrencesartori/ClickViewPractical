# ClickViewPractical

This is a .net 6 web api project based off requirements specified by ClickView Practical task.

It took roughly 3.5 hours to do all 5 Tasks.

To start just setup relevant paths in appsettings.json for the Video/Playlist/DBJson files

Current Known Bugs:

For some reason the Datetime value from the json file always deserialises as Datetime.Min value, not sure why..haven't used much of System.Text.Json, mainly used Newtonsoft.Json


Currently 8 Endpoints


Get -> playlist/GetAllPlaylists -> Will return all Simple Playlists (without video ids)

Get -> playlist/GetAllVideosInPlaylist/{playlistId} -> Will return a Simple Playlist if playlist exists otherwise will return 404

Get -> playlist/GetAllPlaylistsForVideo/{videoId} -> Will return a list of Simple Playlists if video exists otherwise will return 404

Post -> playlist/CreatePlaylist -> Expects a SimplePlaylist object {name:String, description:string} -> will return a Simple Playlist object if successful otherwise will return a 400

Put -> playlist/UpdatePlaylist -> Expects a SimplePlaylist object {name:String, description:string, id:int} -> will return a Simple Playlist object if successful otherwise will return a 400 if missing fields or a 404 if playlist isn't found

Delete -> playlist/DeletePlaylist/{playlistId} -> Will return a 204 if successful or a 404 if playlist isn't found

Put -> playlist/AddVideoToPlaylist/{playlistId}/{videoId} -> Will return a list of video ids if successful otherwise will return a 400 if invalid video/playlist or a 404 if video/playlist isn't found

Delete -> playlist/RemoveVideoFromPlaylist/{playlistId}/{videoId} -> Will return a list of video ids if successful otherwise will return a 400 if invalid video/playlist or a 404 if video/playlist isn't found


