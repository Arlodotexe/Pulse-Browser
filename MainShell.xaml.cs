using GalaSoft.MvvmLight;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pulse_Browser
{
    /// <summary>
    /// ViewModel for MainShell
    /// </summary>
    public class MainShellViewModel : ViewModelBase
    {

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
        }


    }
}
