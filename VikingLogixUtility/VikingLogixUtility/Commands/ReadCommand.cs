using VikingLogixUtility.Bases;
using VikingLogixUtility.Interfaces;
using VikingLogixUtility.Models;
using VikingLogixUtility.RegEx;

namespace VikingLogixUtility.Commands
{
    internal sealed class ReadCommand(ITagEditorLoadable viewModel) : BaseCommand
    {
        public override bool CanExecute(object? parameter)
        {
            return
                !viewModel.IsRunning &&
                RegularExpressions.MappingFilter().IsMatch(viewModel.Address);
        }

        public override void Execute(object? parameter)
        {
            if (viewModel.ScopeItems.Count < 1)
            {
                var settings = new Settings(viewModel.Address, viewModel.Slot);

                settings.Write();

                viewModel.LoadScopes();

                return;
            }

            viewModel.LoadTagEditor();
        }
    }
}
