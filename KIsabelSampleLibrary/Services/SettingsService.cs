using KIsabelSampleLibrary.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace KIsabelSampleLibrary.Services
{
    public class SettingsService
    {
        public string settingsfile = "settings.json";
        public Settings Settings {get; set; }

        public SettingsService()
        {

        }

        public void LoadSettings()
        {
            if (File.Exists(settingsfile))
            {
                Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsfile));
                return;
            }

            Settings = new Settings();
        }

        public void SaveSettings()
        {
            File.WriteAllText(settingsfile, JsonConvert.SerializeObject(Settings));
        }


    }
}
