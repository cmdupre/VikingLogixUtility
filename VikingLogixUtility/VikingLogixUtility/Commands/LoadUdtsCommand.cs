using VikingLogixUtility.Bases;
using VikingLogixUtility.Interfaces;

namespace VikingLogixUtility.Commands
{
    internal sealed class LoadUdtsCommand(IUdtTabViewModel viewModel) : BaseCommand
    {
        public override bool CanExecute(object? parameter)
        {
            return !viewModel.IsRunning;
        }

        public override void Execute(object? parameter)
        {
            if (!CanExecute(null))
                return;

            viewModel.LoadUdts();
        }
    }
}
