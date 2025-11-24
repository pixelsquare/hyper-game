using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Santelmo.Rinsurv
{
    public class Blackboard : IDictionary<string, object>
    {
        private readonly Dictionary<string, BlackboardVariable> blackboardMap = new();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return (from blackboard in blackboardMap
                    select new KeyValuePair<string, object>(blackboard.Key, blackboard.Value.value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value, item.Value.GetType());
        }

        public void Clear()
        {
            blackboardMap.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return blackboardMap.ContainsValue(ConvertToBlackboardVariable(item));
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            var blackboardValues = new List<BlackboardVariable>(blackboardMap.Values);
            blackboardValues.RemoveRange(0, arrayIndex);

            array = (from blackboard in blackboardValues
                     select new KeyValuePair<string, object>(blackboard.id, blackboard.value)).ToArray();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return blackboardMap.Remove(item.Key);
        }

        public int Count => blackboardMap.Count;
        public bool IsReadOnly => false;

        public void Add(string key, object value)
        {
            Add(key, value, typeof(object));
        }

        public bool ContainsKey(string key)
        {
            return blackboardMap.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return blackboardMap.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            var keyExist = blackboardMap.TryGetValue(key, out var blackboardVar);
            value = blackboardVar.value;
            return keyExist;
        }

        public object this[string key]
        {
            get => blackboardMap[key].value;
            set => blackboardMap[key] = ConvertToBlackboardVariable(key, value);
        }

        public ICollection<string> Keys => blackboardMap.Keys;
        public ICollection<object> Values => (from blackboard in blackboardMap
                                              select blackboard.Value.value).ToList();

        public bool Add(string key, object item, Type type)
        {
            return blackboardMap.TryAdd(key, new BlackboardVariable
            {
                id = key,
                type = type,
                value = item
            });
        }

        public bool IsTypeOf(string key, Type type)
        {
            if (!blackboardMap.ContainsKey(key))
            {
                throw new ArgumentException("Key does not exist.", nameof(key));
            }
            
            return blackboardMap[key].type == type;
        }

        private BlackboardVariable ConvertToBlackboardVariable(KeyValuePair<string, object> item)
        {
            var key = item.Key;
            var value = item.Value;
            var type = item.Value.GetType();

            return new BlackboardVariable
            {
                id = key,
                type = type,
                value = value
            };
        }

        private BlackboardVariable ConvertToBlackboardVariable(string key, object item)
        {
            return new BlackboardVariable
            {
                id = key,
                type = item.GetType(),
                value = item
            };
        }
    }
}
