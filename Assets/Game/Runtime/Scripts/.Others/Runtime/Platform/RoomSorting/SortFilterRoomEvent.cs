using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Lobby
{
    public class SortFilterRoomEvent : Event<string>
    {
        public const string EVENT_NAME = "SortFilterRoomEvent";

        public SortFilterRoomEvent() : base(EVENT_NAME)
        {
            SortMode = LobbySort.CurrentSortMode;
            FilterMode = LobbyFilter.CurrentFilterMode;
        }

        public SortFilterRoomEvent(SortRoomMode sortRoomMode, FilterRoomMode filterMode) : base(EVENT_NAME)
        {
            SortMode = sortRoomMode;
            FilterMode = filterMode;
        }

        public SortRoomMode SortMode { get; }
        public FilterRoomMode FilterMode { get; }
    }
}
