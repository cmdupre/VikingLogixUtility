using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Data;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNetTests.Data
{
    [TestFixture]
    internal class TagInfoFieldFixture
    {
        [Test]
        public void TestWithProgramName()
        {
            var tagInfo = TagInfo.Create(new TagPath(string.Empty), 0, DataTypes.BOOL, 0, [], "name");

            Assert.That(tagInfo.ProgramName, Is.EqualTo(string.Empty));

            var programName = "Testing";

            tagInfo = tagInfo.WithProgramName(programName);

            Assert.That(tagInfo.ProgramName, Is.EqualTo(programName));
        }

        [Test]
        public void TestIsUdt()
        {
            // bit 15 set in type

            var tagInfo = TagInfo.Create(new TagPath(string.Empty), 0, (DataTypes)32769, 0, [], "name");

            Assert.That(tagInfo.IsUdt, Is.True);

            // bit 15 not set

            tagInfo = TagInfo.Create(new TagPath(string.Empty), 0, DataTypes.BOOL, 0, [], "name");

            Assert.That(tagInfo.IsUdt, Is.False);
        }
    }
}
