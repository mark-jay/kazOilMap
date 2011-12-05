using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;

namespace kazOilMap
{
    class Utils
    {
        /// <summary>
        /// </summary>
        /// <param name="array"></param>
        /// <returns>array of arrays where each array consecutive contains coordinates of points</returns>
        public static Point[][] toIsolines(double[][] array)
        {
            throw new NotSupportedException("method's not yet implemented");
        }

        /// <summary>
        /// works like Path.GetTempFileName, but for directories, name will be generated
        /// </summary>
        /// <returns></returns>
        public static string GetTempDirectory()
        {
            string path = Path.GetRandomFileName();

            string fullFilePath = Path.Combine(Path.GetTempPath(),path);

            Directory.CreateDirectory(fullFilePath);

            return fullFilePath;
        }

        public static string GetTempDirectory(string directoryName)
        {
            string tempDir = GetTempDirectory();

            string fullPath = Path.Combine(tempDir, directoryName);
            Directory.CreateDirectory(fullPath);

            return fullPath;
        }
    }
}
