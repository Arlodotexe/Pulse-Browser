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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Pulse_Browser.UserControls
{
    public sealed partial class Settings : UserControl
    {
        public Settings()
        {
            this.InitializeComponent();
            PopulateTheme();
        }


        #region Theme selection handlers
        private void SystemMode_Selected(object sender, RoutedEventArgs e)
        {
            SettingsService.SetLocal(SettingKeys.PrefferedTheme, ElementTheme.Default);
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = ElementTheme.Default;
            }
        }

        private void DarkMode_Selected(object sender, RoutedEventArgs e)
        {
            SettingsService.SetLocal(SettingKeys.PrefferedTheme, ElementTheme.Dark);
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = ElementTheme.Dark;
            }
        }

        private void LightMode_Selected(object sender, RoutedEventArgs e)
        {
            SettingsService.SetLocal(SettingKeys.PrefferedTheme, ElementTheme.Light);
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = ElementTheme.Light;
            }
        }
        #endregion

        private void PopulateTheme()
        {
            ElementTheme PrefferedTheme = SettingsService.GetLocal<ElementTheme>(SettingKeys.PrefferedTheme);
            switch (PrefferedTheme)
            {
                case ElementTheme.Default:
                    SystemThemeRadioButton.IsChecked = true;
                    break;
                case ElementTheme.Dark:
                    DarkThemeRadioButton.IsChecked = true;
                    break;
                case ElementTheme.Light:
                    LightThemeRadioButton.IsChecked = true;
                    break;
                default:
                    break;
            }
        }
    }
}
