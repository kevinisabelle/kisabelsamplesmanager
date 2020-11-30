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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SamplesService));

        public delegate void UpdateFeedback(Sample sample, long currentCount, long totalCount, SamplesFolder folder, int folderCurrentCount, int folderTotalCount, RefreshDataStatus threadstatus);

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

        public List<SamplesFolder> GetFolders()
        {
            return DbContext.SamplesFolders.ToList();
        }

        public List<Sample> FindSamples(SampleSearchModel searchParameters)
        {
            IQueryable<Sample> result = DbContext.Samples.ToList().AsQueryable();
            List<SamplesFolder> folders = GetFolders();

            if (searchParameters.query != null)
            {
                result = result.Where(s => s.GetFullAbsolutePath(folders).ToLower().Contains(searchParameters.query.ToLower()));
            }

            if (searchParameters.path != null)
            {
                result = result.Where(s => s.GetFullPath(folders).StartsWith(searchParameters.path));
            }

            if (searchParameters.favorites.HasValue && searchParameters.favorites.Value)
            {
                result = result.Where(s => s.favorite); 
            }

            if (searchParameters.missingFiles.HasValue && searchParameters.missingFiles.Value)
            {
                result = result.Where(s => !s.isFilePresent);
            }


            return result.ToList();
        }

        public void RemoveMissingFiles()
        {
            List<Sample> samples = FindSamples(new SampleSearchModel());

            foreach (var sample in samples)
            {
                if (!sample.isFilePresent)
                {
                    DbContext.Samples.Remove(sample);
                    DbContext.SaveChanges();
                }
            }
        }

        public Sample GetSampleFromFullPath(string fullpath, List<SamplesFolder> folders)
        {
            return DbContext.Samples.FirstOrDefault(s => (s.GetFullPath(folders) == fullpath));
        }

        private Thread AnalysisThread = null;
        public struct RefreshParams
        {
            public List<SamplesFolder> folders;
            public UpdateFeedback updateFeedback;
        }

        public void RefreshDatabase(UpdateFeedback updateFeedback = null)
        {
            if (AnalysisThread != null && AnalysisThread.IsAlive)
            {
                AnalysisThread.Interrupt();
                updateFeedback.Invoke(null, 0, 0, null, 0, 0, RefreshDataStatus.IDLE);
                
                return;
            }

            if (AnalysisThread == null || !AnalysisThread.IsAlive)
            {
                AnalysisThread = new Thread(RefreshDatabaseThread);
            }

            AnalysisThread.Start(new RefreshParams()
            {
                folders = GetFolders(),
                updateFeedback = updateFeedback
            });
        }

        public enum RefreshDataStatus
        {
            IDLE,
            PROCESSING,
            CHECKING_FILES,
            ERROR
        }

        public void RefreshDatabaseThread(object paramsParam)
        {
            RefreshParams paramsObject = (RefreshParams)paramsParam;
            int totalFolders = paramsObject.folders.Count();
            int currentFolder = 0;

            foreach (var folder in paramsObject.folders)
            {
                currentFolder++;

                List<Sample> samplesFiles = AudioFileHelper
                    .AnalyzePath(folder.BasePath, folder, paramsObject.updateFeedback);

                long total = samplesFiles.Count();
                List<SamplesFolder> folders = GetFolders();
                long processed = 0;
                List<Sample> existingSamples = FindSamples(new SampleSearchModel()
                {
                    path = folder.BasePath
                });

                foreach (var sample in samplesFiles)
                {
                    AddSampleIfNotExist(sample, existingSamples, folders);
                    processed++;
                    if (paramsObject.updateFeedback != null)
                    {
                        paramsObject.updateFeedback.Invoke(sample, processed, total, folder, currentFolder, totalFolders, RefreshDataStatus.PROCESSING);
                    }
                }

                samplesFiles = FindSamples(new SampleSearchModel());

                processed = 0;
                total = samplesFiles.Count();

                foreach (var sample in samplesFiles)
                {
                    processed++;
                    string sampleFilePath = sample.GetFullPath(folders);
                    bool fileExists = File.Exists(sampleFilePath);

                    if (sample.isFilePresent != fileExists)
                    {
                        sample.isFilePresent = fileExists;
                        SaveSample(sample);
                    }

                    if (paramsObject.updateFeedback != null)
                    {
                        paramsObject.updateFeedback.Invoke(sample, processed, total, folder, currentFolder, totalFolders, RefreshDataStatus.CHECKING_FILES);
                    }
                }
            }

            paramsObject.updateFeedback.Invoke(null, 0, 0, null, totalFolders, currentFolder, RefreshDataStatus.IDLE);
        }

        public void AddSampleIfNotExist(Sample sample, List<Sample> existingSamples, List<SamplesFolder> folders)
        {
            Sample sampledb = existingSamples.FirstOrDefault(s => s.GetFullPath(folders) == sample.GetFullPath(folders));

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

        public FolderTree GetFolderTree(List<SamplesFolder> folders)
        {
            FolderTree tree = new FolderTree();
            List<Sample> samples = FindSamples(new SampleSearchModel());
            tree.Elements = new List<FolderTreeElement>();

            foreach (var folder in folders)
            {
                FolderTreeElement root = new FolderTreeElement() { Path = folder.BasePath };
                root.Folder = folder;
                root.Elements = GetChildren(root, samples.Select(s => folders.First(f => f.Id == s.SamplesFolderId).BasePath + s.path).Distinct().ToList());
                tree.Elements.Add(root);
            }
            
            return tree;
        }

        private List<FolderTreeElement> GetChildren(FolderTreeElement element, List<string> paths)
        {
            List<FolderTreeElement> children = new List<FolderTreeElement>();

            if (element.Path != null)
            {
                string[] childrenPaths = Directory.GetDirectories(element.Path);

                foreach (string path in childrenPaths)
                {
                    if (!paths.Any(p => p.StartsWith(path)))
                    {
                        continue;
                    }
                    FolderTreeElement elementChild = new FolderTreeElement() { Path = path };
                    elementChild.Elements = GetChildren(elementChild, paths);
                    children.Add(elementChild);
                }
            }

            return children;
        }
    }
}
