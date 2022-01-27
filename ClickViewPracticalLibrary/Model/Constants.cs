using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickViewPracticalLibrary.Model
{
    public class Constants
    {
        public class AppSettingKeys
        {
            public const string PlaylistsPath = "PlaylistPath";
            public const string VideosPath = "VideosPath";
            public const string DBJsonPath = "DBJsonPath";
        }

        public class DefaultValues
        {
            public const int MaxFileTries = 10;
        }
    }
}
