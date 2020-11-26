using KIsabelSampleLibrary.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Services
{
    public class ServicesManager
    {
        private SamplesService _Samples { get; set; }
        private SettingsService _Settings { get; set; }
        private DatabaseContext _dbContext { get; set; }

        public ServicesManager()
        {
            _dbContext = new DatabaseContext();
            _dbContext.Database.Migrate();
            _Settings = new SettingsService();
            _Samples = new SamplesService(_dbContext, _Settings);
            
        }

        public SamplesService Samples()
        {
            return _Samples;
        }

        public SettingsService Settings()
        {
            return _Settings;
        }
    }
}
