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
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using Windows.Storage;

namespace ImageGlass.Settings {
    public class Config {

        #region Internal properties
        private static readonly Source _source = new Source();


        #endregion



        #region ImageGlass settings

        /// <summary>
        /// Gets, sets SampleSetting
        /// </summary>
        public static int SampleSetting { get; set; } = 200;


        #endregion




        #region Public functions

        /// <summary>
        /// Loads and parsse configs from file
        /// </summary>
        public static async void Load() {
            var items = await _source.LoadUserConfigs();

            // Number values
            SampleSetting = items.GetValue(nameof(SampleSetting), SampleSetting);

        }


        /// <summary>
        /// Parses and writes configs to file
        /// </summary>
        public static async void Write() {
            var localFolder = await BaseApp.GetLocalDirAsync();

            using var fs = await localFolder.OpenStreamForWriteAsync(_source.UserFilename, CreationCollisionOption.ReplaceExisting);

            using var writter = new Utf8JsonWriter(fs, new JsonWriterOptions() {
                Indented = true
            });
            
            // write JSON to file
            JsonSerializer.Serialize(writter, GetSettingObjects());
        }

        #endregion


        #region Private functions

        /// <summary>
        /// Converts all settings to ExpandoObject for Json parsing
        /// </summary>
        /// <returns></returns>
        private static dynamic GetSettingObjects() {
            var settings = new ExpandoObject();

            var infoJson = new {
                _source.Description,
                _source.Version
            };

            settings.TryAdd("Info", infoJson);


            // Number values
            settings.TryAdd(nameof(SampleSetting), SampleSetting);


            return settings;
        }

        #endregion

    }
}
