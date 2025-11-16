using System.Collections.Generic;
using Firebase.Messaging;
using Kumu.Extensions;

namespace Kumu.Kulitan.Backend
{
    public class FCMRemoteDataNotification : IRemoteDataNotification
    {
        private readonly FirebaseMessage message;

        public FCMRemoteDataNotification(FirebaseMessage message)
        {
            if (message.Data.Count == 0) // TODO (cj): add return?
            {
                "[FCMRemoteDataNotification] Data count is 0".LogError();
            }
             
            "[FCMRemoteDataNotification] Created notif".Log();
            this.message = message;
        }

        public string From => message.From;
        public string To => message.To;
        public string MessageId => message.MessageId;
        public IDictionary<string, string> Data => message.Data;
    }
}
