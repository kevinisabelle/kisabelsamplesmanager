using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Entity
{
    public class Settings
    {
        public List<string> SamplePaths { get; set; }
        public string AudioInterface { get; set; }
        

        public Settings()
        {
            SamplePaths = new List<string>();
        }

    }
}
