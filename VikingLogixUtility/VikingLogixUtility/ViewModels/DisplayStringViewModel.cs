namespace VikingLogixUtility.ViewModels
{
    internal sealed class DisplayStringViewModel(string name) : IComparable<DisplayStringViewModel>
    {
        public string Name => name.Contains('[') && name.Contains(']')
            ? string.Empty
            : name;

        public string DisplayName => name;

        public int CompareTo(DisplayStringViewModel? other)
        {
            if (other is null)
                return 0;

            var value1 = DisplayName;
            var value2 = other.DisplayName;

            // sort arrays by index
            if (value1.Contains('[') && value1.Contains(']') &&
                value2.Contains('[') && value2.Contains(']') &&
                value1.Split('[')[0] == value2.Split('[')[0] &&
                value1.Split(']')[1] == value2.Split(']')[1])
            {
                value1 = value1.Split('[')[1].Split(']')[0];
                value2 = value2.Split('[')[1].Split(']')[0];
            }

            // sort boolean arrays by index
            if (value1.Contains('.') && value2.Contains('.') &&
                value1.Split('.')[0] == value2.Split('.')[0])
            {
                value1 = value1.Split('.')[1];
                value2 = value2.Split('.')[1];
            }

            // sort numbers as such
            if (double.TryParse(value1, out var double0) &&
                double.TryParse(value2, out var double1))
                return CompareDoubles(double0, double1);

            return string.Compare(value1, value2);
        }

        private static int CompareDoubles(double double0, double double1)
        {
            if (double0 < double1) return -1;
            if (double1 < double0) return 1;
            return 0;
        }
    }
}
