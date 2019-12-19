using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse_Browser.Services
{
    public static class WebNavigationService
    {
        public class WebHistoryEntry
        {
            public DateTime VisitedAt { get; set; }
            public Uri Uri { get; set; }
        }

        public static Stack<WebHistoryEntry> WebHistory { get; set; } = new Stack<WebHistoryEntry>();

        public delegate void OnNavigatedEvent(Uri address);
        public static event OnNavigatedEvent NavigationRequested;

        public delegate void RefreshRequestedEvent();
        public static event RefreshRequestedEvent RefreshRequested;

        public delegate void BackRequestedEvent();
        public static event BackRequestedEvent BackRequested;

        public delegate void ForwardRequestedEvent();
        public static event ForwardRequestedEvent ForwardRequested;

        public static void Navigate(Uri address)
        {
            WebHistory.Push(new WebHistoryEntry()
            {
                Uri = address,
                VisitedAt = DateTime.Now
            });

            NavigationRequested?.Invoke(address);
        }

        public static void Refresh()
        {
            var lastVisited = WebHistory.Peek();
            if (lastVisited != null)
            {
                WebHistory.Push(new WebHistoryEntry()
                {
                    VisitedAt = DateTime.Now,
                    Uri = lastVisited.Uri
                });
            }

            RefreshRequested?.Invoke();
        }

        public static void Back()
        {
            var previousVisited = WebHistory.ElementAtOrDefault(1);
            if (previousVisited != null)
            {
                WebHistory.Push(new WebHistoryEntry()
                {
                    VisitedAt = DateTime.Now,
                    Uri = previousVisited.Uri
                });
            }

            BackRequested?.Invoke();
        }

        public static void Forward() => ForwardRequested?.Invoke();
    }
}
