using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNetTests.Settings
{
    [TestFixture]
    internal class TagPathFixture
    {
        [Test]
        public void TestWithFqn()
        {
            var gateway = "10.1.1.20";

            TagPath tp = new(gateway);

            var fqn = "Fully Qualified Tag Name";

            Assert.That(tp.WithFqn(fqn), Is.EqualTo($"protocol=ab-eip&gateway={gateway}&path=1,0&plc=controllogix&name={fqn}"));
        }
    }
}
