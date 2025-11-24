using NUnit.Framework;

namespace Santelmo.Rinsurv.Editor.Tests
{
    public class BlackboardEditorTest
    {
        private Blackboard blackboard;

        [SetUp]
        public void Setup()
        {
            blackboard = new Blackboard();
        }

        [TearDown]
        public void Cleanup()
        {
        }

        [Test]
        public void Blackboard_Add()
        {
            blackboard.Add("a", 1);
            blackboard.Add<int>("b", 1);
            Assert.That(blackboard.Count, Is.EqualTo(2));

            var success = blackboard.TryAdd<double>("c", 50.0);
            Assert.That(success, Is.True);
            
            blackboard.Add<string>("foo","bar");
            Assert.That(() => blackboard.Add<string>("foo","foobar"), Throws.ArgumentException); // key already exists
        }

        [Test]
        public void Blackboard_AddDuplicate()
        {
            blackboard.Add<int>("a", 100);
            var success = blackboard.TryAdd<int>("a", 50);
            Assert.That(success, Is.False);
        }

        [Test]
        public void Blackboard_Remove()
        {
            blackboard.Add<string>("hello", "world");
            Assert.That(blackboard.Count, Is.EqualTo(1));

            blackboard.Remove("hello");
            Assert.That(blackboard.Count, Is.EqualTo(0));
        }

        [Test]
        public void Blackboard_Get()
        {
            blackboard.Add<string>("hello", "world");

            var value = blackboard.Get("hello");
            Assert.That(value, Is.EqualTo("world"));

            value = blackboard.Get<string>("hello");
            Assert.That(value, Is.EqualTo("world"));
        }

        [Test]
        public void Blackboard_Contains()
        {
            blackboard.Add<string>("hello", "world");
            Assert.That(blackboard.Contains("hello"), Is.True);
            
            Assert.That(() => blackboard.Get<string>("Foobar"), Throws.ArgumentException); // key does not exist
            Assert.That(() => blackboard.Get<bool>("hello"), Throws.ArgumentException); // mismatched type
        }

        [Test]
        public void Blackboard_Clear()
        {
            blackboard.Add<string>("hello", "world");
            Assert.That(blackboard.Count, Is.EqualTo(1));

            blackboard.Clear();
            Assert.That(blackboard.Count, Is.EqualTo(0));
        }

        [Test]
        public void Blackboard_IsTypeOf()
        {
            blackboard.Add<float>("myFloat", 33.59f);
            Assert.That(blackboard.Count, Is.EqualTo(1));
            Assert.That(blackboard.IsTypeOf("myFloat", typeof(float)), Is.True);

            blackboard.Add<string>("hello", "world");
            Assert.That(blackboard.Count, Is.EqualTo(2));
            Assert.That(blackboard.IsTypeOf<string>("hello"), Is.True);
        }
    }
}
