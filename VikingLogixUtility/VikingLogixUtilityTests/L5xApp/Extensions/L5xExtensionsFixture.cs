using L5Sharp.Core;
using VikingLogixUtility.L5xApp.Extensions;

namespace VikingLogixUtilityTests.L5xApp.Extensions
{
    [TestFixture]
    internal class L5xExtensionsFixture
    {
        [Test]
        public void TestGetMappingRungs()
        {
            var content = L5X.Load("../../../L5xApp/Resources/Test.L5X");

            var mapping = content.GetMappingRungs();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(mapping.Count(), Is.EqualTo(166));
                Assert.That(mapping.First(), Is.EqualTo("XIC(Local:1:I.Pt00.Data)OTE(PSH_201A);"));
            }
        }
    }
}
