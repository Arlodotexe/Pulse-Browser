using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Pulse_Browser.Helpers
{
    public class Converters
    {
        public static Visibility BooleanToVisibility(bool boolean)
        {
            return boolean ? Visibility.Visible : Visibility.Collapsed;
        }

        public static bool IsDefined(object x) => x != null;

        public static bool IsNotDefined(object x) => x is null;

        public static bool CollectionIsEmpty(IEnumerable<object> collection) => !collection.Any();

        public static bool CollectionIsNotEmpty(IEnumerable<object> collection) => collection.Any();
        public static bool CollectionIsEmpty(ObservableCollection<object> collection) => !collection.Any();

        public static bool CollectionIsNotEmpty(ObservableCollection<object> collection) => collection.Any();
    }
}
