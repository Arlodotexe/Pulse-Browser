using GalaSoft.MvvmLight;
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

        public Uri CurrentAddress
        {
            get => currentAddress;
            set => Set(() => this.CurrentAddress, ref currentAddress, value);
        }

        public bool WebViewShown
        {
            get => webViewShown;
            set => Set(() => this.WebViewShown, ref webViewShown, value);
        }
    }
    public sealed partial class WebXamlView : UserControl
    {
        public WebXamlViewViewModel ViewModel => DataContext as WebXamlViewViewModel;

        public WebXamlView()
        {
            this.InitializeComponent();
            SetupDefaultViewModel();
            SetupWebNavigationEvents();

            DataContextChanged += (s, e) => this.Bindings.Update();

            // Navigate to the default address so it's in the navigation stack
            Services.WebNavigationService.Navigate(ViewModel.CurrentAddress);

            AppWebView.NavigationStarting += AppWebView_NavigationStarting;
            AppWebView.NavigationFailed += AppWebView_NavigationFailed;
        }
        private void SetupDefaultViewModel() => DataContext = new WebXamlViewViewModel();

        public void SetupWebNavigationEvents()
        {
            Services.WebNavigationService.NavigationRequested += NavigationService_NavigationRequested;
            Services.WebNavigationService.RefreshRequested += WebNavigationService_RefreshRequested;
            Services.WebNavigationService.BackRequested += WebNavigationService_BackRequested;
            Services.WebNavigationService.ForwardRequested += WebNavigationService_ForwardRequested;
        }

        private void AppWebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e) => InterceptHomePage(e.Uri);

        private void AppWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) => InterceptHomePage(args.Uri);

        /// <summary>
        /// Checks Uri for home page and sets up the ViewModel appropriately
        /// </summary>
        /// <param name="uri">Uri to check</param>
        /// <returns>True if found, otherwise false</returns>
        private bool InterceptHomePage(Uri uri)
        {
            switch (uri?.ToString())
            {
                case "about:home":
                    ViewModel.WebViewShown = false;
                    AppFrame.Navigate(typeof(Views.HomePage));
                    return true;
                default:
                    ViewModel.WebViewShown = true;
                    return false;
            }
        }


        private void WebNavigationService_ForwardRequested(Uri address)
        {
            if (InterceptHomePage(address)) return;
            ViewModel.CurrentAddress = address;
        }

        private void WebNavigationService_BackRequested(Uri address)
        {
            if (InterceptHomePage(address)) return;
            ViewModel.CurrentAddress = address;
        }

        private void WebNavigationService_RefreshRequested()
        {
            if (ViewModel.WebViewShown) AppWebView.Refresh();
        }
        private void NavigationService_NavigationRequested(Uri address)
        {
            ViewModel.CurrentAddress = address;
        }
    }
}
