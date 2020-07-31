using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGlass.Base {
    /// <summary>
    /// Filter options for command line arguments
    /// </summary>
    [Flags]
    public enum CommandLineArgFilterOptions {
        None = 0,
        OnlySettings = 1,
        ExludeSettings = 2,
    }

    /// <summary>
    /// Application folders. The name is also folder name, except for _Root and _Temp
    /// </summary>
    public enum Dirs {
        _Root = -1,
        _Temp = 0,

        // The below names are also the folder names
        Themes = 1,
        Assets = 2,
        Lang = 3,
    }
}
