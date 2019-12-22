using GalaSoft.MvvmLight;
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
        private bool backButtonEnabled = false;
        private bool forwardButtonEnabled = false;

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
            SetupDefaultViewModel();
            DataContextChanged += (s, e) => this.Bindings.Update();

            SetupWebNavigationEvents();
        }

        public void SetupWebNavigationEvents()
        {
            Services.NavigationService.CanGoForwardChanged += WebNavigationService_CanGoForwardChanged;
            Services.NavigationService.CanGoBackChanged += WebNavigationService_CanGoBackChanged;
        }

        private void WebNavigationService_CanGoBackChanged(bool canGoBack) => ViewModel.BackButtonEnabled = canGoBack;
        private void WebNavigationService_CanGoForwardChanged(bool canGoForward) => ViewModel.ForwardButtonEnabled = canGoForward;

        private void SetupDefaultViewModel() => DataContext = new MainShellViewModel();

    }
}
