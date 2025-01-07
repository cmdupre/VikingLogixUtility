using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace VikingLogixUtility.Models
{
    /// <summary>
    /// Credit to https://paulstovell.com/dynamic-datagrid/.
    /// </summary>
    /// <param name="templateName"></param>
    internal sealed class CustomBoundColumn(string templateName) : DataGridBoundColumn
    {
        public string TemplateName => templateName;

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return GenerateElement(cell, dataItem);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var binding = new Binding(((Binding)Binding).Path.Path)
            {
                Source = dataItem
            };

            var content = new ContentControl
            {
                ContentTemplate = (DataTemplate)cell.FindResource(TemplateName)
            };

            content.SetBinding(ContentControl.ContentProperty, binding);

            return content;
        }
    }
}
