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
        public string testdatapath = "../../../../testdata/";

        [TestMethod]
        public void SaveAndLoadSettings()
        {
            ServicesManager services = new ServicesManager();
            services.Settings().LoadSettings();
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
            SamplesFolder folder = new SamplesFolder()
            {
                Id = 0,
                BasePath = testdatapath
            };

            ServicesManager services = new ServicesManager();

            List<Sample> samples = AudioFileHelper.AnalyzePath(testdatapath, folder, null);

            services.Audio().PlaySample(samples.First(), new List<SamplesFolder>() { folder });
        }

        [TestMethod]
        public void TestEnumerateDevies()
        {
            ServicesManager services = new ServicesManager();
          
            services.Settings().Settings.AudioDriver = AudioService.AudioDriverType.DirectSoundOut;
            services.Settings().SaveSettings();
        }

        [TestMethod]
        public void TestRefreshDatabase()
        {
            SamplesFolder folder = new SamplesFolder()
            {
                Id = 0,
                BasePath = testdatapath
            };

            ServicesManager services = new ServicesManager();
        
            services.Settings().Settings.AudioDriver = AudioService.AudioDriverType.DirectSoundOut;
            services.Settings().SaveSettings();

            services.Samples().RefreshDatabase();

            string tag1 = "tag1";
            string tag2 = "tag2";

            string genre1 = "genre1";
            string genre2 = "genre2";

            List<Sample> samples = services.Samples().FindSamples(new SampleSearchModel());

            samples.ForEach(s =>
            {
                s.SetTags(new List<string>() { tag1, tag2 });
                s.SetGenres(new List<string>() { genre1, genre2 });
                services.Samples().SaveSample(s);
            });

            List<string> tags = services.Samples().GetTags();
            List<string> genre = services.Samples().GetGenres();

            samples = services.Samples().FindSamples(new SampleSearchModel()
            {
                path = "\\Lush Dancehall ft. Patexx"
            });

            Assert.AreEqual(samples.Count,8);
        }
    }
}
