using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public interface IInventoryModule : IGlobalBinding
    {
        public void AddItem(IItem item, int count);
        public void AddItem(string itemId, int count);
        public void ReduceItem(IItem item, int count);
        public void ReduceItem(string itemId, int count);
        public void UpdateEquipmentProperties(IEquipment equipment, EquipmentProperties properties);
        public int GetItemCount(IItem item);
        public int GetItemCount(string itemId);
        public EquipmentProperties GetEquipmentProperties(IEquipment equipment);
        public UniTask ReadFromCloudAsync();
        public UniTask WriteItemCountsAsync();
        public UniTask WriteItemLevelsAsync();
        public UniTask WriteCurrencyCountsAsync();
    }
}
