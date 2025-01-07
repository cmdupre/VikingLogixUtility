using VikingLogixUtility.Bases;
using VikingLogixUtility.Interfaces;
using VikingLogixUtility.Models;

namespace VikingLogixUtility.Commands
{
    internal sealed class ReadCommand(IEditorViewModel viewModel) : BaseCommand
    {
        public override bool CanExecute(object? parameter)
        {
            return
                !viewModel.IsRunning &&
                !string.IsNullOrWhiteSpace(viewModel.Address);
        }

        public override void Execute(object? parameter)
        {
            if (viewModel.ScopeItems.Count < 1)
            {
                var settings = new Settings(viewModel.Address);
                
                settings.Write();

                viewModel.LoadScopes();

                return;
            }

            viewModel.LoadTagEditor();
        }
    }
}
