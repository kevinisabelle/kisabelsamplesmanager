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
        public string libBaseFolder { get; set; }

        public Sample()
        {
            tags = "";
            key = "";
            notes = "";
            addedDate = DateTime.Now;
            libBaseFolder = "";
            genres = "";
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

        public string GetFullPath()
        {
            return libBaseFolder + path + filename;
        }

        public bool IsSameSample(Sample other)
        {
            if (other == null)
            {
                return false;
            }

            return GetFullPath() == other.GetFullPath();
        }

    }
}
