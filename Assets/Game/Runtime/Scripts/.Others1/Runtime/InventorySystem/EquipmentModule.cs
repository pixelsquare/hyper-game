using System;
using System.Collections.Generic;

namespace Santelmo.Rinsurv
{
    public class EquipmentModule : IMergeEquipmentModule, ILevelingEquipmentModule
    {
        private readonly IInventoryModule _inventoryModule;
        private readonly ICurrencyModule _currencyModule;

        // TODO: Move to scriptable object?
        private readonly Dictionary<int, int> _equipmentLevelingCostMap = new()
        {
            {1, 1000},
            {2, 2500},
            {3, 5000},
            {4, 10_000},
            {5, 25_000},
            {6, 50_000},
            {7, 100_000},
            {8, 250_000},
            {9, 500_000},
            {10, 1_000_000},
        };

        public EquipmentModule(IInventoryModule invModule, ICurrencyModule currModule)
        {
            _inventoryModule = invModule;
            _currencyModule = currModule;
        }
        
        public IEquipment CanMergeEquipment(IEquipment baseEquipment, IEquipment addOnEquipment)
        {
            if (baseEquipment.EquipmentProperties.Quality != addOnEquipment.EquipmentProperties.Quality)
            {
                return null;
            }

            if (baseEquipment.EquipmentProperties.Quality >= (int) EquipmentQuality.Legendary)
            {
                return null;
            }

            IEquipment newEquipment;
            
            // TODO: Move to scriptable object?
            switch ((EquipmentQuality) baseEquipment.EquipmentProperties.Quality)
            {
                case EquipmentQuality.Common:
                case EquipmentQuality.Excellent:
                    if (baseEquipment.GetType() != addOnEquipment.GetType())
                    {
                        return null;
                    }
                    
                    newEquipment = (baseEquipment as ICloneable)?.Clone() as IEquipment;
                    newEquipment?.UpgradeQuality();
                    return newEquipment;
                case EquipmentQuality.Good:
                case EquipmentQuality.Great:
                    newEquipment = (baseEquipment as ICloneable)?.Clone() as IEquipment;
                    newEquipment?.UpgradeQuality();
                    return newEquipment;
                case EquipmentQuality.Legendary:
                default:
                    return null;
            }
        }

        public IEquipment MergeEquipment(IEquipment baseEquipment, IEquipment addOnEquipment)
        {
            var newEquipment = CanMergeEquipment(baseEquipment, addOnEquipment);

            if (newEquipment is null)
            {
                return null;
            }

            _inventoryModule.ReduceItem(baseEquipment, 1);
            _inventoryModule.ReduceItem(addOnEquipment, 1);
            _inventoryModule.AddItem(newEquipment, 1);
            
            return newEquipment;
        }
        
        public (IEquipment equipment, int cost) CanLevelUpEquipment(IEquipment baseEquipment)
        {
            var targetLevel = baseEquipment.EquipmentProperties.Level + 1;

            if (_equipmentLevelingCostMap.TryGetValue(targetLevel, out var cost))
            {
                if (_currencyModule.GetCurrency(Currency.COIN_KEY) < cost)
                {
                    return (null, cost);
                }
                
                var newEquipment = (baseEquipment as ICloneable)?.Clone() as IEquipment;
                newEquipment?.UpgradeLevel();
                return (newEquipment, cost);
            }

            return (null, 0);
        }

        public IEquipment LevelUpEquipment(IEquipment baseEquipment)
        {
            var newEquipmentCostPair = CanLevelUpEquipment(baseEquipment);

            if (newEquipmentCostPair.equipment is null)
            {
                return null;
            }
            
            _inventoryModule.ReduceItem(baseEquipment, 1);
            _inventoryModule.AddItem(newEquipmentCostPair.equipment, 1);
            _currencyModule.ReduceCurrency(Currency.COIN_KEY, newEquipmentCostPair.cost);
            return newEquipmentCostPair.equipment;
        }
    }
}
