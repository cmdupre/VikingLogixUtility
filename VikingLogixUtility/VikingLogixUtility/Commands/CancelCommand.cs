using VikingLogixUtility.Bases;
using VikingLogixUtility.Interfaces;

namespace VikingLogixUtility.Commands
{
    internal sealed class CancelCommand(IEditorViewModel viewModel) : BaseCommand
    {
        public override bool CanExecute(object? parameter)
        {
            return viewModel.IsRunning;
        }

        public override void Execute(object? parameter)
        {
            viewModel.CancelRequested = true;
        }
    }
}
