using KIsabelSampleLibrary.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Services
{
    public class ServicesManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ServicesManager));

        private SamplesService _Samples { get; set; }
        private SettingsService _Settings { get; set; }
        private DatabaseContext _dbContext { get; set; }

        private AudioService _audio { get; set; }

        public ServicesManager()
        {
            log.Info("Starting services...");
            _dbContext = new DatabaseContext();
            _dbContext.Database.Migrate();
            _Settings = new SettingsService();
            _Samples = new SamplesService(_dbContext, _Settings);
            _audio = new AudioService(_Settings);
            log.Info("Dont starting servicess.");

        }

        public SamplesService Samples()
        {
            return _Samples;
        }

        public SettingsService Settings()
        {
            return _Settings;
        }

        public DatabaseContext Db()
        {
            return _dbContext;
        }

        public AudioService Audio()
        {
            return _audio;
        }

    }
}
