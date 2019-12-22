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
    public class WebHistoryEntry : NavigationEntry
    {
        public WebHistoryEntry() { }
        public WebHistoryEntry(NavigationEntry navigationEntry)
        {
            Current = navigationEntry.Current;
            Kind = navigationEntry.Kind;
            VisitedAt = navigationEntry.VisitedAt;
            WebUri = navigationEntry.WebUri;
            NativePageParam = navigationEntry.NativePageParam;
            NativePageType = navigationEntry.NativePageType;
        }

        public Uri Favicon { get; set; }
    }
    public class HistoryViewerViewModel : ViewModelBase
    {
        public ObservableCollection<WebHistoryEntry> History = new ObservableCollection<WebHistoryEntry>();
    }

    public sealed partial class History : UserControl
    {
        public History()
        {
            this.InitializeComponent();
            DataContextChanged += (s, e) => this.Bindings.Update();
            DataContext = new HistoryViewerViewModel();

            PopulateHistoryView();
        }

        private void PopulateHistoryView()
        {
            if (ViewModel.IsInDesignMode)
            {
                ViewModel.History = new ObservableCollection<WebHistoryEntry>(new List<WebHistoryEntry>() {
                    new WebHistoryEntry()
                    {
                        WebUri = new Uri("https://google.com/"),
                        VisitedAt = DateTime.Now,
                        Current = true
                    }
                });
                return;
            }

            // TODO: Instead of showing history from the current session and the current view, get saved entries from persistent storage
            var History = MainShell.CurrentInstance.CurrentNavigationService.HistoryStack;

            if (History is null) return;

            // Convert full History class to WebHistory
            // Filters out native navigation pages and add favicon
            var WebHistory = History.Where(h => h.Kind == NavigationPageType.Web).Select(h => new WebHistoryEntry(h) { Favicon = new Uri($"http://www.google.com/s2/favicons?domain={h.WebUri.Host}") });

            if (WebHistory != null) ViewModel.History = new ObservableCollection<WebHistoryEntry>(WebHistory);
        }

        public HistoryViewerViewModel ViewModel => DataContext as HistoryViewerViewModel;
    }
}
