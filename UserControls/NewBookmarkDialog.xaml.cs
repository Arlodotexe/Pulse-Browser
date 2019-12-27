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
    public class NewBookmarkDialogViewModel : ViewModelBase
    {
        private string _uri;
        public string Uri
        {
            get => _uri;
            set => Set(() => Uri, ref _uri, value);
        }
    }
    public sealed partial class NewBookmarkDialog : UserControl
    {
        public NewBookmarkDialogViewModel ViewModel => DataContext as NewBookmarkDialogViewModel;
        public NewBookmarkDialog()
        {
            this.InitializeComponent();
            DataContext = new NewBookmarkDialogViewModel();
            DataContextChanged += (s, e) => this.Bindings.Update();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Uri.TryCreate(ViewModel.Uri, UriKind.RelativeOrAbsolute, out Uri newUri)) return;

            // Convert to absolute HTTPS uri if not already absolute
            if (!newUri.IsAbsoluteUri) newUri = new Uri($"https://{newUri}");

            var newBookmark = new Services.Bookmark()
            {
                Uri = newUri,
                Icon = new Windows.UI.Xaml.Media.Imaging.BitmapImage() { UriSource = new Uri($"http://www.google.com/s2/favicons?domain={newUri.Host}") }
            };

            await Services.BookmarksService.AddBookmark(newBookmark);
            Helpers.AppContentDialog.Hide();
        }
    }
}
