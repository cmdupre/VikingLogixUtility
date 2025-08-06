using VikingLibPlcTagNet.Data;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNetTests.Data
{
    [TestFixture]
    internal class TemplateInfoFixture
    {
        [Test]
        public void TestWithProgramName()
        {
            var templateInfo = TemplateInfo.Create(
                new TagPath(string.Empty, 0),
                string.Empty,
                0, 0, 0, 0, 0, 0, 0,
                Array.Empty<FieldInfo>());

            Assert.That(templateInfo.ProgramName, Is.EqualTo(string.Empty));

            var programName = "Testing";

            templateInfo = templateInfo.WithProgramName(programName);

            Assert.That(templateInfo.ProgramName, Is.EqualTo(programName));
        }
    }
}
