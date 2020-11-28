using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Services
{
    public static class PathHelper
    {

        public static string SanitizeBaseFolderPath(string basePath)
        {
            string result = basePath.Replace("/", "\\");
            result = result.Replace("\\\\", "\\");

            if (!result.EndsWith("\\"))
            {
                result = result + "\\";
            }

            return result;
        }

        public static string SanitizeSamplePathFolder(string samplePath, bool withEndSlash = true)
        {
            string result = samplePath.Replace("/", "\\");
            result = result.Replace("\\\\", "\\");

            if (withEndSlash)
            {
                if (!result.EndsWith("\\"))
                {
                    result = result + "\\";
                }
            } else
            {
                if (result.EndsWith("\\"))
                {
                    result = result.Substring(0, result.Length-1);
                }
            }

            if (result.StartsWith("\\"))
            {
                result = result.Substring(1);
            }

            return result;
        }

    }
}
