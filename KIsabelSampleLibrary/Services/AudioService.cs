using KIsabelSampleLibrary.Entity;
using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;

namespace KIsabelSampleLibrary.Services
{
    public class AudioService
    {
        public enum AudioDriverType
        {
            WaveOut,
            DirectSoundOut,
            WASAPI,
            ASIO
        }

        private SettingsService Settings { get; set; }

        public AudioService(SettingsService settings)
        {
            Settings = settings;
            settings.Settings.DirectOutDeviceId = DirectSoundOut.DSDEVID_DefaultPlayback;
        }

        public List<DirectSoundDeviceInfo> GetInterfacesForSelectedDriver()
        {
            switch (Settings.Settings.AudioDriver)
            {
                case AudioDriverType.DirectSoundOut:

                    return DirectSoundOut.Devices.ToList();
                    
            }

            return new List<DirectSoundDeviceInfo>();
        }

        public DirectSoundOut GetOutDevice()
        {
            return new DirectSoundOut(Settings.Settings.DirectOutDeviceId);
        }

        public void PlaySample(Sample sample)
        {
            DirectSoundOut device = GetOutDevice();

            WaveStream mainOutputStream = new WaveFileReader(sample.GetFullPath());
            WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);
            volumeStream.Volume = 1;
            device.Init(volumeStream);

            device.Play();

            device.Stop();
        }
    }
}
