using ClosedXML.Excel;
using Microsoft.Win32;
using VikingLogixUtility.Models;

namespace VikingLogixUtility.Processors
{
    internal static class TagEditorExporter
    {
        public static SaveFileDialog GetSaveFileDialog() => new()
        {
            AddExtension = true,
            CheckPathExists = true,
            Filter = "Excel Files|*.xlsx",
            OverwritePrompt = true,
            Title = "Tag Editor Export"
        };

        public static void Geaux(TagEditorGridTable tagEditorGridTable, string filename)
        {
            var exportTable = tagEditorGridTable.GetExportTable();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet(exportTable, "Tag Editor Export");
            worksheet.Table(0).SetShowColumnStripes(false);
            worksheet.Table(0).SetShowRowStripes(false);
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filename);
        }
    }
}
