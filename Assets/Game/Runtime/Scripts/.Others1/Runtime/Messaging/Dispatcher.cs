using System;
using System.Collections.Generic;
using System.Linq;
using com.ootii.Messages;
using MessageInternal = com.ootii.Messages.IMessage;
using MessageHandlerInternal = com.ootii.Messages.MessageHandler;
using Object = UnityEngine.Object;

namespace Santelmo.Rinsurv
{
    public delegate void MessageHandler(IMessage message);

    public class Dispatcher
    {
        public MessageRecipientType RecipientType
        {
            get => (MessageRecipientType)MessageDispatcher.RecipientType;
            set => MessageDispatcher.RecipientType = (int)value;
        }

        public static bool ReportUnhandledMessages
        {
            get => MessageDispatcher.ReportUnhandledMessages;
            set => MessageDispatcher.ReportUnhandledMessages = value;
        }

        public static int FrameIndex
        {
            get => MessageDispatcher.FrameIndex;
            set => MessageDispatcher.FrameIndex = value;
        }

        public static List<IMessage> Messages
        {
            get => MessageDispatcher.Messages.Select(Message.FromMessageInternal).OfType<IMessage>().ToList();
            set => MessageDispatcher.Messages = value.Select(Message.ToMessageInternal).OfType<MessageInternal>().ToList();
        }

        // Message Handler Cache to store reference for Wrapped Message Handler and Internal Message Handler
        private readonly static Dictionary<MessageHandler, MessageHandlerInternal> _messageHandlerCache = new();

        public static void AddListener(string id, MessageHandler handler, bool immediate = false)
        {
            AddListener(id, "", handler, immediate);
        }

        public static void AddListener(string id, string filter, MessageHandler handler, bool immediate = false)
        {
            MessageHandlerInternal internalHandler = message => { handler?.Invoke(Message.FromMessageInternal(message)); };
            _messageHandlerCache.TryAdd(handler, internalHandler);
            MessageDispatcher.AddListener(id, filter, internalHandler, immediate);
        }

        public static void AddListener(Object owner, string id, MessageHandler handler, bool immediate = false)
        {
            MessageHandlerInternal internalHandler = message => { handler?.Invoke(Message.FromMessageInternal(message)); };
            _messageHandlerCache.TryAdd(handler, internalHandler);
            MessageDispatcher.AddListener(owner, id, internalHandler, immediate);
        }

        public static void SendMessage(string id, float delay = 0f)
        {
            MessageDispatcher.SendMessage(id, delay);
        }

        public static void SendMessage(string id, string filter, float delay = 0f)
        {
            MessageDispatcher.SendMessage(id, filter, delay);
        }

        public static void SendMessageData(string id, object data, float delay = 0f)
        {
            MessageDispatcher.SendMessageData(id, data, delay);
        }

        public static void SendMessage(object sender, string id, object data, float delay)
        {
            MessageDispatcher.SendMessage(sender, id, data, delay);
        }

        public static void SendMessage(object sender, object recipient, string id, object data, float delay)
        {
            MessageDispatcher.SendMessage(sender, recipient, id, data, delay);
        }

        public static void SendMessage(object sender, string recipient, string id, object data, float delay)
        {
            MessageDispatcher.SendMessage(sender, recipient, id, data, delay);
        }

        public static void SendMessage(IMessage message, bool setUnhandledToHandled = false)
        {
            MessageDispatcher.SendMessage(Message.ToMessageInternal(message), setUnhandledToHandled);
        }

        public static void RemoveListener(string id, MessageHandler handler, bool immediate)
        {
            RemoveListener(id, "", handler, immediate);
        }

        public static void RemoveListener(string id, string filter, MessageHandler handler, bool immediate = false)
        {
            if (!_messageHandlerCache.ContainsKey(handler))
            {
                throw new Exception("Handler does not exist!");
            }

            MessageDispatcher.RemoveListener(id, filter, _messageHandlerCache[handler], immediate);
            _messageHandlerCache.Remove(handler);
        }

        public static void RemoveListener(Object owner, string id, MessageHandler handler, bool immediate = false)
        {
            if (!_messageHandlerCache.ContainsKey(handler))
            {
                throw new Exception("Handler does not exist!");
            }

            MessageDispatcher.RemoveListener(owner, id, _messageHandlerCache[handler], immediate);
            _messageHandlerCache.Remove(handler);
        }

        public static void ClearMessages()
        {
            MessageDispatcher.ClearMessages();
        }

        public static void ClearListeners()
        {
            MessageDispatcher.ClearListeners();
            _messageHandlerCache.Clear();
        }
    }
}
