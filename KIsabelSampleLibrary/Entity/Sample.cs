using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIsabelSampleLibrary.Entity
{
    public class Sample
    {
        public int Id { get; set; }
        public string path { get; set; }
        public string filename { get; set; }
        public string tags { get; set; }
        public string type { get; set; }
        public string genres { get; set; }
        public int bpm { get; set; }
        public string key { get; set; }
        public int lengthMs { get; set; }
        public string notes { get; set; }
        public DateTime addedDate { get; set; }
        public bool favorite { get; set; }
        public bool isFilePresent { get; set; }
        public int SamplesFolderId { get; set; }

        public Sample()
        {
            tags = "";
            key = "";
            notes = "";
            addedDate = DateTime.Now;
            SamplesFolderId = 0;
            genres = "";
            isFilePresent = false;
        }

        public List<string> GetTags()
        {
            return tags.Split("|").ToList();
        }

        public List<string> GetGenres()
        {
            return genres.Split("|").ToList();
        }

        public void SetTags(List<string> tags)
        {
            this.tags = string.Join('|', tags);
        }

        public void SetGenres(List<string> genres)
        {
            this.genres = string.Join('|', genres);
        }

        public override string ToString()
        {
            return filename;
        }

        public string GetFullPath(List<SamplesFolder> folders)
        {
            return folders.First(f => f.Id == SamplesFolderId).BasePath + GetPartialPath();
        }

        public string GetPartialPath()
        {
            return path + filename;
        }

        public bool IsSameSample(Sample other)
        {
            if (other == null)
            {
                return false;
            }

            return GetPartialPath() == other.GetPartialPath();
        }

    }
}
