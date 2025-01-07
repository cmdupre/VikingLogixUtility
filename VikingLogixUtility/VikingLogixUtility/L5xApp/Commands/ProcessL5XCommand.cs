using Microsoft.Win32;
using VikingLogixUtility.Bases;
using VikingLogixUtility.L5xApp.Processors;
using VikingLogixUtility.L5xApp.ViewModels;

namespace VikingLogixUtility.L5xApp.Commands
{
    internal sealed class ProcessL5XCommand(L5xTagExportViewModel viewModel) : BaseCommand
    {
        public override bool CanExecute(object? parameter)
        {
            return !viewModel.IsRunning;
        }

        public override async void Execute(object? parameter)
        {
            var ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Title = "Select L5X file to load...",
                Filter = "L5X Files|*.L5X"
            };

            if (ofd.ShowDialog() ?? false)
            {
                viewModel.L5XFilename = ofd.FileName;
                viewModel.IsRunning = true;

                await Task.Run(() =>
                {
                    try
                    {
                        ScopeLoader.Geaux(viewModel);
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
}
