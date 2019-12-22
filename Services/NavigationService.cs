using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse_Browser.Services
{
    public class NavigationEntry
    {
        public DateTime VisitedAt { get; set; }
        public NavigationPageType Kind { get; set; }

        public Uri WebUri { get; set; }

        public Type NativePageType { get; set; }
        public object NativePageParam { get; set; }

        /// <summary>
        /// Marker indicating that this is the current active page
        /// </summary>
        public bool Current
        { get; set; } = false;
    }
    public enum NavigationPageType
    {
        Web, Native
    }
    public class NavigationService
    {

        public Stack<NavigationEntry> HistoryStack { get; set; } = new Stack<NavigationEntry>();

        public delegate void OnNavigatedEvent(NavigationEntry navigationEntry);
        public event OnNavigatedEvent NavigationRequested;

        public delegate void RefreshRequestedEvent();
        public event RefreshRequestedEvent RefreshRequested;

        public delegate void BackRequestedEvent(NavigationEntry navigationEntry);
        public event BackRequestedEvent BackRequested;

        public delegate void ForwardRequestedEvent(NavigationEntry navigationEntry);
        public event ForwardRequestedEvent ForwardRequested;

        public delegate void CanGoForwardChangedEvent(bool canGoForward);
        public event CanGoForwardChangedEvent CanGoForwardChanged;

        public delegate void CanGoBackChangedEvent(bool canGoBack);
        public event CanGoBackChangedEvent CanGoBackChanged;

        public void Navigate(Uri address)
        {
            // Reset all entries to not current
            foreach (var h in HistoryStack) h.Current = false;

            var newNavigationEntry = new NavigationEntry()
            {
                Kind = NavigationPageType.Web,
                WebUri = address,
                VisitedAt = DateTime.Now,
                Current = true
            };

            HistoryStack.Push(newNavigationEntry);

            CanGoBackChanged?.Invoke(true);
            CanGoForwardChanged?.Invoke(false);

            NavigationRequested?.Invoke(newNavigationEntry);
        }
        public void Navigate(Type pageType) => Navigate(pageType, null);
        public void Navigate(Type pageType, object param)
        {
            // Reset all entries to not current
            foreach (var h in HistoryStack) h.Current = false;

            var newNavigationEntry = new NavigationEntry()
            {
                Kind = NavigationPageType.Native,
                NativePageType = pageType,
                NativePageParam = param,
                VisitedAt = DateTime.Now,
                Current = true
            };

            HistoryStack.Push(newNavigationEntry);

            CanGoBackChanged?.Invoke(true);
            CanGoForwardChanged?.Invoke(false);

            NavigationRequested?.Invoke(newNavigationEntry);
        }

        public void Refresh()
        {
            RefreshRequested?.Invoke();
        }

        public void Back()
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

                BackRequested?.Invoke(newEntry);
            }
        }

        public void Forward()
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

                ForwardRequested?.Invoke(newEntry);
            }
        }
    }
}
