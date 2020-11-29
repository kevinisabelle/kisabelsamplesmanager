using KIsabelSampleLibrary.Entity;
using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;

namespace KIsabelSampleLibrary.Services
{
    public class AudioService
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AudioService));

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

        public void PlaySample(Sample sample, List<SamplesFolder> folders)
        {
            if (!sample.isFilePresent)
            {
                return;
            }

            log.Debug("Playing sample: " + sample);

            DirectSoundOut device = GetOutDevice();

            WaveStream mainOutputStream = new WaveFileReader(sample.GetFullPath(folders));
            WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);
            volumeStream.Volume = 1;
            device.Init(volumeStream);

            device.Play();

            //device.Stop();
        }
    }
}
