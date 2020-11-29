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
        
       
        public Guid DirectOutDeviceId { get; set; }

        public Guid WaveOutDeviceId { get; set; }

        public Guid WASAPIDeviceId { get; set; }

        public Guid ASIODeviceId { get; set; }

        public Guid MidiInputDeviceId { get; set; }

        public AudioDriverType AudioDriver { get; set; }


        public bool AutoplaySamplesOnClick { get; set; }

        

        
        public Settings()
        {
          
            AudioDriver = AudioDriverType.DirectSoundOut;
            DirectOutDeviceId = Guid.Empty;
            AutoplaySamplesOnClick = false;

        }

       
    }
}
