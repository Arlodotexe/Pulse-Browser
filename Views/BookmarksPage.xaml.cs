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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pulse_Browser.Views
{
    public class BookmarksPageViewModel : ViewModelBase
    {
        private List<Bookmark> _bookmarks;

        public List<Bookmark> Bookmarks
        {
            get => _bookmarks;
            set => Set(() => Bookmarks, ref _bookmarks, value);
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookmarksPage : Page
    {
        public BookmarksPageViewModel ViewModel => DataContext as BookmarksPageViewModel;
        public BookmarksPage()
        {
            this.InitializeComponent();
            DataContext = new BookmarksPageViewModel();
            DataContextChanged += (s, e) => this.Bindings.Update();

            PopulateDesignModeBookmarks();

        }

        private void PopulateDesignModeBookmarks()
        {
            ViewModel.Bookmarks = new List<Bookmark>()
            {
                new Bookmark()
                {
                     Uri = new Uri("https://google.com/"),
                     Icon = new BitmapImage() {UriSource = new Uri($"http://www.google.com/s2/favicons?domain=google.com")},
                },
                new Bookmark()
                {
                     Uri = new Uri("https://linkedin.com/"),
                     Icon = new BitmapImage() {UriSource = new Uri($"http://www.google.com/s2/favicons?domain=linkedin.com")},
                },
                new Bookmark()
                {
                     Uri = new Uri("https://youtube.com/"),
                     Icon = new BitmapImage() {UriSource = new Uri($"http://www.google.com/s2/favicons?domain=youtube.com")},
                },
                new Bookmark()
                {
                     Uri = new Uri("https://facebook.com/"),
                     Icon = new BitmapImage() {UriSource = new Uri($"http://www.google.com/s2/favicons?domain=facebook.com")},
                },
                new Bookmark()
                {
                     Uri = new Uri("https://facebook.com/"),
                     Icon = new BitmapImage() {UriSource = new Uri($"http://www.google.com/s2/favicons?domain=facebook.com")},
                },
                new Bookmark()
                {
                     Uri = new Uri("https://google.com/"),
                     Icon = new BitmapImage() {UriSource = new Uri($"http://www.google.com/s2/favicons?domain=google.com")},
                },
            };
        }
    }
}
