using KIsabelSampleLibrary.Entity;
using System;
using System.Collections.Generic;
using System.IO;
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
                .ToList()
                .SelectMany(s => s.GetTags())
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }

        public List<string> GetGenres()
        {
            return DbContext.Samples
                .ToList()
                .SelectMany(s => s.GetGenres())
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }

        public List<Sample> FindSamples(SampleSearchModel searchParameters)
        {
            IQueryable<Sample> result = DbContext.Samples.AsQueryable() ;

            if (searchParameters.query != null)
            {
                result = result.Where(s => s.filename.Contains(searchParameters.query));
            }

            if (searchParameters.path != null)
            {
                result = result.Where(s => s.path.StartsWith(searchParameters.path));
            }

            return result.ToList();
        }

        public Sample GetSampleFromFullPath(string fullpath)
        {
            return DbContext.Samples.FirstOrDefault(s => (s.libBaseFolder + s.path + s.filename) == fullpath);
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

        public FolderTree GetFolderTree(string basePath)
        {
            FolderTree tree = new FolderTree();
            FolderTreeElement root = new FolderTreeElement() { Path = basePath };
            root.Elements = GetChildren(root);
            tree.Elements = new List<FolderTreeElement>() { root };
            return tree;
        }

        private List<FolderTreeElement> GetChildren(FolderTreeElement element)
        {
            List<FolderTreeElement> children = new List<FolderTreeElement>();

            string[] childrenPaths = Directory.GetDirectories(element.Path);

            foreach (string path in childrenPaths)
            {
                FolderTreeElement elementChild = new FolderTreeElement() { Path = path};
                elementChild.Elements = GetChildren(elementChild);
                children.Add(elementChild);
            }

            return children;
        }
    }
}
