using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Pulse_Browser.Services
{
    public class Bookmark
    {
        public Uri Uri { get; set; }
        public BitmapImage Icon { get; set; }
    }

    public static class BookmarksService
    {
        public static async Task<List<Bookmark>> GetBookmarks()
        {
            List<Bookmark> Bookmarks = (await Helpers.Storage.GetLocalClass<IEnumerable<Bookmark>>("Bookmarks"))?.ToList() ?? new List<Bookmark>();

            return Bookmarks;
        }

        public static async void AddBookmark(Bookmark bookmark)
        {
            List<Bookmark> Bookmarks = (await Helpers.Storage.GetLocalClass<IEnumerable<Bookmark>>("Bookmarks"))?.ToList() ?? new List<Bookmark>();

            Bookmarks.Add(bookmark);

            await Helpers.Storage.StoreLocalClass("Bookmarks", Bookmarks);
        }
    }
}
