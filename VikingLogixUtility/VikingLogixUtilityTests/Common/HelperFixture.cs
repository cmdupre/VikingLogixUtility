using L5Sharp.Core;
using VikingLogixUtility.Common;

namespace VikingLogixUtilityTests.Common
{
    [TestFixture]
    internal class HelperFixture
    {
        [Test]
        public void TestGetMappingRungs()
        {
            var content = L5X.Load("../../../L5xApp/Resources/Test.L5X");

            var mapping = Helper.GetMappingRungs(content);

            Assert.Multiple(() =>
            {
                Assert.That(mapping.Count(), Is.EqualTo(166));
                Assert.That(mapping.First(), Is.EqualTo("XIC(Local:1:I.Pt00.Data)OTE(PSH_201A);"));
            });
        }
    }
}
