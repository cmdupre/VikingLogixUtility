using VikingLogixUtility.Models;

namespace VikingLogixUtilityTests.Models
{
    [TestFixture]
    internal class SettingsFixture
    {
        [Test]
        public void TestLoadDefault()
        {
            var settings = Settings.Load();

            Assert.That(settings.Address, Is.Empty);
        }

        [Test]
        public void TestCanSave()
        {
            var filename = Path.GetTempFileName();

            var address = "testAddress";

            var settings = new Settings(address);

            settings.Write(filename);

            var settings2 = Settings.Load(filename);

            File.Delete(filename);

            Assert.That(settings2.Address, Is.EqualTo(address));
        }
    }
}
