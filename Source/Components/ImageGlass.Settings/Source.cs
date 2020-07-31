/*
ImageGlass Preview project - Image viewer for Windows
Copyright (C) 2020 DUONG DIEU PHAP
Project homepage: https://imageglass.org

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using ImageGlass.Base;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ImageGlass.Settings {
    public class Source {

        #region Public properties

        /// <summary>
        /// Gets the user config file name.
        /// </summary>
        public string UserFilename { get => "igp.config.json"; }


        /// <summary>
        /// Gets the default config file located.
        /// </summary>
        public string DefaultFilename { get => "igp.config.default.json"; }


        /// <summary>
        /// Gets the admin config file name.
        /// </summary>
        public string AdminFilename { get => "igp.config.admin.json"; }


        /// <summary>
        /// Config file description
        /// </summary>
        public string Description { get; set; } = "ImageGlass Preview configuration file";


        /// <summary>
        /// Config file version
        /// </summary>
        public string Version { get; set; } = "0.1";


        /// <summary>
        /// Gets, sets value indicates that the config file is compatible with this ImageGlass Preview version or not
        /// </summary>
        public bool IsCompatible { get; set; } = true;


        #endregion


        #region Public methods


        /// <summary>
        /// Loads all config files: default, user, command-lines, admin;
        /// then unify configs.
        /// </summary>
        public async Task<IConfigurationRoot> LoadUserConfigs() {
            var args = BaseApp.GetCommandLineArgs(CommandLineArgFilterOptions.OnlySettings);
            var localDir = await BaseApp.GetLocalDirAsync();

            var userConfig = new ConfigurationBuilder()
                .SetBasePath(localDir.Path)
                .AddJsonFile(DefaultFilename, optional: true)
                .AddJsonFile(UserFilename, optional: true)
                .AddCommandLine(args)
                .AddJsonFile(AdminFilename, optional: true)
                .Build();

            return userConfig;
        }


        #endregion

    }
}
