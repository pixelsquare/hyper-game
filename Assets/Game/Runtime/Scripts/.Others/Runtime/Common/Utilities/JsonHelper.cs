using System;
using UnityEngine;


namespace Kumu.Kulitan.Common
{
    public static class JsonHelper
    {
        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>      
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Data;
        }

        /// <summary>
        /// Serializes Array into JSON format
        /// </summary>         
        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Data = array;
            return JsonUtility.ToJson(wrapper);
        }

        /// <summary>
        /// Serializes Array into JSON format with boolean can be used as a param checker.
        /// </summary>         
        public static string ToJson<T>(T[] array, bool isJsonTrue)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Data = array;
            return JsonUtility.ToJson(wrapper, isJsonTrue);
        }


        /// <summary>
        /// Generic data struct reference to any serializable data.
        /// </summary>
        [Serializable]
        private class Wrapper<T>
        {
            public T[] Data;
        }
    }
}
