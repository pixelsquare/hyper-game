using System;
using com.ootii.Collections;
using MessageInternal = com.ootii.Messages.Message;
using IMessageInternal = com.ootii.Messages.IMessage;

namespace Santelmo.Rinsurv
{
    public class Message : IMessage, IEquatable<MessageInternal>
    {
        public string Type
        {
            get => _type;
            set => _type = value;
        }

        public object Sender
        {
            get => _sender;
            set => _sender = value;
        }

        public object Recipient
        {
            get => _recipient;
            set => _recipient = value;
        }

        public float Delay
        {
            get => _delay;
            set => _delay = value;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public object Data
        {
            get => _data;
            set => _data = value;
        }

        public bool IsSent
        {
            get => _isSent;
            set => _isSent = value;
        }

        public bool IsHandled
        {
            get => _isHandled;
            set => _isHandled = value;
        }

        public int FrameIndex
        {
            get => _frameIndex;
            set => _frameIndex = value;
        }

        protected string _type = "";
        protected object _sender;
        protected object _recipient;
        protected float _delay;
        protected int _id;
        protected object _data;
        protected bool _isSent;
        protected bool _isHandled;
        protected int _frameIndex;

        private readonly static ObjectPool<Message> _pool = new(40, 10);

        public Message()
        {
        }

        public Message(IMessageInternal message)
        {
            _type = message.Type;
            _sender = message.Sender;
            _recipient = message.Recipient;
            _delay = message.Delay;
            _id = message.ID;
            _data = message.Data;
            _isSent = message.IsSent;
            _isHandled = message.IsHandled;
            _frameIndex = message.FrameIndex;
        }

        public void Clear()
        {
            _type = "";
            _sender = null;
            _recipient = null;
            _id = 0;
            _data = null;
            _isSent = false;
            _isHandled = false;
            _delay = 0f;
        }

        public void Release()
        {
            Clear();

            _isSent = true;
            _isHandled = true;

            if (this is Message)
            {
                _pool.Release(this);
            }
        }

        public bool Equals(MessageInternal other)
        {
            return _type.Equals(other.Type) && _sender.Equals(other.Sender) && _recipient.Equals(other.Recipient) && _delay.Equals(other.Delay) && _id.Equals(other.ID) && _data.Equals(other.Data) && _isSent.Equals(other.IsSent) && _isHandled.Equals(other.IsHandled) && _frameIndex.Equals(other.FrameIndex);
        }

        public static Message Allocate()
        {
            var message = _pool.Allocate();
            message.IsSent = false;
            message.IsHandled = false;
            return message ?? new Message();
        }

        public static void Release(Message message)
        {
            if (message == null)
            {
                return;
            }

            message.IsSent = true;
            message.IsHandled = true;
            _pool.Release(message);
        }

        public static void Release(IMessage message)
        {
            if (message == null)
            {
                return;
            }

            message.Clear();
            message.IsSent = true;
            message.IsHandled = true;

            if (message is Message messageInstance)
            {
                _pool.Release(messageInstance);
            }
        }

        public static Message FromMessageInternal(IMessageInternal other)
        {
            return new Message(other);
        }

        public static MessageInternal ToMessageInternal(IMessage other)
        {
            return new MessageInternal
            {
                Type = other.Type,
                Sender = other.Sender,
                Recipient = other.Recipient,
                ID = other.Id,
                Data = other.Data,
                IsSent = other.IsSent,
                IsHandled = other.IsHandled,
                Delay = other.Delay
            };
        }
    }
}
