using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Santelmo.Rinsurv.Tests
{
    public class DispatcherTests
    {
        private GameObject go;

        [SetUp]
        public void Setup()
        {
            go = new GameObject("MyGameObject");
        }

        [TearDown]
        public void Cleanup()
        {
            Dispatcher.ClearMessages();
            Dispatcher.ClearListeners();
            Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator Dispatcher_SendMessageBasic()
        {
            var myInt = 0;

            var handler = new MessageHandler(message =>
            {
                myInt = (int)message.Data;
            });

            Dispatcher.AddListener("MyEvent", handler);
            yield return null;

            Dispatcher.SendMessage(go, "MyEvent", 99, 0);
            yield return null;

            Assert.That(myInt, Is.EqualTo(99));
        }

        [UnityTest]
        public IEnumerator Dispatcher_SendMessageWithRecipientObject()
        {
            var myInt = 0;

            var handler = new MessageHandler(message =>
            {
                myInt = (int)message.Data;
            });

            Dispatcher.AddListener(go, "MyEvent", handler);
            yield return null;

            Dispatcher.SendMessage(go, go, "MyEvent", 99, 0);
            yield return null;

            Assert.That(myInt, Is.EqualTo(99));
        }

        [UnityTest]
        public IEnumerator Dispatcher_SendMessageWithRecipientString()
        {
            var myInt = 0;

            var handler = new MessageHandler(message =>
            {
                myInt = (int)message.Data;
            });

            Dispatcher.AddListener(go, "MyEvent", handler);
            yield return null;

            Dispatcher.SendMessage(go, "MyGameObject", "MyEvent", 99, 0);
            yield return null;

            Assert.That(myInt, Is.EqualTo(99));
        }
    }
}
