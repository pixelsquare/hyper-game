using System;
using System.Collections.Generic;
using System.Linq;

namespace Kumu.Kulitan.Events
{
    public class Slot<Key> : ISubscriber<Key>, IDisposable
    {
        private INotifier<Key> notifier;
        private Dictionary<Key, List<ISubscription<Key>>> subscriptions;

        public Slot(INotifier<Key> notifier)
        {
            subscriptions = new Dictionary<Key, List<ISubscription<Key>>>();
            this.notifier = notifier;
        }
        
        public bool IsValid => notifier != null;

        public void UnSubscribeFor(Key evt, Action<IEvent<Key>> listener)
        {
            if (notifier == null)
            {
                return;
            }
            GetSubscriptions(evt).ForEach(x =>
            {
                if (x.Action == listener)
                {
                    x.Cancel();
                }
            });
        }

        public void UnSubscribeAllOn(Key evt)
        {
            if (notifier == null)
            {
                return;
            }
            GetSubscriptions(evt).ForEach(x => x.Cancel());
        }

        public ISubscription<Key> SubscribeOn(Key evt, Action<IEvent<Key>> listener)
        {
            if (notifier == null)
            {
                return null;
            }
            var subscription = notifier.SubscribeOn(evt, listener);
            GetSubscriptions(evt).Add(subscription);
            return subscription;
        }

        public void CancelAll()
        {
            if (subscriptions == null)
            {
                return;
            }

            foreach (var obj in subscriptions.Select(p => p.Value))
            {
                obj.ForEach(x => x.Cancel());
            };

            subscriptions.Clear();
        }

        public void Dispose()
        {
            CancelAll();

            subscriptions = null;
            notifier = null;
        }

        private List<ISubscription<Key>> GetSubscriptions(Key evt)
        {
            if (!subscriptions.TryGetValue(evt, out var events))
            {
                events = new List<ISubscription<Key>>();
                subscriptions.Add(evt, events);
            }

            return events;
        }
    }
}
