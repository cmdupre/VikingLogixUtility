using System.Windows;
using VikingLogixUtility.Extensions;
using VikingLogixUtility.Interfaces;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Processors
{
    internal sealed class TagProcessorUdt : ITagEditable
    {
        public void Read(TagEditorViewModel vm)
        {
            if (vm.PlcInfo is null ||
                vm.ScopeSelectedItem is null ||
                vm.UdtSelectedItem is null ||
                vm.ParameterSelectedItems is null ||
                !vm.ParameterSelectedItems.Any())
                return;

            App.Current.Dispatcher.Invoke(() =>
            {
                vm.ClearTagEditor();
                vm.TagEditorGridTable.SetColumns([.. vm.ParameterSelectedItems.Select(p => p.Name)]);
            });

            var tagNames = vm.PlcInfo.GetTagNamesFor(vm.ScopeSelectedItem.Name, vm.UdtSelectedItem.Name, vm.FilterText);

            App.Current.Dispatcher.Invoke(() =>
            {
                vm.ProgressBarValue = 0;

                vm.ProgressBarMaximum =
                    tagNames.Count() *
                    vm.ParameterSelectedItems.Count();

                vm.ProgressBarVisibility = Visibility.Visible;
            });

            foreach (var tagName in tagNames)
            {
                if (vm.Cancel())
                    return;

                var tagValues = new List<string>();

                foreach (var parameterSelectedItem in vm.ParameterSelectedItems)
                {
                    App.Current.Dispatcher.Invoke(() =>
                        ++vm.ProgressBarValue);

                    var tag = vm.PlcInfo.GetTag(
                        vm.ScopeSelectedItem.Name, tagName, vm.UdtSelectedItem.Name, parameterSelectedItem.Name, vm.Logger);

                    tagValues.Add(tag?.Value ?? "[ERROR]");
                }

                App.Current.Dispatcher.Invoke(() =>
                    vm.TagEditorGridTable.AddRow(tagName, [.. tagValues]));
            }
        }

        public void Write(TagEditorViewModel vm, string tagName, string columnName, string writeValue)
        {
            if (vm.PlcInfo is null ||
                vm.ScopeSelectedItem is null ||
                vm.UdtSelectedItem is null)
                return;

            vm.PlcInfo.GetTag(vm.ScopeSelectedItem.Name, tagName, vm.UdtSelectedItem.Name, columnName.Raw(), vm.Logger)?
                .Write(writeValue, vm.Logger);
        }
    }
}
