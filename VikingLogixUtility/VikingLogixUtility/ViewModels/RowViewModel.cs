using System.Collections.ObjectModel;

namespace VikingLogixUtility.ViewModels
{
    internal sealed class RowViewModel
    {
        private readonly ObservableCollection<CellViewModel> cells = [];

        public RowViewModel(params CellViewModel[] cells)
        {
            foreach (var property in cells)
                Properties.Add(property);
        }

        public RowViewModel(IEnumerable<CellViewModel> cells) : this([.. cells])
        {
            // NOP.
        }

        public ObservableCollection<CellViewModel> Properties => cells;
    }
}
