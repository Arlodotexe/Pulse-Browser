using GalaSoft.MvvmLight;
using Pulse_Browser.Services;
using System;
using System.Collections.Generic;
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
    public class WebXamlViewViewModel : ViewModelBase
    {
        private Uri currentAddress = new Uri("about:home");
        private bool webViewShown = false;

        public Uri CurrentWebAddress
        {
            get => currentAddress;
            set => Set(() => this.CurrentWebAddress, ref currentAddress, value);
        }

        public bool WebViewShown
        {
            get => webViewShown;
            set => Set(() => this.WebViewShown, ref webViewShown, value);
        }
    }
    public sealed partial class WebXamlView : UserControl
    {
        private WebXamlViewViewModel ViewModel => DataContext as WebXamlViewViewModel;
        public NavigationService NavigationService = new NavigationService();

        public WebView WebView;
        public Frame Frame;

        private bool _canGoBack = false;
        private bool _canGoForward = false;
        public bool CanGoBack { get => _canGoBack; }
        public bool CanGoForward { get => _canGoForward; }

        public WebXamlView()
        {
            this.InitializeComponent();
            SetupDefaultViewModel();
            SetupNavigationService();

            this.WebView = AppWebView;
            this.Frame = AppFrame;

            DataContextChanged += (s, e) => this.Bindings.Update();

            // Navigate to the default address so it's in the navigation stack
            NavigationService.Navigate(typeof(Views.BookmarksPage));
        }
        private void SetupDefaultViewModel() => DataContext = new WebXamlViewViewModel();

        private void SetupNavigationService()
        {
            NavigationService.NavigationRequested += NavigationService_NavigationRequested;
            NavigationService.RefreshRequested += NavigationService_RefreshRequested;
            NavigationService.BackRequested += NavigationService_BackRequested;
            NavigationService.ForwardRequested += NavigationService_ForwardRequested;
            NavigationService.CanGoForwardChanged += NavigationService_CanGoForwardChanged;
            NavigationService.CanGoBackChanged += NavigationService_CanGoBackChanged;
        }

        private void Navigate(NavigationEntry navigationEntry)
        {
            if (navigationEntry.Kind == NavigationPageType.Web)
            {
                ViewModel.CurrentWebAddress = navigationEntry.WebUri;
                ViewModel.WebViewShown = true;
            }
            else if (navigationEntry.Kind == NavigationPageType.Native)
            {
                ViewModel.WebViewShown = false;
                AppFrame.Navigate(navigationEntry.NativePageType, navigationEntry.NativePageParam);
            }
        }

        public delegate void NavigationStartingEvent(Uri uri);
        public event NavigationStartingEvent NavigationStarting;

        private void NavigationService_CanGoBackChanged(bool canGoBack) => _canGoBack = canGoBack;
        private void NavigationService_CanGoForwardChanged(bool canGoForward) => _canGoForward = canGoForward;
        private void NavigationService_NavigationRequested(NavigationEntry navigationEntry) => Navigate(navigationEntry);

        private void NavigationService_ForwardRequested(NavigationEntry navigationEntry) => Navigate(navigationEntry);

        private void NavigationService_BackRequested(NavigationEntry navigationEntry) => Navigate(navigationEntry);

        private void NavigationService_RefreshRequested() => AppWebView.Refresh();

        private void NavigationService_NavigationRequested(Uri address) => ViewModel.CurrentWebAddress = address;

        private void AppWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (!args.IsSuccess) return;

            // If the current item isn't a duplicate of the new item, push it
            if (NavigationService.HistoryStack.ToList().FirstOrDefault(h => h.Current)?.WebUri != args.Uri)
            {
                NavigationService.AddNewEntry(new NavigationEntry()
                {
                    Kind = NavigationPageType.Web,
                    WebUri = args.Uri,
                    VisitedAt = DateTime.Now,
                    Current = true
                });
            }
        }

        private void AppWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) => NavigationStarting?.Invoke(args.Uri);
    }
}
