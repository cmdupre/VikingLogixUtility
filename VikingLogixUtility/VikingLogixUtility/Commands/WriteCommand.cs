using System.Windows;
using VikingLogixUtility.Bases;
using VikingLogixUtility.Common;
using VikingLogixUtility.Interfaces;

namespace VikingLogixUtility.Commands
{
    internal sealed class WriteCommand(IWriteable viewModel) : BaseCommand
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
            var result = MessageBox.Show(
                "Are you sure?",
                "Write Tags",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
            {
                MessageBox.Show("Operation cancelled.", "Write Tags", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            viewModel.WriteTags();
        }
    }
}
