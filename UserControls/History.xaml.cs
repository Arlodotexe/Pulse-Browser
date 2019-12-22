using GalaSoft.MvvmLight;
using Pulse_Browser.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Pulse_Browser.UserControls
{
    public class HistoryViewerViewModel : ViewModelBase
    {
        public ObservableCollection<NavigationEntry> History = new ObservableCollection<NavigationEntry>();
    }

    public sealed partial class History : UserControl
    {
        public History()
        {
            this.InitializeComponent();
            DataContextChanged += (s, e) => this.Bindings.Update();
            PopulateHistoryView();
        }

        private void PopulateHistoryView()
        {
            // TODO: Instead of showing history from the current session and the current view, get saved entries from persistent storage
            ViewModel.History = new ObservableCollection<NavigationEntry>(MainShell.CurrentInstance.CurrentNavigationService.HistoryStack.Where(h => h.Kind == NavigationPageType.Web));
        }

        public HistoryViewerViewModel ViewModel => DataContext as HistoryViewerViewModel;
    }
}
