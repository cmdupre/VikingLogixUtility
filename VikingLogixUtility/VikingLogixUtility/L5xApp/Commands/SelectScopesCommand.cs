using VikingLogixUtility.Bases;
using VikingLogixUtility.L5xApp.ViewModels;

namespace VikingLogixUtility.L5xApp.Commands
{
    internal class SelectScopesCommand(L5xTagExportViewModel viewModel) : BaseCommand
    {
        public override bool CanExecute(object? parameter)
        {
            return
                !viewModel.IsRunning &&
                !string.IsNullOrEmpty(viewModel.L5XFilename);
        }

        public override void Execute(object? parameter)
        {
            var parm = parameter as string;

            foreach (var scope in viewModel.ScopeListBox)
            {
                scope.IsSelected = parm?.CompareTo("ALL") == 0;
            }
        }
    }
}
