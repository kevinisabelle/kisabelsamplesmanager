using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Entity
{
    public class Settings
    {
        List<string> SamplePaths { get; set; }

        public Settings()
        {
            SamplePaths = new List<string>();
        }

    }
}
