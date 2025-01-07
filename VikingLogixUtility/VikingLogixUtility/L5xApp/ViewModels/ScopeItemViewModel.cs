using VikingLogixUtility.Bases;

namespace VikingLogixUtility.L5xApp.ViewModels
{
    internal sealed class ScopeItemViewModel(string name, int? tagCount = null) : BaseNotifyPropertyChanged
    {
        private bool isSelected = false;
        private string name = name;
        private int? tagCount = tagCount;

        public bool IsSelected
        {
            get => isSelected;

            set
            {
                isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => name;

            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        public int? TagCount
        {
            get => tagCount;

            set
            {
                tagCount = value;
                NotifyPropertyChanged();
            }
        }
    }
}
