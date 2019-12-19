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

            Services.NavigationService.NavigationRequested += NavigationService_NavigationRequested;
        }

        private void SetupDefaultViewModel()
        {
            DataContext = new MainShellViewModel();
        }

        private void NavigationService_NavigationRequested(Uri address)
        {
            ViewModel.WebViewShown = true;
            ViewModel.CurrentAddress = address;
        }

        private void AppFrame_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame fromFrame = sender as Frame;

            fromFrame.Navigate(typeof(Views.HomePage));
        }
    }
}
