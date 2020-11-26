using KIsabelSampleLibrary.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {


        [TestMethod]
        public void TestMethod1()
        {
            ServicesManager services = new ServicesManager();

            services.Settings().LoadSettings();

            services.Settings().SaveSettings();


        }
    }
}
