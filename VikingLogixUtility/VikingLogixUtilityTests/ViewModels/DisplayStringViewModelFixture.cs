using System.Collections.ObjectModel;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtilityTests.ViewModels
{
    [TestFixture]
    internal class DisplayStringViewModelFixture
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

            var orderedRows = rows
                .OrderBy(x => x)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(orderedRows[0].DisplayName, Is.EqualTo("A"));
                Assert.That(orderedRows[1].DisplayName, Is.EqualTo("B"));
            });
        }

        [Test]
        public void TestComparerWithNumbers()
        {
            var rows = new ObservableCollection<DisplayStringViewModel>()
            {
                new("1"),
                new("10"),
                new("9"),
                new("11"),
            };

            var orderedRows = rows
                .OrderBy(x => x)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(orderedRows[0].DisplayName, Is.EqualTo("1"));
                Assert.That(orderedRows[1].DisplayName, Is.EqualTo("9"));
                Assert.That(orderedRows[2].DisplayName, Is.EqualTo("10"));
                Assert.That(orderedRows[3].DisplayName, Is.EqualTo("11"));
            });
        }

        [Test]
        public void TestSortArrayIndicesProperly()
        {
            var rows = new ObservableCollection<DisplayStringViewModel>()
            {
                new("SomeOtherArray[2]"),
                new("Array[1]"),
                new("ThisIsNotAnArray"),
                new("Array[10]"),
                new("SomeOtherArray[1]"),
                new("Array[2]"),
            };

            var orderedRows = rows
                .OrderBy(x => x)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(orderedRows[0].DisplayName, Is.EqualTo("Array[1]"));
                Assert.That(orderedRows[1].DisplayName, Is.EqualTo("Array[2]"));
                Assert.That(orderedRows[2].DisplayName, Is.EqualTo("Array[10]"));
                Assert.That(orderedRows[3].DisplayName, Is.EqualTo("SomeOtherArray[1]"));
                Assert.That(orderedRows[4].DisplayName, Is.EqualTo("SomeOtherArray[2]"));
                Assert.That(orderedRows[5].DisplayName, Is.EqualTo("ThisIsNotAnArray"));
            });
        }

        [Test]
        public void TestSortArrayIndicesDescendingProperly()
        {
            var rows = new ObservableCollection<DisplayStringViewModel>()
            {
                new("SomeOtherArray[2]"),
                new("Array[1]"),
                new("ThisIsNotAnArray"),
                new("Array[10]"),
                new("SomeOtherArray[1]"),
                new("Array[2]"),
            };

            var orderedRows = rows
                .OrderByDescending(x => x)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(orderedRows[0].DisplayName, Is.EqualTo("ThisIsNotAnArray"));
                Assert.That(orderedRows[1].DisplayName, Is.EqualTo("SomeOtherArray[2]"));
                Assert.That(orderedRows[2].DisplayName, Is.EqualTo("SomeOtherArray[1]"));
                Assert.That(orderedRows[3].DisplayName, Is.EqualTo("Array[10]"));
                Assert.That(orderedRows[4].DisplayName, Is.EqualTo("Array[2]"));
                Assert.That(orderedRows[5].DisplayName, Is.EqualTo("Array[1]"));
            });
        }

        [Test]
        public void TestSortBooleanArray()
        {
            var rows = new ObservableCollection<DisplayStringViewModel>()
            {
                new("Testing.1"),
                new("Testing.11"),
                new("Testing.2"),
            };

            var orderedRows = rows
                .OrderBy(x => x)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(orderedRows[0].DisplayName, Is.EqualTo("Testing.1"));
                Assert.That(orderedRows[1].DisplayName, Is.EqualTo("Testing.2"));
                Assert.That(orderedRows[2].DisplayName, Is.EqualTo("Testing.11"));
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

            var orderedRows = rows
                .OrderBy(x => x)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(orderedRows[0].DisplayName, Is.EqualTo("-1"));
                Assert.That(orderedRows[1].DisplayName, Is.EqualTo("-0.123"));
                Assert.That(orderedRows[2].DisplayName, Is.EqualTo("0"));
                Assert.That(orderedRows[3].DisplayName, Is.EqualTo("0"));
                Assert.That(orderedRows[4].DisplayName, Is.EqualTo("0.9"));
                Assert.That(orderedRows[5].DisplayName, Is.EqualTo("1"));
            });
        }

        [Test]
        public void TestName()
        {
            const string testValue = "[ALL]";

            var value = new DisplayStringViewModel(testValue);

            Assert.Multiple(() =>
            {
                Assert.That(value.Name, Is.EqualTo(string.Empty));
                Assert.That(value.DisplayName, Is.EqualTo(testValue));
            });
        }
    }
}
