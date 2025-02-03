using VikingLogixUtility.Models;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Interfaces
{
    internal interface IExportable
    {
        bool IsRunning { get; }
        DisplayStringViewModel? ScopeSelectedItem { get; }
        TagEditorGridTable TagEditorGridTable { get; }
        void ExportTags();
    }
}
