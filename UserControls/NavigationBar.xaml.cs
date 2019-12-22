using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
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
    public sealed partial class NavigationBar : UserControl
    {
        public NavigationBar()
        {
            this.InitializeComponent();
        }

        public bool BackButtonEnabled
        {
            get { return (bool)GetValue(BackButtonEnabledProperty); }
            set { SetValue(BackButtonEnabledProperty, value); }
        }

        private DependencyProperty BackButtonEnabledProperty = DependencyProperty.Register(
          "BackButtonEnabled",
          typeof(bool),
          typeof(bool),
          new PropertyMetadata(null)
        );

        public bool ForwardButtonEnabled
        {
            get { return (bool)GetValue(ForwardButtonEnabledProperty); }
            set { SetValue(ForwardButtonEnabledProperty, value); }
        }

        private DependencyProperty ForwardButtonEnabledProperty = DependencyProperty.Register(
          "ForwardButtonEnabled",
          typeof(bool),
          typeof(bool),
          new PropertyMetadata(null)
        );

        public event NavigationQuerySubmittedHandler NavigationQuerySubmitted;
        public delegate void NavigationQuerySubmittedHandler(string query);

        public event HomeButtonClickedHandler HomeButtonClicked;
        public delegate void HomeButtonClickedHandler();

        public event RefreshButtonClickedHandler RefreshButtonClicked;
        public delegate void RefreshButtonClickedHandler();

        public event BackButtonClickedHandler BackButtonClicked;
        public delegate void BackButtonClickedHandler();

        public event ForwardButtonClickedHandler ForwardButtonClicked;
        public delegate void ForwardButtonClickedHandler();

        public event SettingsMenuFlyoutItemClickedHandler SettingsMenuFlyoutItemButtonClicked;
        public delegate void SettingsMenuFlyoutItemClickedHandler();

        public event HistoryMenuFlyoutItemClickedHandler HistoryMenuFlyoutItemButtonClicked;
        public delegate void HistoryMenuFlyoutItemClickedHandler();

        public event FavoritesMenuFlyoutItemClickedHandler FavoritesMenuItemButtonClicked;
        public delegate void FavoritesMenuFlyoutItemClickedHandler();

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) => NavigationQuerySubmitted?.Invoke(args.QueryText);

        private void HomeButton_Click(object sender, RoutedEventArgs e) => HomeButtonClicked?.Invoke();

        private void SettingsMenuFlyoutItem_Click(object sender, RoutedEventArgs e) => SettingsMenuFlyoutItemButtonClicked?.Invoke();
        private void HistoryMenuFlyoutItem_Click(object sender, RoutedEventArgs e) => HistoryMenuFlyoutItemButtonClicked?.Invoke();

        private void RefreshButton_Click(object sender, RoutedEventArgs e) => RefreshButtonClicked?.Invoke();

        private void BackButton_Click(object sender, RoutedEventArgs e) => BackButtonClicked?.Invoke();

        private void ForwardButton_Click(object sender, RoutedEventArgs e) => ForwardButtonClicked?.Invoke();

        private void FavoritesButton_Click(object sender, RoutedEventArgs e) => FavoritesMenuItemButtonClicked?.Invoke();

        private void MoreMenuButton_Click(object sender, RoutedEventArgs e)
        {
            MoreMenuFlyout.ShowAt(sender as FrameworkElement, new FlyoutShowOptions() { Placement = FlyoutPlacementMode.BottomEdgeAlignedRight });
        }
    }
}
