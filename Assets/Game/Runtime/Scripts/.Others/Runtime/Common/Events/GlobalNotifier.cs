using System;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Common
{
    public class GlobalNotifier : Singleton<GlobalNotifier>, INotifier<string>
    {
        private readonly Notifier<string> notifier = new Notifier<string>();

        public void Trigger(string evtKey)
        {
            notifier.Trigger(evtKey);
        }

        public void Trigger(IEvent<string> evt)
        {
            notifier.Trigger(evt);
        }

        public ISubscription<string> SubscribeOn(string evt, Action<IEvent<string>> listener)
        {
            return (notifier as ISubscriber<string>).SubscribeOn(evt, listener);
        }

        public void UnSubscribeFor(string evt, Action<IEvent<string>> listener)
        {
            (notifier as ISubscriber<string>).UnSubscribeFor(evt, listener);
        }

        public void UnSubscribeAllOn(string evt)
        {
            (notifier as ISubscriber<string>).UnSubscribeAllOn(evt);
        }
    }
}
