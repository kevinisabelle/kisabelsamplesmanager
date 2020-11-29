using KIsabelSampleLibrary.Entity;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KIsabelSampleLibrary.Services
{
    public enum AudioDriverType
    {
        WaveOut,
        DirectSoundOut,
        WASAPI,
        ASIO
    }

    public class AudioService
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AudioService));

        

        private SettingsService Settings { get; set; }

        public AudioService(SettingsService settings)
        {
            Settings = settings;
            settings.Settings.DirectOutDeviceId = DirectSoundOut.DSDEVID_DefaultPlayback;
        }


        public static List<KeyValuePair<string, string>> GetAvailableInterfaces(AudioDriverType driverType)
        {
            switch (driverType)
            {
                case AudioDriverType.DirectSoundOut:

                    return DirectSoundOut.Devices.Select(d => new KeyValuePair<string, string>(d.Guid.ToString(), d.Description)).ToList();

                case AudioDriverType.ASIO:
                    return AsioOut.GetDriverNames().Select(d => new KeyValuePair<string, string>(d, d)).ToList();

            }

            return new List<KeyValuePair<string, string>>();
        }

        

        public DirectSoundOut GetDirectSoundOutDevice(Guid deviceId)
        {
            return new DirectSoundOut(deviceId);
        }

        public AsioOut GetAsioDevice(string driverName)
        {
            return new AsioOut(driverName);
        }

        public void PlaySample(Sample sample, List<SamplesFolder> folders)
        {
            if (!sample.isFilePresent)
            {
                return;
            }

            log.Debug("Playing sample: " + sample);

            DirectSoundOut device = GetDirectSoundOutDevice(Settings.Settings.DirectOutDeviceId);

            WaveStream mainOutputStream = new WaveFileReader(sample.GetFullPath(folders));
            WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);
            volumeStream.Volume = 1;
            device.Init(volumeStream);

            device.Play();

            //device.Stop();
        }
    }
}
