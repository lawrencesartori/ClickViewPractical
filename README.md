# ClickViewPractical

This is a .net 6 web api project based off requirements specied by ClickView Practical task.

It took roughly 3.5 hours to do all 5 Tasks.

To Start just setup relevant paths in appsettings.json for the Video/Playlist/DBJson files

Current Known Bugs:

For some reason the Datetime value from the json file always deserialises as Datetime.Min value, not sure why..haven't used much of System.Text.Json, mainly used Newtonsoft.Json
