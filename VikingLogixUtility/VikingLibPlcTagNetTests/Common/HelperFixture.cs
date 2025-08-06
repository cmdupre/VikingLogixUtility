using VikingLibPlcTagNet.Common;

namespace VikingLibPlcTagNetTests.Common
{
    [TestFixture]
    internal class HelperFixture
    {
        [TestCase(0, false, null)]
        [TestCase(0xC1, true, DataTypes.BOOL)]
        public void TestGetDataType(int value, bool expectedResult, DataTypes? expectedType)
        {
            var result = Helper.TryGetDataType(value, out var type);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expectedResult));
                Assert.That(type, Is.EqualTo(expectedType));
            }
        }
    }
}
