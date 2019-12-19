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
            /// <summary>
            /// Marker indicating that this is the current active page
            /// </summary>
            public bool Current { get; set; } = false;
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

        public delegate void BackRequestedEvent(Uri address);
        public static event BackRequestedEvent BackRequested;

        public delegate void ForwardRequestedEvent(Uri address);
        public static event ForwardRequestedEvent ForwardRequested;

        public delegate void CanGoForwardChangedEvent(bool canGoForward);
        public static event CanGoForwardChangedEvent CanGoForwardChanged;

        public delegate void CanGoBackChangedEvent(bool canGoBack);
        public static event CanGoBackChangedEvent CanGoBackChanged;

        public static void Navigate(Uri address)
        {
            // Reset all entries to not current
            foreach (var h in WebHistoryStack) h.Current = false;

            WebHistoryStack.Push(new WebHistoryEntry()
            {
                Uri = address,
                VisitedAt = DateTime.Now,
                NavigationType = HistoryNavigationType.Direct,
                Current = true
            });

            CanGoBackChanged?.Invoke(true);
            CanGoForwardChanged?.Invoke(false);

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
            var currentEntry = WebHistoryStack.FirstOrDefault(h => h.Current);

            var validNavigationStack = WebHistoryStack
                // Transform to list so we don't affect the original stack
                ?.ToList();

            int currentIndex = WebHistoryStack.ToList().IndexOf(currentEntry);

            if (currentIndex > 0)
            {
                validNavigationStack = validNavigationStack
                // Remove all items preceding the current item, include current
                ?.GetRange(currentIndex, WebHistoryStack.Count() - currentIndex);
            }

            validNavigationStack = validNavigationStack
            // Don't include navigations where the page was refreshed or navigated back, they could result in showing the same page again
            ?.Where(e => e.NavigationType == HistoryNavigationType.Direct || e.NavigationType == HistoryNavigationType.Forward)

            // Convert it back to an array so we can iterate through it properly
            ?.ToList();

            var newEntry = validNavigationStack.ElementAtOrDefault(1);
            validNavigationStack.Remove(newEntry);

            if (validNavigationStack != null)
            {
                CanGoBackChanged?.Invoke(validNavigationStack?.Count > 1 == true);
                CanGoForwardChanged?.Invoke(true);

                WebHistoryStack.Push(new WebHistoryEntry()
                {
                    VisitedAt = DateTime.Now,
                    Uri = newEntry.Uri,
                    NavigationType = HistoryNavigationType.Back
                });

                BackRequested?.Invoke(newEntry.Uri);
            }
        }

        public static void Forward()
        {
            var currentEntry = WebHistoryStack.FirstOrDefault(h => h.Current);

            int currentIndex = WebHistoryStack.ToList().IndexOf(currentEntry);

            // If the current index isn't the last one in the stack, adjust the index to include the currentEntry
            if (currentIndex != WebHistoryStack.Count() - 1)
            {
                currentIndex += 1;
            }

            var validNavigationStack = WebHistoryStack
                // Transform to list so we don't affect the original stack
                ?.ToList()
                // Get all items preceding the current item, including current
                ?.GetRange(0, currentIndex)
                // Don't include navigations where the page was refreshed or navigated back, they could result in showing the same page again
                .Where(e => e.NavigationType == HistoryNavigationType.Back || e.NavigationType == HistoryNavigationType.Direct)
                // Convert it back to an array so we can iterate through it properly
                ?.ToList();


            // Reverse it to get the item that was closest to the currentEntry
            validNavigationStack.Reverse();

            var newEntry = validNavigationStack.ElementAtOrDefault(0);
            // Remove this new entry so we can correctly count the number of further valid history items
            validNavigationStack.Remove(newEntry);

            if (newEntry != null)
            {
                CanGoBackChanged?.Invoke(true);
                CanGoForwardChanged?.Invoke(currentIndex != WebHistoryStack.Count() - 1);

                WebHistoryStack.Push(new WebHistoryEntry()
                {
                    VisitedAt = DateTime.Now,
                    Uri = newEntry.Uri,
                    NavigationType = HistoryNavigationType.Forward
                });
                ForwardRequested?.Invoke(newEntry.Uri);
            }
        }
    }
}
