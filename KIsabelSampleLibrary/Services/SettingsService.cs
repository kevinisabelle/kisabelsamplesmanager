using KIsabelSampleLibrary.Entity;
using Newtonsoft.Json;
using System.IO;

namespace KIsabelSampleLibrary.Services
{
    public class SettingsService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SettingsService));


        public string settingsfile = "settings.json";
        public Settings Settings {get; set; }

        public SettingsService()
        {
            LoadSettings();
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
