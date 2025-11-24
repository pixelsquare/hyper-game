using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public interface IInventoryManager : IGlobalBinding
    {
        public UniTask InitializeAsync();

        public void AddItemsToInventory(IEnumerable<IItem> items);

        public UniTask SyncUpAsync();

        public UniTask SyncDownAsync();
    }
}
