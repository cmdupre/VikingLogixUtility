using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNetTests.Settings
{
    [TestFixture]
    internal class TagPathFixture
    {
        [Test]
        public void CreatesObjectAndReturnsAttributeString()
        {
            var gateway = "10.1.1.20";

            TagPath tp = new(gateway, 2);

            var fqn = "Fully Qualified Tag Name";

            Assert.That(tp.GetAttributeString(fqn), Is.EqualTo($"protocol=ab-eip&gateway={gateway}&path=1,2&plc=controllogix&name={fqn}"));
        }
    }
}
