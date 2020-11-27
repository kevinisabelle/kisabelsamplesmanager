using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Services
{
    public class SampleSearchModel
    {
        public enum ConnectionType
        {
            AND,
            OR
        }

        public string query = null;
        public string[] tags = null;
        public ConnectionType tagsSearchType = ConnectionType.AND;
        public string[] genres = null;
        public ConnectionType genresSearchType = ConnectionType.AND;
        public bool? favorites = null;

    }
}
