using KIsabelSampleLibrary.Entity;
using KIsabelSampleLibrary.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class BasicTests
    {
        public string testdatapath = "../../../testdata/";

        [TestMethod]
        public void SaveAndLoadSettings()
        {
            ServicesManager services = new ServicesManager();

            services.Settings().LoadSettings();

            services.Settings().Settings.SamplePaths.Add("K:\\Google Drive\\Studio Data\\Samples2020");
            services.Settings().Settings.AudioInterface = "Interface #1";

            services.Settings().SaveSettings();

            services.Settings().LoadSettings();
            Assert.AreEqual(services.Settings().Settings.AudioInterface, "Interface #1");


        }

        [TestMethod]
        public void SaveAndLoadSampleEntity()
        {
            ServicesManager services = new ServicesManager();

            Sample sampleData = new Sample()
            {
                filename = "somefile.wav",
                bpm = 92,
                genres = "reggaeton,pop",
                key = "c#",
                lengthMs = 1500,
                path = "somepath",
                tags = "[\"kick\", \"kick\"]",
                type = "none"
            };

            services.Db().Samples.Add(sampleData);
            services.Db().SaveChanges();

            List<Sample> samples = services.Db().Samples.ToList();

            Assert.AreEqual(1, samples.Count());
        }

        [TestMethod]
        public void TestReadwaveFile()
        {
            ServicesManager services = new ServicesManager();

            AudioFileReader reader = new AudioFileReader(testdatapath + "ADSR Major Lazer Clap.wav");
            long length = (long)reader.TotalTime.TotalMilliseconds;
            System.Console.WriteLine(length);

            WaveFormat format = reader.ToSampleProvider().WaveFormat;
        }

        [TestMethod]
        public void TestGetSamplesFromPath()
        {
            ServicesManager services = new ServicesManager();
            services.Settings().Settings.SamplePaths.Add(testdatapath);
            services.Settings().SaveSettings();

            List<Sample> samples = AudioFileHelper.AnalyzePath(testdatapath, testdatapath);

            services.Audio().PlaySample(samples.First());
        }

        [TestMethod]
        public void TestEnumerateDevies()
        {
            ServicesManager services = new ServicesManager();
            services.Settings().Settings.SamplePaths.Add(testdatapath);
            services.Settings().Settings.AudioDriver = AudioService.AudioDriverType.DirectSoundOut;
            services.Settings().SaveSettings();

           // List<DirectSoundOut> devices = services.Audio().GetInterfacesForSelectedDriver();

        }
    }
}
