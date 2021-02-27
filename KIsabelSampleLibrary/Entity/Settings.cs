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

        public string ASIODeviceId { get; set; }

        public Guid MidiInputDeviceId { get; set; }

        public AudioDriverType AudioDriver { get; set; }


        public bool AutoplaySamplesOnClick { get; set; }

        public List<string> MidiInputNotes { get; set; }

        
        public Settings()
        {
            MidiInputNotes = new List<string>()
            {
                "C3",
                "C#3",
                "D3",
                "D#3",
                "E3",
                "F3",
                "F#3",
                "G3",

                "C4",
                "C#4",
                "D4",
                "D#4",
                "E4",
                "F4",
                "F#4",
                "G4",
            };

            AudioDriver = AudioDriverType.DirectSoundOut;
            DirectOutDeviceId = Guid.Empty;
            AutoplaySamplesOnClick = false;

        }

       
    }
}
