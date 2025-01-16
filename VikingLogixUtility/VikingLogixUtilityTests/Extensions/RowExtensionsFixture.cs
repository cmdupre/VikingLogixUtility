using System.Collections.ObjectModel;
using VikingLogixUtility.Extensions;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtilityTests.Extensions
{
    [TestFixture]
    internal class RowExtensionsFixture
    {
        [Test]
        public void TestGetComparerWithListInAscendingOrder()
        {
            var rows = new ObservableCollection<RowViewModel>()
            {
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "A", string.Empty, true),
                    new("Value", "1", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "D", string.Empty, true),
                    new("Value", "4", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "B", string.Empty, true),
                    new("Value", "2", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "C", string.Empty, true),
                    new("Value", "3", string.Empty, true),
                }),
            };

            rows = rows.Sort(0);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("D"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("C"));
                Assert.That(rows[0].Properties[1].ReadValue.ToString(), Is.EqualTo("4"));
                Assert.That(rows[1].Properties[1].ReadValue.ToString(), Is.EqualTo("3"));
            });

            rows = rows.Sort(0);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("A"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("B"));
                Assert.That(rows[0].Properties[1].ReadValue.ToString(), Is.EqualTo("1"));
                Assert.That(rows[1].Properties[1].ReadValue.ToString(), Is.EqualTo("2"));
            });

            rows = rows.Sort(0);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("D"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("C"));
                Assert.That(rows[0].Properties[1].ReadValue.ToString(), Is.EqualTo("4"));
                Assert.That(rows[1].Properties[1].ReadValue.ToString(), Is.EqualTo("3"));
            });
        }

        [Test]
        public void TestGetComparerWithListInDescendingOrder()
        {
            var rows = new ObservableCollection<RowViewModel>()
            {
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "D", string.Empty, true),
                    new("Value", "4", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "A", string.Empty, true),
                    new("Value", "1", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "B", string.Empty, true),
                    new("Value", "2", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "C", string.Empty, true),
                    new("Value", "3", string.Empty, true),
                }),
            };

            rows = rows.Sort(0);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("A"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("B"));
                Assert.That(rows[0].Properties[1].ReadValue.ToString(), Is.EqualTo("1"));
                Assert.That(rows[1].Properties[1].ReadValue.ToString(), Is.EqualTo("2"));
            });

            rows = rows.Sort(0);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("D"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("C"));
                Assert.That(rows[0].Properties[1].ReadValue.ToString(), Is.EqualTo("4"));
                Assert.That(rows[1].Properties[1].ReadValue.ToString(), Is.EqualTo("3"));
            });

            rows = rows.Sort(0);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("A"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("B"));
                Assert.That(rows[0].Properties[1].ReadValue.ToString(), Is.EqualTo("1"));
                Assert.That(rows[1].Properties[1].ReadValue.ToString(), Is.EqualTo("2"));
            });
        }

        [Test]
        public void TestGetComparerWithNumbers()
        {
            var rows = new ObservableCollection<RowViewModel>()
            {
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "A", string.Empty, true),
                    new("Value", "10", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "B", string.Empty, true),
                    new("Value", "2", string.Empty, true),
                }),
            };

            rows = rows.Sort(1);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("B"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("A"));
            });

            rows = rows.Sort(1);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("A"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("B"));
            });
        }

        [Test]
        public void TestGetComparerForceAscending()
        {
            var rows = new ObservableCollection<RowViewModel>()
            {
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "B", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "A", string.Empty, true),
                }),
            };

            rows = rows.Sort(0, true);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("A"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("B"));
            });

            rows = rows.Sort(0, true);

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("A"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("B"));
            });
        }

        [Test]
        public void TestSortsNegativeValuesProperly()
        {
            var rows = new ObservableCollection<RowViewModel>()
            {
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "1", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "0", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "0.9", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "-1", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "-0.123", string.Empty, true),
                }),
                new(new List<CellViewModel>()
                {
                    new("Tag-Name", "0", string.Empty, true),
                }),
            };

            rows = rows.Sort(0, true);

            var testing = rows.Select(x => x.Properties[0].ReadValue.ToString()).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(rows[0].Properties[0].ReadValue.ToString(), Is.EqualTo("-1"));
                Assert.That(rows[1].Properties[0].ReadValue.ToString(), Is.EqualTo("-0.123"));
                Assert.That(rows[2].Properties[0].ReadValue.ToString(), Is.EqualTo("0"));
                Assert.That(rows[3].Properties[0].ReadValue.ToString(), Is.EqualTo("0"));
                Assert.That(rows[4].Properties[0].ReadValue.ToString(), Is.EqualTo("0.9"));
                Assert.That(rows[5].Properties[0].ReadValue.ToString(), Is.EqualTo("1"));
            });
        }
    }
}
