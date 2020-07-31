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
}
