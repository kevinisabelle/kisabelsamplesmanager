using System;
using System.Collections.Generic;
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

        public override string ToString()
        {
            return path + filename + " (" + lengthMs + " ms)";
        }

        public string GetFullPath()
        {
            return libBaseFolder + path + filename;
        }


    }
}
