using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Santelmo.Rinsurv.Backend;

namespace Santelmo.Rinsurv
{
    /// <summary>
    /// Handles items count and level data
    /// </summary>
    public class InventoryModule : IInventoryModule, ICurrencyModule
    {
        public static InventoryModule Create(IItemDataModule itemDataModule, ICloudSyncService cloudSyncService)
        {
            return new InventoryModule(itemDataModule, cloudSyncService);
        }

        private readonly IItemDataModule _itemDataModule;
        private readonly ICloudSyncService _cloudSyncService;

        private readonly Dictionary<string, int> _inventoryMap = new();
        private Dictionary<string, IEquipment> _equipmentMap = new();
        private readonly Dictionary<string, int> _currencyMap = new();

        private CancellationTokenSource _readCancellationTokenSource = new();
        private CancellationTokenSource _writeCurrencyCancellationTokenSource = new();
        private CancellationTokenSource _writeCountsCancellationTokenSource = new();
        private CancellationTokenSource _writeLevelsCancellationTokenSource = new();

        private const string CURRENCY_COUNTS_KEY = "currency_counts";
        private const string ITEM_COUNTS_KEY = "item_counts";
        private const string ITEM_LEVELS_KEY = "item_levels";

        private InventoryModule(IItemDataModule itemDataModule, ICloudSyncService cloudSyncService)
        {
            _itemDataModule = itemDataModule;
            _cloudSyncService = cloudSyncService;
        }

        public void AddItem(IItem item, int count)
        {
            _inventoryMap.TryAdd(item.Id, 0);
            _inventoryMap[item.Id] += count;
        }

        public void AddItem(string itemId, int count)
        {
            var item = _itemDataModule.GetItemFromId(itemId);
            if (item is not null)
            {
                AddItem(item, count);
            }
            else
            {
                Debug.LogWarning($"Item {itemId} not found");
            }
        }

        public void ReduceItem(IItem item, int count)
        {
            if (!_inventoryMap.ContainsKey(item.Id))
            {
                return;
            }

            _inventoryMap[item.Id] -= count;

            if (_inventoryMap[item.Id] <= 0)
            {
                _inventoryMap.Remove(item.Id);
            }
        }

        public void ReduceItem(string itemId, int count)
        {
            var item = _itemDataModule.GetItemFromId(itemId);
            if (item is not null)
            {
                ReduceItem(item, count);
            }
            else
            {
                Debug.LogWarning($"Item {itemId} not found");
            }
        }

        public void UpdateEquipmentProperties(IEquipment equipment, EquipmentProperties properties)
        {
            _equipmentMap.TryAdd(equipment.Id, equipment);
            _equipmentMap[equipment.Id].EquipmentProperties = properties;
        }

        public int GetItemCount(IItem item)
        {
            return _inventoryMap.TryGetValue(item.Id, out var count) ? count : 0;
        }

        public int GetItemCount(string itemId)
        {
            var item = _itemDataModule.GetItemFromId(itemId);
            if (item is not null)
            {
                return GetItemCount(item);
            }

            Debug.LogWarning($"Item {itemId} not found");
            return 0;
        }

        public EquipmentProperties GetEquipmentProperties(IEquipment equipment)
        {
            return _equipmentMap.TryGetValue(equipment.Id, out var item) ? item.EquipmentProperties : new EquipmentProperties();
        }

        public async UniTask ReadFromCloudAsync()
        {
            if (_cloudSyncService is null)
            {
                Debug.LogError($"No {typeof(ICloudSyncService)} found");
                return;
            }

            _readCancellationTokenSource?.Cancel();
            _readCancellationTokenSource?.Dispose();
            _readCancellationTokenSource = new CancellationTokenSource();

            try
            {
                var (currency, counts, levels) = await UniTask.WhenAll(
                    _cloudSyncService.ReadAsync<Dictionary<string, int>>(CURRENCY_COUNTS_KEY, _readCancellationTokenSource.Token),
                    _cloudSyncService.ReadAsync<Dictionary<string, int>>(ITEM_COUNTS_KEY, _readCancellationTokenSource.Token),
                    _cloudSyncService.ReadAsync<Dictionary<string, EquipmentProperties>>(ITEM_LEVELS_KEY, _readCancellationTokenSource.Token));

                if (currency != null)
                {
                    foreach (var (key, value) in counts)
                    {
                        currency[key] = Convert.ToInt32(value);
                    }
                }

                if (counts != null)
                {
                    foreach (var (key, value) in counts)
                    {
                        _inventoryMap[key] = Convert.ToInt32(value);
                    }
                }

                if (levels != null)
                {
                    foreach (var (key, value) in levels)
                    {
                        if (!_equipmentMap.ContainsKey(key))
                        {
                            Debug.LogError($"Item {key} not found in local dictionary {nameof(_equipmentMap)}");
                            continue;
                        }

                        _equipmentMap[key].EquipmentProperties = value as EquipmentProperties;
                    }
                }
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e);
            }
        }

        public async UniTask WriteItemCountsAsync()
        {
            if (_cloudSyncService is null)
            {
                Debug.LogError($"No {typeof(ICloudSyncService)} found");
                return;
            }

            _writeCountsCancellationTokenSource?.Cancel();
            _writeCountsCancellationTokenSource?.Dispose();
            _writeCountsCancellationTokenSource = new CancellationTokenSource();

            var dataToSync = new Dictionary<string, int>();
            foreach (var itemCountPair in _inventoryMap)
            {
                dataToSync[itemCountPair.Key] = itemCountPair.Value;
            }

            try
            {
                await _cloudSyncService.WriteAsync(ITEM_COUNTS_KEY, dataToSync, _writeCountsCancellationTokenSource.Token);
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e);
            }
        }

        public async UniTask WriteItemLevelsAsync()
        {
            if (_cloudSyncService is null)
            {
                Debug.LogError($"No {typeof(ICloudSyncService)} found");
                return;
            }

            _writeLevelsCancellationTokenSource?.Cancel();
            _writeLevelsCancellationTokenSource?.Dispose();
            _writeLevelsCancellationTokenSource = new CancellationTokenSource();

            var dataToSync = new Dictionary<string, EquipmentProperties>();
            foreach (var equipment in _equipmentMap)
            {
                dataToSync[equipment.Key] = equipment.Value.EquipmentProperties;
            }

            try
            {
                await _cloudSyncService.WriteAsync(ITEM_LEVELS_KEY, dataToSync, _writeLevelsCancellationTokenSource.Token);
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e);
            }
        }

        public async UniTask WriteCurrencyCountsAsync()
        {
            if (_cloudSyncService is null)
            {
                Debug.LogError($"No {typeof(ICloudSyncService)} found");
                return;
            }

            _writeCurrencyCancellationTokenSource?.Cancel();
            _writeCurrencyCancellationTokenSource?.Dispose();
            _writeCurrencyCancellationTokenSource = new CancellationTokenSource();

            var dataToSync = new Dictionary<string, int>();
            foreach (var currencyCountPair in _currencyMap)
            {
                dataToSync[currencyCountPair.Key] = currencyCountPair.Value;
            }

            try
            {
                await _cloudSyncService.WriteAsync(CURRENCY_COUNTS_KEY, dataToSync, _writeCurrencyCancellationTokenSource.Token);
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e);
            }
        }

        public int GetCurrency(string key)
        {
            return _currencyMap.TryGetValue(key, out var count) ? count : 0;
        }

        public int AddCurrency(string key, int amount)
        {
            _currencyMap.TryAdd(key, 0);
            _currencyMap[key] += amount;

            return _currencyMap[key];
        }

        public int ReduceCurrency(string key, int amount)
        {
            if (!_currencyMap.ContainsKey(key))
            {
                return 0;
            }

            _currencyMap[key] -= amount;

            return _currencyMap[key];
        }
    }
}
