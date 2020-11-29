using KIsabelSampleLibrary.Entity;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static KIsabelSampleLibrary.Services.SamplesService;

namespace KIsabelSampleLibrary.Services
{
    public class AudioFileHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AudioFileHelper));


        public static Sample AnalyzeFile(string path, SamplesFolder samplesBasePath)
        {
            try
            {
                if (File.Exists(path))
                {
                    if (!path.EndsWith(".wav"))
                    {
                        return null;
                    }

                    AudioFileReader reader = new AudioFileReader(path);

                    return new Sample()
                    {
                        lengthMs = (int)reader.TotalTime.TotalMilliseconds,
                        filename = Path.GetFileName(path),
                        addedDate = DateTime.Now,
                        path = PathHelper.SanitizeSamplePathFolder(Path.GetDirectoryName(reader.FileName).Replace(Path.GetDirectoryName(samplesBasePath.BasePath), "")),
                        SamplesFolderId = samplesBasePath.Id,
                        isFilePresent = true

                    };
                }
            } catch (Exception e)
            {
                log.Error(e);
            }

            return null;
        }

        public static List<Sample> AnalyzePath(string path, SamplesFolder libBasePath, UpdateFeedback feedback)
        {
            List<Sample> result = new List<Sample>();

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                Sample sample = AnalyzeFile(file, libBasePath);

                if (sample != null)
                {
                    result.Add(sample);
                }

                feedback.Invoke(sample, -1, -1, libBasePath, 0, 0, RefreshDataStatus.PROCESSING);
            }

            foreach (string directories in Directory.GetDirectories(path))
            {
                result.AddRange(AnalyzePath(directories, libBasePath, feedback));
            }

            return result;
        }

        
    }
}
