using NUnit.Framework;

namespace Santelmo.Rinsurv.Editor.Tests
{
    public class DispatcherEditorTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Cleanup()
        {
            Dispatcher.ClearMessages();
            Dispatcher.ClearListeners();
        }

        [Test]
        public void Dispatcher_SendMessageBasic()
        {
            var myInt = 0;

            var handler = new MessageHandler(message =>
            {
                myInt = 99;
            });

            Dispatcher.AddListener("MyEvent", handler, true);
            Dispatcher.SendMessage("MyEvent");

            Assert.That(myInt, Is.EqualTo(99));
        }

        [Test]
        public void Dispatcher_SendMessageData()
        {
            var myInt = 0;

            var handler = new MessageHandler(message =>
            {
                myInt = (int)message.Data;
            });

            Dispatcher.AddListener("MyEvent", handler, true);
            Dispatcher.SendMessageData("MyEvent", 99);

            Assert.That(myInt, Is.EqualTo(99));
        }

        [Test]
        public void Dispatcher_SendMessageWithFilter()
        {
            var myInt = 0;

            var handler = new MessageHandler(message =>
            {
                myInt = 99;
            });

            Dispatcher.AddListener("MyEvent", "MyFilter", handler, true);
            Dispatcher.SendMessage("MyEvent", "MyFilter");

            Assert.That(myInt, Is.EqualTo(99));
        }

        [Test]
        public void Dispatcher_SendCustomMessage()
        {
            var message = Message.Allocate();
            message.Type = "MyEvent";
            message.Data = new
            {
                MaxHealth = 100,
                CurrentHealth = 50
            };

            dynamic data = null;

            var handler = new MessageHandler(message =>
            {
                data = message.Data;
            });

            Dispatcher.AddListener("MyEvent", handler, true);
            Dispatcher.SendMessage(message);

            Assert.That(data.MaxHealth, Is.EqualTo(100));
            Assert.That(data.CurrentHealth, Is.EqualTo(50));
        }

        [Test]
        public void Dispatcher_RemoveListener()
        {
            var myInt = 0;

            var handler = new MessageHandler(message =>
            {
                myInt = 99;
            });

            Dispatcher.AddListener("MyEvent", handler, true);
            Dispatcher.RemoveListener("MyEvent", handler, true);
            Dispatcher.SendMessage("MyEvent");

            Assert.That(myInt, Is.EqualTo(0));
        }
    }
}
