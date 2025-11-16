using System;
using System.Collections.Generic;
using System.Linq;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    /// Collection of extension methods for lists
    /// </summary>
    public static class ListExtensions
    {
        private static readonly Random defaultRng = new Random();

        /// <summary>
        /// Returns a random item from a list
        /// </summary>
        public static T GetRandomItem<T>(this IList<T> list)
        {
            if (!list.Any())
            {
                return default;
            }

            return list.ElementAt(defaultRng.Next(list.Count));
        }

        /// <summary>
        /// Randomizes the order in the list.
        /// Fisher-Yates shuffle implementation from https://stackoverflow.com/a/1262619.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            Shuffle(list, defaultRng);
        }

        /// <summary>
        /// Randomizes the order in the list.
        /// Fisher-Yates shuffle implementation from https://stackoverflow.com/a/1262619.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            var n = list.Count;
            while (n > 1)
            {
                --n;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
