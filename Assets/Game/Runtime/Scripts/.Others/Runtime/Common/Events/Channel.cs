using System;
using System.Collections.Generic;

namespace Kumu.Kulitan.Events
{
    public class Channel<EventID> : IChannel<EventID> where EventID : IComparable
    {
        private readonly EventID id;
        private readonly List<ISubscription<EventID>> subscriptions;

        public Channel(EventID id)
        {
            subscriptions = new List<ISubscription<EventID>>();
            this.id = id;
        }

        public ISubscription<EventID> Subscribe(Action<IEvent<EventID>> listener)
        {
            ISubscription<EventID> subscription = new Subscription<EventID>();
            subscriptions.Add(subscription);
            subscription.Action = listener;
            subscription.ID = id;
            subscription.Channel = this;
            return subscription;
        }

        public void UnSubscribe(Action<IEvent<EventID>> listener)
        {
            for (var i = subscriptions.Count - 1; i >= 0; --i)
            {
                if (subscriptions[i].Action == listener)
                {
                    subscriptions.RemoveAt(i);
                    break;
                }
            }
        }

        public void UnSubscribeAll()
        {
            subscriptions.Clear();
        }

        public void Remove(ISubscription<EventID> subscription)
        {
            subscriptions.Remove(subscription);
        }

        public void Trigger(IEvent<EventID> evt)
        {
            for (var i = subscriptions.Count - 1; i >= 0 && i < subscriptions.Count; --i)
            {
                var subscription = subscriptions[i];
                if (!subscription.Invoke(evt))
                {
                    subscriptions.Remove(subscription);
                }
            }
        }
    }
}
