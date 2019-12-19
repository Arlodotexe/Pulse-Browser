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
            public HistoryNavigationType NavigationType { get; set; }
        }

        public enum HistoryNavigationType
        {
            Direct, Refresh, Back, Forward
        }

        /// <summary>
        /// Represents the full web history, complete with forward, backward, and refresh navigation entries
        /// </summary>
        public static Stack<WebHistoryEntry> WebHistoryStack { get; set; } = new Stack<WebHistoryEntry>();

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
            WebHistoryStack.Push(new WebHistoryEntry()
            {
                Uri = address,
                VisitedAt = DateTime.Now,
                NavigationType = HistoryNavigationType.Direct
            });

            NavigationRequested?.Invoke(address);
        }

        public static void Refresh()
        {
            var lastVisited = WebHistoryStack.Peek();
            if (lastVisited != null)
            {
                WebHistoryStack.Push(new WebHistoryEntry()
                {
                    VisitedAt = DateTime.Now,
                    Uri = lastVisited.Uri,
                    NavigationType = HistoryNavigationType.Refresh
                });
            }

            RefreshRequested?.Invoke();
        }

        public static void Back()
        {
            var validNavigationStack = WebHistoryStack
                // Don't include navigations where the page was refreshed or navigated back, they could result in showing the same page again
                .Where(e => e.NavigationType == HistoryNavigationType.Direct || e.NavigationType == HistoryNavigationType.Forward);

            if (validNavigationStack?.ElementAtOrDefault(1) != null)
            {
                WebHistoryStack.Push(new WebHistoryEntry()
                {
                    VisitedAt = DateTime.Now,
                    Uri = validNavigationStack?.ElementAtOrDefault(1).Uri,
                    NavigationType = HistoryNavigationType.Back
                });
            }

            BackRequested?.Invoke();
        }

        public static void Forward()
        {
            var previousVisited = WebHistoryStack.ElementAtOrDefault(1);
            if (previousVisited != null)
            {
                WebHistoryStack.Push(new WebHistoryEntry()
                {
                    VisitedAt = DateTime.Now,
                    Uri = previousVisited.Uri,
                    NavigationType = HistoryNavigationType.Forward
                });
            }

            ForwardRequested?.Invoke();
        }
    }
}
