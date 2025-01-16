using L5Sharp.Core;
using VikingLogixUtility.L5xApp.Interfaces;
using VikingLogixUtility.L5xApp.ViewModels;

namespace VikingLogixUtility.L5xApp.Processors
{
    internal static class ScopeLoader
    {
        public static void Geaux(ITagExportViewModel viewModel)
        {
            viewModel.Log($"Loading {viewModel.L5XFilename}...");

            var content = L5X.Load(viewModel.L5XFilenameToolTip);

            App.Current.Dispatcher.Invoke(() =>
            {
                viewModel.ScopeListBox.Clear();
                viewModel.ScopeListBox.Add(new ScopeItemViewModel(content.Controller.Name, content.Tags.Count));
            });

            foreach (var program in content.Programs)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.ScopeListBox.Add(new ScopeItemViewModel(program.Name, program.Tags.Count));
                });
            }

            viewModel.Log("Done.");
        }
    }
}
