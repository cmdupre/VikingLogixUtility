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
            var tagInfo = TagInfo.Create(new TagPath(string.Empty, 0), 0, DataTypes.BOOL, 0, [], "name");

            Assert.That(tagInfo.ProgramName, Is.EqualTo(string.Empty));

            var programName = "Testing";

            tagInfo = tagInfo.WithProgramName(programName);

            Assert.That(tagInfo.ProgramName, Is.EqualTo(programName));
        }

        [Test]
        public void TestWithNewName()
        {
            var tagInfo = TagInfo.Create(new TagPath(string.Empty, 0), 0, DataTypes.BOOL, 0, [], "name");

            var name = "newName";

            var newTagInfo = tagInfo.WithNewName(name);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(newTagInfo.Name, Is.EqualTo(name));
                Assert.That(newTagInfo.Type, Is.EqualTo(DataTypes.BOOL));
            }
        }

        [Test]
        public void TestIsUdt()
        {
            // bit 15 set in type

            var tagInfo = TagInfo.Create(new TagPath(string.Empty, 0), 0, (DataTypes)32769, 0, [], "name");

            Assert.That(tagInfo.IsUdt, Is.True);

            // bit 15 not set

            tagInfo = TagInfo.Create(new TagPath(string.Empty, 0), 0, DataTypes.BOOL, 0, [], "name");

            Assert.That(tagInfo.IsUdt, Is.False);
        }

        [Test]
        public void TestStringIsNotUdt()
        {
            var tagInfo = TagInfo.Create(new TagPath(string.Empty, 0), 0, DataTypes.STRING, 0, [], "name");

            Assert.That(tagInfo.IsUdt, Is.False);
        }
    }
}
