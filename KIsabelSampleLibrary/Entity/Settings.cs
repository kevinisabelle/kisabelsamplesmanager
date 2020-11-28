using KIsabelSampleLibrary.Services;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;
using static KIsabelSampleLibrary.Services.AudioService;

namespace KIsabelSampleLibrary.Entity
{
    public class Settings
    {
        public List<string> SamplePaths { get; set; }
        
        public string AudioInterface { get; set; }

        public Guid DirectOutDeviceId { get; set; }
        
        public AudioDriverType AudioDriver { get; set; }
        
        public Settings()
        {
            SamplePaths = new List<string>();
            AudioDriver = AudioDriverType.DirectSoundOut;
            DirectOutDeviceId = Guid.Empty;
        }

        public void SanitizePaths()
        {
            for (int i =0; i<SamplePaths.Count; i++)
            {
                SamplePaths[i] = PathHelper.SanitizeBaseFolderPath(SamplePaths[i]);
            }
        }
    }
}
