using VikingLogixUtility.L5xApp.Models;

namespace VikingLogixUtilityTests.L5xApp.Models
{
    [TestFixture]
    internal class ExportTableFixture
    {
        [Test]
        public void CreatesExportTable()
        {
            var exportTable = ExportTable.Create();

            Assert.Multiple(() =>
            {
                Assert.That(exportTable, Is.Not.Null);
                Assert.That(exportTable.Table, Is.Not.Null);
                Assert.That(exportTable.Rows, Has.Count.EqualTo(0));
            });
        }

        [Test]
        public void AddsRow()
        {
            var exportTable = ExportTable.Create();

            exportTable.AddRow("1", "2", null, null, "3", "4", null, 5, "6");
            exportTable.AddRow("7", "8", null, null, "9", "0", null, 1, "2");

            Assert.Multiple(() =>
            {
                Assert.That(exportTable, Is.Not.Null);
                Assert.That(exportTable.Table, Is.Not.Null);
                Assert.That(exportTable.Rows, Has.Count.EqualTo(2));
            });
        }
    }
}
