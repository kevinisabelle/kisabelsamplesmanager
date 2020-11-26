using KIsabelSampleLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Services
{
    
    public class SamplesService
    {
        private DatabaseContext DbContext { get; set; }
        private SettingsService Settings { get; set; }

        public SamplesService(DatabaseContext context, SettingsService settings)
        {
            DbContext = context;
            Settings = settings;
        }


    }
}
