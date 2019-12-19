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

        public delegate void BackRequestedEvent();
        public static event BackRequestedEvent BackRequested;

        public delegate void ForwardRequestedEvent();
        public static event ForwardRequestedEvent ForwardRequested;

        public static void Navigate(Uri address) => NavigationRequested?.Invoke(address);

        public static void Refresh() => RefreshRequested?.Invoke();

        public static void Back() => BackRequested?.Invoke();

        public static void Forward() => ForwardRequested?.Invoke();
    }
}
