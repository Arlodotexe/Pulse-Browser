using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse_Browser.Services
{
    public static class NavigationService
    {
        public class HistoryEntry
        {
            public DateTime VisitedAt { get; set; }
            public Uri Uri { get; set; }
            /// <summary>
            /// Marker indicating that this is the current active page
            /// </summary>
            public bool Current { get; set; } = false;
        }

        public static Stack<HistoryEntry> HistoryStack { get; set; } = new Stack<HistoryEntry>();

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
            foreach (var h in HistoryStack) h.Current = false;

            HistoryStack.Push(new HistoryEntry()
            {
                Uri = address,
                VisitedAt = DateTime.Now,
                Current = true
            });

            CanGoBackChanged?.Invoke(true);
            CanGoForwardChanged?.Invoke(false);

            NavigationRequested?.Invoke(address);
        }

        public static void Refresh()
        {
            RefreshRequested?.Invoke();
        }

        public static void Back()
        {
            var currentEntry = HistoryStack.FirstOrDefault(h => h.Current);
            int currentIndex = HistoryStack.ToList().IndexOf(currentEntry);

            var validNavigationStack = HistoryStack
                // Transform to list so we don't affect the original stack
                ?.ToList();

            // Find the index of the current address in the valid history items
            int currentIndex_validStack = validNavigationStack.FindLastIndex(h => h.Current);

            var newEntry = validNavigationStack.ElementAtOrDefault(currentIndex_validStack + 1);
            if (newEntry is null)
            {
                // We can't go back, the UI should reflect that
                CanGoBackChanged?.Invoke(false);
                // And the code shouldn't try.
                return;
            }

            foreach (var h in HistoryStack) h.Current = false;

            // This is the entry we're navigating to, so it should be marked as current
            HistoryStack.First(h => h.VisitedAt == newEntry.VisitedAt).Current = true;
            int newHistoryItemIndexOnStack = HistoryStack.ToList().FindIndex(h => h.VisitedAt == newEntry.VisitedAt);

            if (validNavigationStack != null)
            {
                CanGoBackChanged?.Invoke(newHistoryItemIndexOnStack + 1 < HistoryStack.Count());
                CanGoForwardChanged?.Invoke(true);

                BackRequested?.Invoke(newEntry.Uri);
            }
        }

        public static void Forward()
        {
            var currentEntry = HistoryStack.FirstOrDefault(h => h.Current);

            int currentIndex = HistoryStack.ToList().IndexOf(currentEntry);

            var validNavigationStack = HistoryStack
                // Transform to list so we don't affect the original stack
                ?.ToList();

            // Find the index of the current address in the valid history items
            int currentIndex_validStack = validNavigationStack.FindIndex(h => h.VisitedAt == currentEntry.VisitedAt);

            var newEntry = validNavigationStack.ElementAtOrDefault(currentIndex_validStack - 1);
            if (newEntry is null)
            {
                // We can't go forward, the UI should reflect that
                CanGoForwardChanged?.Invoke(false);
                // And the code shouldn't try.
                return;
            }

            foreach (var h in HistoryStack) h.Current = false;

            // This is the entry we're navigating to, so it should be marked as current
            HistoryStack.First(h => h.VisitedAt == newEntry.VisitedAt).Current = true;

            if (newEntry != null)
            {
                CanGoBackChanged?.Invoke(true);
                // If there is something else at the front of the stack, we can navigate forward to it
                CanGoForwardChanged?.Invoke(currentIndex_validStack - 1 > 0);

                ForwardRequested?.Invoke(newEntry.Uri);
            }
        }
    }
}
