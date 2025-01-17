using VikingLogixUtility.RegEx;

namespace VikingLogixUtilityTests.RegEx
{
    [TestFixture]
    internal class RegularExpressionsFixture
    {
        [TestCase("10.200.201.30", true)]
        [TestCase("10.1.1.1", true)]
        [TestCase("0.1.1.1", true)]
        [TestCase("10.1.1.1.", false)]
        [TestCase("10.1.1", false)]
        [TestCase("256.1.1.1", false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("testing", false)]
        public void TestValidIpAddress(string input, bool expected)
        {
            Assert.That(RegularExpressions.MappingFilter().IsMatch(input), Is.EqualTo(expected));
        }
    }
}
