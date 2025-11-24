using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public static class ItemDatabaseExtensions
    {
        public static IEnumerable<IItem> GetRandomItems(this ItemDatabase itemDatabase, int count)
        {
            var allItems = itemDatabase.AllItems.ToArray();
            return Enumerable.Range(0, count)
                             .Select(x => allItems.ElementAt(Random.Range(0, allItems.Length)));
        }
    }
}
