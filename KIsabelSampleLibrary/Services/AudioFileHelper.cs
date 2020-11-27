using KIsabelSampleLibrary.Entity;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KIsabelSampleLibrary.Services
{
    public class AudioFileHelper
    {
        public static Sample AnalyzeFile(string path, string samplesBasePath)
        {
            if (File.Exists(path))
            {
                AudioFileReader reader = new AudioFileReader(path);

                return new Sample()
                {
                    lengthMs = (int)reader.TotalTime.TotalMilliseconds,
                    filename = Path.GetFileName(path),
                    addedDate = DateTime.Now,
                    path = Path.GetDirectoryName(reader.FileName).Replace(Path.GetDirectoryName(samplesBasePath), ""),
                    libBaseFolder = samplesBasePath
                    
                };
            }

            return null;
        }

        public static List<Sample> AnalyzePath(string path, string libBasePath)
        {
            List<Sample> result = new List<Sample>();

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                result.Add(AnalyzeFile(file, libBasePath));
            }

            foreach (string directories in Directory.GetDirectories(path))
            {
                result.AddRange(AnalyzePath(directories, libBasePath));
            }

            return result;
        }

        
    }
}
