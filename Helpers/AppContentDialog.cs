using Pulse_Browser.Services;
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
        public static readonly ContentDialog CurrentDialog = new ContentDialog()
        {
            CloseButtonText = "Close",
            CloseButtonCommand = new GalaSoft.MvvmLight.Command.RelayCommand(() => { Hide(); })
        };

        public static bool IsOpen = false;

        public static async void OpenDialog(FrameworkElement content)
        {
            CurrentDialog.Content = content;

            var PrefferedTheme = SettingsService.GetLocal<ElementTheme>(SettingKeys.PrefferedTheme);
            CurrentDialog.RequestedTheme = PrefferedTheme;

            IsOpen = true;
            await CurrentDialog.ShowAsync();
        }

        public static async Task OpenDialogAsync(FrameworkElement content)
        {
            CurrentDialog.Content = content;

            var PrefferedTheme = SettingsService.GetLocal<ElementTheme>(SettingKeys.PrefferedTheme);
            CurrentDialog.RequestedTheme = PrefferedTheme;

            IsOpen = true;
            await CurrentDialog.ShowAsync();
        }

        public static void Hide()
        {
            CurrentDialog.Hide();
            IsOpen = false;
        }
    }
}
