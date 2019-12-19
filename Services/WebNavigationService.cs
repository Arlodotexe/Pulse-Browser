using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse_Browser.Services
{
    public static class WebNavigationService
    {
        public delegate void OnNavigatedEvent(Uri address);
        public static event OnNavigatedEvent NavigationRequested;

        public delegate void RefreshRequestedEvent();
        public static event RefreshRequestedEvent RefreshRequested;

        public static void Navigate(Uri address)
        {
            NavigationRequested?.Invoke(address);
        }

        public static void Refresh() => RefreshRequested?.Invoke();
    }
}
