using VikingLogixUtility.Bases;
using VikingLogixUtility.L5xApp.Interfaces;
using VikingLogixUtility.L5xApp.Processors;

namespace VikingLogixUtility.L5xApp.Commands
{
    internal sealed class ExportCommand(ITagExportViewModel viewModel) : BaseCommand
    {
        public override bool CanExecute(object? parameter)
        {
            return
                !viewModel.IsRunning &&
                !string.IsNullOrEmpty(viewModel.L5XFilename);
        }

        public override async void Execute(object? parameter)
        {
            viewModel.IsRunning = true;

            await Task.Run(() =>
            {
                try
                {
                    Exporter.Geaux(viewModel);
                }
                catch (Exception ex)
                {
                    viewModel.Log(string.Empty);
                    viewModel.Log($"Unhandled Exception: {ex.Message}");
                    viewModel.Log($"Process aborted.");
                }
            });

            viewModel.IsRunning = false;
        }
    }
}
