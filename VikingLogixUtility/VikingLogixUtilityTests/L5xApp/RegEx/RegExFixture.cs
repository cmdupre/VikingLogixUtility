using VikingLogixUtility.L5xApp.RegEx;

namespace VikingLogixUtilityTests.L5xApp.RegEx
{
    [TestFixture]
    internal class RegExFixture
    {
        [TestCase("", false)]
        [TestCase("something else", false)]
        [TestCase("Local:1:I.Pt00.Data", true)]
        [TestCase("Local:1:I.Pt00.DATA", true)]
        [TestCase("Local:1:I.Pt00. DATA", true)]
        [TestCase("TruckDockRIO:1:I.Data.0", true)]
        public void TestMappingFilter(string input, bool expected)
        {
            Assert.That(RegularExpressions.MappingFilter().IsMatch(input), Is.EqualTo(expected), input);
        }
    }
}
