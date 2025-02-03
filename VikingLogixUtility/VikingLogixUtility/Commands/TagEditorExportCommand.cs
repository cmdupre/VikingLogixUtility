using VikingLogixUtility.Bases;
using VikingLogixUtility.Common;
using VikingLogixUtility.Interfaces;

namespace VikingLogixUtility.Commands
{
    internal sealed class TagEditorExportCommand(IExportable viewModel) : BaseCommand
    {
        public override bool CanExecute(object? parameter)
        {
            return
                !viewModel.IsRunning &&
                viewModel.ScopeSelectedItem?.Name != Constants.All &&
                (viewModel.TagEditorGridTable?.VisibleRows.Any() ?? false);
        }

        public override void Execute(object? parameter)
        {
            viewModel.ExportTags();
        }
    }
}
