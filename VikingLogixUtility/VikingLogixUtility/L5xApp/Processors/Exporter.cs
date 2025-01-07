using ClosedXML.Excel;
using L5Sharp.Core;
using System.Data;
using VikingLogixUtility.Common;
using VikingLogixUtility.Extensions;
using VikingLogixUtility.L5xApp.ViewModels;
using VikingLogixUtility.L5xApp.Models;

namespace VikingLogixUtility.L5xApp.Processors
{
    internal static class Exporter
    {
        public static void Geaux(L5xTagExportViewModel viewModel)
        {
            viewModel.Log($"Exporting to {viewModel.L5XFilename.WithXlsxExtension()}...");

            var selectedScopes = viewModel.ScopeListBox
                .Where(i => i.IsSelected)
                .Select(i => i.Name);

            var content = L5X.Load(viewModel.L5XFilenameToolTip);

            var mapping = Helper.GetMappingRungs(content);

            var exportTable = ExportTable.Create();

            foreach (var scope in selectedScopes)
            {
                var tags = scope == content.Controller.Name
                    ? content.Tags
                    : content.Programs.Where(p => p.Name == scope).Single().Tags;

                TagWriter.Geaux(viewModel, exportTable, mapping, scope, tags);
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet(exportTable.Table, "Tag Export");
                worksheet.Table(0).SetShowColumnStripes(false);
                worksheet.Table(0).SetShowRowStripes(false);
                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(viewModel.L5XFilenameToolTip.WithXlsxExtension());
            }

            viewModel.Log("Done.");
        }
    }
}
