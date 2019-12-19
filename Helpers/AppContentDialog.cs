using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Pulse_Browser.Helpers
{
    /// <summary>
    /// A central Content Dialog helper to help manage dialogs
    /// </summary>
    public static class AppContentDialog
    {
        private static readonly ContentDialog dialog = new ContentDialog();

        public static bool IsOpen = false;

        public static async void OpenDialog(FrameworkElement content)
        {
            dialog.Content = content;
            if (!IsOpen)
            {
                await dialog.ShowAsync();
            }
        }

        public static async Task OpenDialogAsync(FrameworkElement content)
        {
            dialog.Content = content;
            if (!IsOpen)
            {
                await dialog.ShowAsync();
            }
        }

        public static void Hide()
        {
            dialog.Hide();
        }
    }
}
