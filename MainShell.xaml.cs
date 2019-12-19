﻿using GalaSoft.MvvmLight;
using System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pulse_Browser
{
    /// <summary>
    /// ViewModel for MainShell
    /// </summary>
    public class MainShellViewModel : ViewModelBase
    {
        private Uri currentAddress = new Uri("about:home");
        private bool webViewShown = false;
        private bool backButtonEnabled = false;
        private bool forwardButtonEnabled = false;

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

        public bool BackButtonEnabled
        {
            get => backButtonEnabled;
            set => Set(() => BackButtonEnabled, ref backButtonEnabled, value);
        }

        public bool ForwardButtonEnabled
        {
            get => forwardButtonEnabled;
            set => Set(() => ForwardButtonEnabled, ref forwardButtonEnabled, value);
        }
    }

    /// <summary>
    /// Main app shell
    /// </summary>
    public sealed partial class MainShell : Page
    {
        public MainShellViewModel ViewModel => DataContext as MainShellViewModel;

        public MainShell()
        {
            InitializeComponent();
            DataContextChanged += (s, e) => this.Bindings.Update();

            SetupDefaultViewModel();
            SetupWebNavigationEvents();

            AppWebView.NavigationStarting += AppWebView_NavigationStarting;
            AppWebView.NavigationFailed += AppWebView_NavigationFailed;
            AppWebView.NavigationCompleted += AppWebView_NavigationCompleted;
        }

        private void AppWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (!(sender is WebView webView)) return;
            ViewModel.BackButtonEnabled = webView.CanGoBack;
            ViewModel.ForwardButtonEnabled = webView.CanGoForward;
        }

        private void AppWebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e) => InterceptHomePage(e.Uri);

        private void AppWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) => InterceptHomePage(args.Uri);

        private void InterceptHomePage(Uri uri)
        {
            switch (uri?.ToString())
            {
                case "about:home":
                    ViewModel.WebViewShown = false;
                    AppFrame.Navigate(typeof(Views.HomePage));
                    break;
                default:
                    ViewModel.WebViewShown = true;
                    break;
            }
        }

        public void SetupWebNavigationEvents()
        {
            Services.WebNavigationService.NavigationRequested += NavigationService_NavigationRequested;
            Services.WebNavigationService.RefreshRequested += WebNavigationService_RefreshRequested;
            Services.WebNavigationService.BackRequested += WebNavigationService_BackRequested;
            Services.WebNavigationService.ForwardRequested += WebNavigationService_ForwardRequested;
        }

        private void WebNavigationService_ForwardRequested()
        {
            if (AppWebView.CanGoForward) AppWebView.GoForward();
        }

        private void WebNavigationService_BackRequested()
        {
            if (AppWebView.CanGoBack) AppWebView.GoBack();
        }

        private void WebNavigationService_RefreshRequested()
        {
            if (ViewModel.WebViewShown) AppWebView.Refresh();
        }

        private void SetupDefaultViewModel() => DataContext = new MainShellViewModel();

        private void NavigationService_NavigationRequested(Uri address)
        {
            ViewModel.CurrentAddress = address;
        }
    }
}
