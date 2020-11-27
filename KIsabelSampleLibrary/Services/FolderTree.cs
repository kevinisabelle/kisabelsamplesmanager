using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Services
{
    public class FolderTree
    {
        public string BasePath { get; set; }

        public List<FolderTreeElement> Elements { get; set; }

    }

    public class FolderTreeElement
    {
        public string Path { get; set; }
        public List<FolderTreeElement> Elements { get; set; }
    }
}
