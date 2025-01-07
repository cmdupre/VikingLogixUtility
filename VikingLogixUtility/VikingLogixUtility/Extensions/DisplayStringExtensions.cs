using System.Collections.ObjectModel;
using VikingLogixUtility.Models;
using VikingLogixUtility.ViewModels;

namespace VikingLogixUtility.Extensions
{
    internal static class DisplayStringExtensions
    {
        public static ObservableCollection<DisplayStringViewModel> Sort(this ObservableCollection<DisplayStringViewModel> items)
        {
            var itemsAsList = items.ToList();

            itemsAsList.Sort(new DisplayStringComparer());

            return new ObservableCollection<DisplayStringViewModel>(itemsAsList);
        }
    }
}
