using System;

namespace Santelmo.Rinsurv
{
    public static class BlackboardExtensions
    {
        public static void Add<T>(this Blackboard blackboard, string key, object item)
        {
            if (!blackboard.Add(key, item, typeof(T)))
            {
                throw new ArgumentException("Key already exist.", nameof(key));
            }
        }

        public static bool TryAdd<T>(this Blackboard blackboard, string key, object item)
        {
            return blackboard.Add(key, item, typeof(T));
        }

        public static object Get(this Blackboard blackboard, string key, object defaultValue = null)
        {
            return blackboard.ContainsKey(key) ? blackboard[key] : defaultValue;
        }

        public static T Get<T>(this Blackboard blackboard, string key)
        {
            if (!blackboard.ContainsKey(key))
            {
                throw new ArgumentException("Key does not exist.", nameof(key));
            }

            if (blackboard[key].GetType() != typeof(T))
            {
                throw new ArgumentException("Argument mismatch.", nameof(T));
            }

            return (T)blackboard[key];
        }

        public static bool TryGet(this Blackboard blackboard, string key, out object item)
        {
            return blackboard.TryGetValue(key, out item);
        }

        public static bool Contains(this Blackboard blackboard, string key)
        {
            return blackboard.ContainsKey(key);
        }

        public static bool IsTypeOf<T>(this Blackboard blackboard, string key)
        {
            return blackboard.IsTypeOf(key, typeof(T));
        }
    }
}
