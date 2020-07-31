using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        /// Gets the path based on the startup folder of ImageGlass Preview.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string StartUpDir(params string[] paths) {
            var dir = Windows.Storage.ApplicationData.Current.LocalFolder.Path;

            var newPaths = paths.ToList();
            newPaths.Insert(0, dir);

            return Path.Combine(newPaths.ToArray());
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
