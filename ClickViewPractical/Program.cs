using ClickViewPracticalLibrary.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<ILoaderConfigManager, LoaderConfigManager>();
builder.Services.AddSingleton<PlaylistLoader>();
builder.Services.AddScoped<IPlaylistStore, PlaylistStore>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();

var app = builder.Build();

var playlistLoader = app.Services.GetService<PlaylistLoader>();
if (playlistLoader != null)
{
    await playlistLoader.LoadDataIfNeedToDbJsonFile();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
