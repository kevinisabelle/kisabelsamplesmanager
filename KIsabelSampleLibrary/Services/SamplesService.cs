using KIsabelSampleLibrary.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace KIsabelSampleLibrary.Services
{
    
    public class SamplesService
    {
        public delegate void UpdateFeedback(Sample sample, long currentCount, long totalCount, RefreshDataStatus threadstatus);

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
                result = result.Where(s => (s.libBaseFolder + s.path + s.filename).StartsWith(searchParameters.path));
            }

            return result.ToList();
        }

        public Sample GetSampleFromFullPath(string fullpath)
        {
            return DbContext.Samples.FirstOrDefault(s => (s.libBaseFolder + s.path + s.filename) == fullpath);
        }

        private Thread AnalysisThread = null;
        public struct RefreshParams
        {
            public string path;
            public UpdateFeedback updateFeedback;
        }

        public void RefreshDatabase(string path, UpdateFeedback updateFeedback = null)
        {
            if (AnalysisThread != null && AnalysisThread.IsAlive)
            {
                AnalysisThread.Interrupt();
                updateFeedback.Invoke(null, 0, 0, RefreshDataStatus.IDLE);
                
                return;
            }

            if (AnalysisThread == null || !AnalysisThread.IsAlive)
            {
                AnalysisThread = new Thread(RefreshDatabaseThread);
            }

            AnalysisThread.Start(new RefreshParams()
            {
                path = path,
                updateFeedback = updateFeedback
            });
        }

        public enum RefreshDataStatus
        {
            IDLE,
            PROCESSING,
            ERROR
        }

        public void RefreshDatabaseThread(object updateFeedbackp)
        {
            RefreshParams updateFeedback = (RefreshParams)updateFeedbackp;

            List<Sample> samplesFiles = AudioFileHelper.AnalyzePath(updateFeedback.path, updateFeedback.path);
            long total = samplesFiles.Count();
            long processed = 0;
            List<Sample> existingSamples = FindSamples(new SampleSearchModel()
            {
                path = updateFeedback.path
            });

            foreach (var sample in samplesFiles)
            {
                try
                {
                    Thread.Sleep(1);
                }
                catch (ThreadInterruptedException e)
                {
                    break;
                }

                AddSampleIfNotExist(sample, existingSamples);
                processed++;
                if (updateFeedback.updateFeedback != null)
                {
                    updateFeedback.updateFeedback.Invoke(sample, processed, total, RefreshDataStatus.PROCESSING);
                }
            }

            updateFeedback.updateFeedback.Invoke(null, processed, total, RefreshDataStatus.IDLE);
        }

        public void AddSampleIfNotExist(Sample sample, List<Sample> existingSamples)
        {
            Sample sampledb = existingSamples.FirstOrDefault(s => s.GetFullPath() == sample.GetFullPath());

            if (sampledb == null)
            {
                DbContext.Add(sample);
                existingSamples.Add(sample);
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
            List<Sample> samples = FindSamples(new SampleSearchModel());
            FolderTreeElement root = new FolderTreeElement() { Path = basePath };
            root.Elements = GetChildren(root, samples.Select(s => s.libBaseFolder + s.path).Distinct().ToList());
            tree.Elements = new List<FolderTreeElement>() { root };
            return tree;
        }

        private List<FolderTreeElement> GetChildren(FolderTreeElement element, List<string> paths)
        {
            List<FolderTreeElement> children = new List<FolderTreeElement>();

            string[] childrenPaths = Directory.GetDirectories(element.Path);

            foreach (string path in childrenPaths)
            {
                if (!paths.Any(p => p.StartsWith(path)))
                {
                    continue;
                }
                FolderTreeElement elementChild = new FolderTreeElement() { Path = path};
                elementChild.Elements = GetChildren(elementChild, paths);
                children.Add(elementChild);
            }

            return children;
        }
    }
}
