/*
ImageGlass Project - Image viewer for Windows
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

using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ImageGlass.Settings {
    public class Source {

        #region Public properties

        /// <summary>
        /// Gets the user config file name.
        /// </summary>
        public string UserFilename { get => "igconfig.json"; }


        /// <summary>
        /// Gets the default config file located.
        /// </summary>
        public string DefaultFilename { get => "igconfig.default.json"; }


        /// <summary>
        /// Gets the admin config file name.
        /// </summary>
        public string AdminFilename { get => "igconfig.admin.json"; }


        /// <summary>
        /// Config file description
        /// </summary>
        public string Description { get; set; } = "ImageGlass configuration file";


        /// <summary>
        /// Config file version
        /// </summary>
        public string Version { get; set; } = "0.1";


        /// <summary>
        /// Gets, sets value indicates that the config file is compatible with this ImageGlass version or not
        /// </summary>
        public bool IsCompatible { get; set; } = true;


        #endregion


        #region Public methods


        /// <summary>
        /// Loads all config files: default, user, command-lines, admin;
        /// then unify configs.
        /// </summary>
        public IConfigurationRoot LoadUserConfigs() {
            const string CMD_PREFIX = "--";
            var args = Environment.GetCommandLineArgs()
                // find the commands with prefix
                .Where(cmd => cmd.StartsWith(CMD_PREFIX))
                // remove the prefix
                .Select(cmd => cmd.Substring(2))
                .ToArray();

            var userConfig = new ConfigurationBuilder()
                .SetBasePath(Windows.Storage.ApplicationData.Current.LocalFolder.Path)
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
