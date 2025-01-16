using System.Collections.ObjectModel;
using VikingLogixUtility.Extensions;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtilityTests.Extensions
{
    [TestFixture]
    internal class DisplayStringExtensionsFixture
    {
        [Test]
        public void TestComparer()
        {
            var rows = new ObservableCollection<DisplayStringViewModel>()
            {
                new("A"),
                new("D"),
                new("B"),
                new("C"),
            };

            rows = rows.Sort();

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].DisplayName, Is.EqualTo("A"));
                Assert.That(rows[1].DisplayName, Is.EqualTo("B"));
            });
        }

        [Test]
        public void TestComparerWithNumbers()
        {
            var rows = new ObservableCollection<DisplayStringViewModel>()
            {
                new("2"),
                new("1"),
            };

            rows = rows.Sort();

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].DisplayName, Is.EqualTo("1"));
                Assert.That(rows[1].DisplayName, Is.EqualTo("2"));
            });
        }

        [Test]
        public void TestSortsNegativeValuesProperly()
        {
            var rows = new ObservableCollection<DisplayStringViewModel>()
            {
                new("1"),
                new("0"),
                new("0.9"),
                new("-1"),
                new("-0.123"),
                new("0"),
            };

            rows = rows.Sort();

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].DisplayName, Is.EqualTo("-1"));
                Assert.That(rows[1].DisplayName, Is.EqualTo("-0.123"));
                Assert.That(rows[2].DisplayName, Is.EqualTo("0"));
                Assert.That(rows[3].DisplayName, Is.EqualTo("0"));
                Assert.That(rows[4].DisplayName, Is.EqualTo("0.9"));
                Assert.That(rows[5].DisplayName, Is.EqualTo("1"));
            });
        }
    }
}
