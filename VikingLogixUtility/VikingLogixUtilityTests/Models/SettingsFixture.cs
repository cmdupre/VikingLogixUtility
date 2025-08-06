using VikingLogixUtility.Models;

namespace VikingLogixUtilityTests.Models
{
    [TestFixture]
    internal class SettingsFixture
    {
        [Test]
        public void LoadsCorrectDefaults()
        {
            var settings = Settings.Load();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(settings.Address, Is.Empty);
                Assert.That(settings.Slot, Is.Zero);
            }
        }

        [Test]
        public void SavesSettings()
        {
            var filename = Path.GetTempFileName();

            var address = "testAddress";

            var slot = 2;

            var settings = new Settings(address, slot);

            settings.Write(filename);

            var settings2 = Settings.Load(filename);

            File.Delete(filename);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(settings2.Address, Is.EqualTo(address));
                Assert.That(settings2.Slot, Is.EqualTo(slot));
            }
        }
    }
}
