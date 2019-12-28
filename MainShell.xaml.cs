using GalaSoft.MvvmLight;
using Pulse_Browser.Services;
using System;
using System.Web;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pulse_Browser
{
    /// <summary>
    /// ViewModel for MainShell
    /// </summary>
    public class MainShellViewModel : ViewModelBase
    {
        private bool backButtonEnabled = false;
        private bool forwardButtonEnabled = false;
        private string addressBarText = "";

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

        public string AddressBarText
        {
            get => addressBarText;
            set => Set(() => AddressBarText, ref addressBarText, value);
        }
    }

    /// <summary>
    /// Main app shell
    /// </summary>
    public sealed partial class MainShell : Page
    {
        public static MainShell CurrentInstance;
        public MainShellViewModel ViewModel => DataContext as MainShellViewModel;
        public NavigationService CurrentNavigationService;

        public MainShell()
        {
            CurrentInstance = this;

            InitializeComponent();
            SetupDefaultViewModel();
            DataContextChanged += (s, e) => this.Bindings.Update();

            SetupTheme();
        }

        private void SetupTheme()
        {
            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;

            // Set active window colors
            titleBar.ForegroundColor = titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.BackgroundColor = titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 0, 120, 215);

        }

        private void SetupDefaultViewModel() => DataContext = new MainShellViewModel();

        private void NavigationBar_NavigationQuerySubmitted(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                bool isUri = Uri.TryCreate(query, UriKind.Absolute, out Uri destination)
                    && (destination.Scheme == Uri.UriSchemeHttp || destination.Scheme == Uri.UriSchemeHttps);

                if (isUri)
                {
                    CurrentNavigationService?.Navigate(destination);
                }
                else
                {
                    Uri searchAddress = new Uri($"https://www.google.com/search?q={HttpUtility.UrlEncode(query)}");
                    CurrentNavigationService?.Navigate(searchAddress);
                }
            }
        }

        private void NavigationBar_BackButtonClicked()
        {
            CurrentNavigationService?.Back();
        }

        private void NavigationBar_ForwardButtonClicked()
        {
            CurrentNavigationService?.Forward();
        }

        private void NavigationBar_RefreshButtonClicked()
        {
            CurrentNavigationService?.Refresh();
        }

        private void WebXamlView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var XamlWebViewer = sender as UserControls.WebXamlView;

            CurrentNavigationService = XamlWebViewer.NavigationService;
            CurrentNavigationService.CanGoBackChanged += CurrentNavigationService_CanGoBackChanged;
            CurrentNavigationService.CanGoForwardChanged += CurrentNavigationService_CanGoForwardChanged;
            WebXamlView.WebView.NavigationStarting += WebView_NavigationStarting;
            CurrentNavigationService.NavigationRequested += CurrentNavigationService_NavigationRequested;
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            ViewModel.AddressBarText = args.Uri.ToString();
        }

        private void CurrentNavigationService_NavigationRequested(NavigationEntry navigationEntry)
        {
            switch (navigationEntry.Kind)
            {
                case NavigationPageType.Web:
                    break;
                case NavigationPageType.Native:
                    ViewModel.AddressBarText = null;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CurrentNavigationService_CanGoForwardChanged(bool canGoForward) => ViewModel.ForwardButtonEnabled = canGoForward;

        private void CurrentNavigationService_CanGoBackChanged(bool canGoBack) => ViewModel.BackButtonEnabled = canGoBack;

        private void WebXamlView_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CurrentNavigationService.CanGoBackChanged -= CurrentNavigationService_CanGoBackChanged;
            CurrentNavigationService.CanGoForwardChanged -= CurrentNavigationService_CanGoForwardChanged;
            CurrentNavigationService = null;
        }

        private void NavigationBar_HistoryMenuFlyoutItemButtonClicked()
        {
            Helpers.AppContentDialog.OpenDialog(new UserControls.Options());
        }

        private void NavigationBar_FavoritesMenuItemButtonClicked()
        {
            Helpers.AppContentDialog.OpenDialog(new UserControls.NewBookmarkDialog());
        }

        private void NavigationBar_SettingsMenuFlyoutItemButtonClicked()
        {
            Helpers.AppContentDialog.OpenDialog(new UserControls.Options(UserControls.Options.LandingTab.Settings));
        }

        private void NavigationBar_HomeButtonClicked()
        {
            CurrentNavigationService.Navigate(typeof(Views.BookmarksPage));
        }
    }
}
