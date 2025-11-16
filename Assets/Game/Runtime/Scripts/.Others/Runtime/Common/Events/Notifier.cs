using System;
using System.Collections.Generic;

namespace Kumu.Kulitan.Events
{
    public class Notifier<EventID> : INotifier<EventID> where EventID : IComparable
    {
        private Dictionary<EventID, IChannel<EventID>> channels;
        private Event<EventID> defaultEvent;
        private Dictionary<EventID, int> eventMapCount;

        public Notifier()
        {
            channels = new Dictionary<EventID, IChannel<EventID>>();
            defaultEvent = new Event<EventID>(default(EventID));
            eventMapCount = new Dictionary<EventID, int>();
        }

        public Dictionary<EventID, int> GetEventMapCount()
        {
            return eventMapCount;
        }

        public int GetEventCount(EventID evt)
        {
            eventMapCount.TryGetValue(evt, out var count);
            return count;
        }

        public ISubscription<EventID> SubscribeOn(EventID evt, Action<IEvent<EventID>> listener)
        {
            var channel = GetChannelFor(evt);
            if (channel == null)
            {
                channel = new Channel<EventID>(evt);
                channels.Add(evt, channel);
            }
            return channel.Subscribe(listener);
        }
        
        public void Trigger(EventID evtKey)
        {
            defaultEvent.ID = evtKey;
            Trigger(defaultEvent);
        }

        public void Trigger(IEvent<EventID> evt)
        {
            if (eventMapCount.ContainsKey(evt.ID))
            {
                eventMapCount[evt.ID]++;
            }
            else
            {
                eventMapCount.Add(evt.ID, 1);
            }

            var channel = GetChannelFor(evt.ID);
            channel?.Trigger(evt);
        }

        public void UnSubscribeFor(EventID evt, Action<IEvent<EventID>> listener)
        {
            var channel = GetChannelFor(evt);
            channel?.UnSubscribe(listener);
        }

        public void UnSubscribeAllOn(EventID evt)
        {
            var channel = GetChannelFor(evt);
            channel?.UnSubscribeAll();
        }

        private IChannel<EventID> GetChannelFor(EventID evt)
        {
            channels.TryGetValue(evt, out var channel);
            return channel;
        }
    }
}
