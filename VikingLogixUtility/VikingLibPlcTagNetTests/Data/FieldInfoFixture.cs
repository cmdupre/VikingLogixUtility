using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Data;

namespace VikingLibPlcTagNetTests.Data
{
    [TestFixture]
    internal class FieldInfoFixture
    {
        [Test]
        public void TestWithName()
        {
            var fieldInfo = FieldInfo.Create(0, 0, (ushort)DataTypes.BOOL);

            Assert.That(fieldInfo.Name, Is.EqualTo(string.Empty));

            var name = "Testing";

            fieldInfo = fieldInfo.WithName(name);

            Assert.That(fieldInfo.Name, Is.EqualTo(name));
        }

        [Test]
        public void TestIsUdt()
        {
            // bit 15 set in type

            var fieldInfo = FieldInfo.Create(0, 32769, 0);

            Assert.That(fieldInfo.IsUdt, Is.True);

            // bit 15 not set

            fieldInfo = FieldInfo.Create(0, 1, 0);

            Assert.That(fieldInfo.IsUdt, Is.False);
        }
    }
}
