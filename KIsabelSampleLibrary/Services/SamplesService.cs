using KIsabelSampleLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIsabelSampleLibrary.Services
{
    
    public class SamplesService
    {
        public delegate void UpdateFeedback(Sample sample);

        private DatabaseContext DbContext { get; set; }
        private SettingsService Settings { get; set; }

        public SamplesService(DatabaseContext context, SettingsService settings)
        {
            DbContext = context;
            Settings = settings;
        }

        public List<string> GetTags()
        {
            return DbContext.Samples
                .SelectMany(s => s.GetTags())
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }

        public List<string> GetGenres()
        {
            return DbContext.Samples
                .SelectMany(s => s.GetGenres())
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }

        public List<Sample> FindSamples(SampleSearchModel searchParameters)
        {
            IQueryable<Sample> result = DbContext.Samples.Where(s => true);

            if (searchParameters.query != "")
            {
                result = result.Where(s => s.GetFullPath().Contains(searchParameters.query));
            }

            return result.ToList();
        }

        public Sample GetSampleFromFullPath(string fullpath)
        {
            return DbContext.Samples.FirstOrDefault(s => s.GetFullPath() == fullpath);
        }

        public void RefreshDatabase(string path, UpdateFeedback updateFeedback = null)
        {
            List<Sample> samplesFiles = AudioFileHelper.AnalyzePath(path, path);
            foreach (var sample in samplesFiles)
            {
                AddSampleIfNotExist(sample);

                if (updateFeedback != null)
                {
                    updateFeedback.Invoke(sample);
                }
            }
        }

        public void AddSampleIfNotExist(Sample sample)
        {
            Sample sampledb = GetSampleFromFullPath(sample.GetFullPath());

            if (!sample.IsSameSample(sampledb))
            {
                DbContext.Add(sample);
                DbContext.SaveChanges();
            }
        }

        public void SaveSample(Sample sample)
        {
            DbContext.Update(sample);
            DbContext.SaveChanges();
        }
    }
}
