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
    public sealed partial class Options : UserControl
    {
        public Options()
        {
            this.InitializeComponent();
        }

        public enum LandingTab
        {
            History, Settings
        }
        public Options(LandingTab landingTab)
        {
            this.InitializeComponent();

            switch (landingTab)
            {
                case LandingTab.History:
                    OptionsPivot.SelectedIndex = 0;
                    break;
                case LandingTab.Settings:
                    OptionsPivot.SelectedIndex = 1;
                    break;
                default:
                    throw new NotImplementedException($"Landing tab not recognized: {landingTab}");
            }
        }
    }
}
