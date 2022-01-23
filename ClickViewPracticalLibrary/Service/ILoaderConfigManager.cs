namespace ClickViewPracticalLibrary.Service
{
    public interface ILoaderConfigManager
    {
        public string PlaylistPath { get; }
        public string VideoPath { get; }

        public string DBJsonPath { get; }
    }
    
}
