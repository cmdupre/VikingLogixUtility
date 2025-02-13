using System.Windows;
using VikingLogixUtility.Interfaces;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Processors
{
    internal sealed class TagProcessorPdt : ITagEditable
    {
        public void Read(TagEditorViewModel vm)
        {
            if (vm.PlcInfo is null ||
                vm.ScopeSelectedItem is null)
                return;

            vm.ClearTagEditor();

            var tagNames = vm.PlcInfo.GetTagNamesFor(vm.ScopeSelectedItem.Name);

            vm.ProgressBarValue = 0;
            vm.ProgressBarMaximum = tagNames.Count();
            vm.ProgressBarVisibility = Visibility.Visible;

            foreach (var tagName in tagNames)
            {
                App.Current.Dispatcher.Invoke(() =>
                    ++vm.ProgressBarValue);

                if (vm.Cancel())
                    return;

                var tag = vm.PlcInfo.GetTag(vm.ScopeSelectedItem.Name, tagName, null, null, vm.Logger);

                App.Current.Dispatcher.Invoke(() =>
                    vm.TagEditorGridTable.AddRow(tagName, tag?.Value ?? "[ERROR]"));
            }
        }

        public void Write(TagEditorViewModel vm, string tagName, string columnName, string writeValue)
        {
            if (vm.PlcInfo is null ||
                vm.ScopeSelectedItem is null)
                return;

            vm.PlcInfo.GetTag(vm.ScopeSelectedItem.Name, tagName, null, null, vm.Logger)?
                .Write(writeValue, vm.Logger);
        }
    }
}
