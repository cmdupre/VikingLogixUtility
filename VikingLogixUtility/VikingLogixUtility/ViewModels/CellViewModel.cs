using System.Windows;
using VikingLogixUtility.Bases;

namespace VikingLogixUtility.ViewModels
{
    internal sealed class CellViewModel(string name, string readValue, string writeValue, bool readOnly = false) : BaseNotifyPropertyChanged
    {
        public string Name
        {
            get => name;

            private set => name = value;
        }

        public string ReadValue
        {
            get => readValue;

            set
            {
                readValue = value;
                NotifyPropertyChanged();
            }
        }

        public string WriteValue
        {
            get => writeValue;

            set
            {
                writeValue = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility Visible => readOnly
            ? Visibility.Hidden
            : Visibility.Visible;

        public bool Span => Visible == Visibility.Hidden;
    }
}
