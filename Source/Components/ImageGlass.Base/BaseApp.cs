using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace ImageGlass.Base {
    /// <summary>
    /// Provides the base information of ImageGlass Preview
    /// </summary>
    public class BaseApp {

        /// <summary>
        /// Command line argument prefix
        /// </summary>
        const string CMD_PREFIX = "--";


        /// <summary>
        /// Gets StorageFolder based on the startup folder of ImageGlass Preview.
        /// </summary>
        /// <param name="dir">Subfolder name</param>
        /// <returns></returns>
        public static async Task<StorageFolder> GetLocalDirAsync(Dirs dir = Dirs._Root) {
            if (dir == Dirs._Root) {
                return ApplicationData.Current.LocalFolder;
            }

            if (dir == Dirs._Temp) {
                return ApplicationData.Current.TemporaryFolder;
            }


            return await ApplicationData.Current.LocalFolder.CreateFolderAsync(dir.ToString(), CreationCollisionOption.OpenIfExists);
        }


        /// <summary>
        /// Gets command line arguments
        /// </summary>
        /// <param name="options">Filter options</param>
        /// <returns></returns>
        public static string[] GetCommandLineArgs(CommandLineArgFilterOptions options = CommandLineArgFilterOptions.None) {
            var queries = Environment.GetCommandLineArgs() as IEnumerable<string>;

            #region Only keep settings command line arguments
            if ((options & CommandLineArgFilterOptions.OnlySettings) == CommandLineArgFilterOptions.OnlySettings) {
                queries = queries
                    // find the commands with prefix
                    .Where(cmd => cmd.StartsWith(CMD_PREFIX))

                    // remove the prefix
                    .Select(cmd => cmd.Substring(2));
            }
            #endregion


            #region Exclude settings command line arguments
            if ((options & CommandLineArgFilterOptions.ExludeSettings) == CommandLineArgFilterOptions.ExludeSettings) {
                queries = queries
                    // find the commands with prefix
                    .Where(cmd => !cmd.StartsWith(CMD_PREFIX));
            }
            #endregion


            return queries.ToArray();
        }
    }
}
