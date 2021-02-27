using KIsabelSampleLibrary.Entity;
using NAudio.Wave;
using NAudio.Wave.Asio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KIsabelSampleLibrary.Services
{
    public enum AudioDriverType
    {
        DirectSoundOut,
        ASIO
    }

    public class AudioService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AudioService));

        private SettingsService Settings { get; set; }


        public AudioService(SettingsService settings)
        {
            Settings = settings;
            //settings.Settings.DirectOutDeviceId = DirectSoundOut.DSDEVID_DefaultPlayback;
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

        public void DisposeAllDevices()
        {
            if (asioDevice!= null)
            {
                asioDevice.Dispose();
                asioDevice = null;
            }

            if (directSoundDevice != null)
            {
                directSoundDevice.Dispose();
                directSoundDevice = null;
            }
        }

        public DirectSoundOut GetDirectSoundOutDevice(Guid deviceId)
        {
            return new DirectSoundOut(deviceId);
        }

        public AsioOut GetAsioOutDevice(string deviceId)
        {
            return new AsioOut(deviceId);
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

            switch (Settings.Settings.AudioDriver)
            {
                case AudioDriverType.ASIO:
                    PlayAsioSample(sample, folders);
                    break;

                case AudioDriverType.DirectSoundOut:
                    PlayDirectSoundSample(sample, folders);
                    break;
            }

            
        }

        private DirectSoundOut directSoundDevice = null;

        private void PlayDirectSoundSample(Sample sample, List<SamplesFolder> folders)
        {

            if (directSoundDevice == null)
            {
                directSoundDevice = GetDirectSoundOutDevice(Settings.Settings.DirectOutDeviceId);
            }

            /*if (directSoundDevice.PlaybackState == PlaybackState.Playing)
            {
                directSoundDevice.Stop();
            }*/

            WaveStream mainOutputStream = new WaveFileReader(sample.GetFullPath(folders));
            WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);
            volumeStream.Volume = 1;
            directSoundDevice.Init(volumeStream);

            directSoundDevice.Play();
        }

        private AsioOut asioDevice = null;

        private void PlayAsioSample(Sample sample, List<SamplesFolder> folders)
        {
            try
            {
                if (asioDevice != null)
                {
                    AsioDriver driver = AsioDriver.GetAsioDriverByName(asioDevice.DriverName);
                    driver.DisposeBuffers();
                }

                if (asioDevice == null)
                {
                    asioDevice = GetAsioDevice(Settings.Settings.ASIODeviceId);
                }

                
                /*if (asioDevice.PlaybackState == PlaybackState.Playing)
                {
                    asioDevice.Stop();
                }*/


                WaveStream mainOutputStream = new WaveFileReader(sample.GetFullPath(folders));
                WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);
                volumeStream.Volume = 1;

                asioDevice.Init(mainOutputStream);
                // asioDevice.AutoStop = true;
                asioDevice.Play();

            }catch (Exception e)
            {
                log.Error(e);
            }
        }

        

    }
}
