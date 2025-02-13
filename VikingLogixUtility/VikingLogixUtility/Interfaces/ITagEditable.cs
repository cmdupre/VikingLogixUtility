using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Interfaces
{
    internal interface ITagEditable
    {
        public void Read(TagEditorViewModel vm);
        public void Write(TagEditorViewModel vm, string tagName, string columnName, string writeValue);
    }
}
