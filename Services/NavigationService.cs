using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse_Browser.Services
{
    public static class NavigationService
    {
        public delegate void OnNavigatedEvent(Uri address);
        public static event OnNavigatedEvent NavigationRequested;

        public static void Navigate(Uri address)
        {
            NavigationRequested?.Invoke(address);
        }
    }
}
