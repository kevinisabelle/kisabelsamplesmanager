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
        
        public string AudioInterface { get; set; }

        public Guid DirectOutDeviceId { get; set; }
        
        public AudioDriverType AudioDriver { get; set; }
        
        public Settings()
        {
          
            AudioDriver = AudioDriverType.DirectSoundOut;
            DirectOutDeviceId = Guid.Empty;
        }

       
    }
}
