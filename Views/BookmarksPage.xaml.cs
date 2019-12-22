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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pulse_Browser.Views
{
    public class BookmarksPageViewModel : ViewModelBase
    {
        private List<Bookmark> _bookmarks;

        public List<Bookmark> Bookmarks
        {
            get => _bookmarks;
            set => Set(() => Bookmarks, ref _bookmarks, Bookmarks);
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
            DataContextChanged += (s, e) => this.Bindings.Update();
        }


    }
}
