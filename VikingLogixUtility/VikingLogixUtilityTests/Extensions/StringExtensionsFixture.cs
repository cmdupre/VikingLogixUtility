using VikingLogixUtility.Extensions;

namespace VikingLogixUtilityTests.Extensions
{
    [TestFixture]
    internal sealed class StringExtensionsFixture
    {
        [TestCase("Testing.EXT", "Testing_EXT.xlsx")]
        [TestCase(@"C:\Users\some.user\Desktop\Testing.EXT", @"C:\Users\some.user\Desktop\Testing_EXT.xlsx")]
        [TestCase("Testing", "Testing.xlsx")]
        [TestCase("", ".xlsx")]
        [TestCase(" ", " .xlsx")]
        public void GetsFilenameWithXlsxExtension(string input, string expected)
        {
            Assert.That(input.WithXlsxExtension(), Is.EqualTo(expected));
        }
    }
}