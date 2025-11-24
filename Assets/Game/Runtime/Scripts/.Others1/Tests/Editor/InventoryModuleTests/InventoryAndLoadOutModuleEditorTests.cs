using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Santelmo.Rinsurv.Backend;

namespace Santelmo.Rinsurv.Editor.Tests
{
    public class InventoryAndLoadoutModuleEditorTests
    {
        [Test]
        public void AddItem()
        {
            var weapon = new Weapon
            {
                Id = "weapon_id_000"
            };

            var itemCounts = new Dictionary<string, int>();
            itemCounts.TryAdd(weapon.Id, 0);
            var initialCount = itemCounts.TryGetValue(weapon.Id, out var result) ? result : 0;
            const int delta = 56;
            var mockInventory = new Mock<IInventoryModule>();
            mockInventory.Setup(x => x.AddItem(It.IsAny<IItem>(), It.IsAny<int>())).Callback<IItem, int>((item, count) =>
            {
                itemCounts.TryAdd(item.Id, 0);
                itemCounts[item.Id] += count;
            });

            var inventory = mockInventory.Object;
            Assert.AreNotEqual(initialCount + delta, itemCounts[weapon.Id]);
            inventory.AddItem(weapon, delta);
            Assert.AreEqual(initialCount + delta, itemCounts[weapon.Id]);
        }

        [Test]
        public void ReduceItem()
        {
            var weapon = new Weapon
            {
                Id = "weapon_id_000"
            };

            var itemCounts = new Dictionary<string, int>
            {
                {weapon.Id, 56},
            };

            const int delta = 5;
            var initialCount = itemCounts.TryGetValue(weapon.Id, out var result) ? result : 0;
            var mockInventory = new Mock<IInventoryModule>();
            mockInventory.Setup(x => x.ReduceItem(It.IsAny<IItem>(), It.IsAny<int>())).Callback<IItem, int>((item, count) => itemCounts[item.Id] -= count);
            var inventory = mockInventory.Object;
            Assert.AreNotEqual(initialCount - delta, itemCounts[weapon.Id]);
            inventory.ReduceItem(weapon, delta);
            Assert.AreEqual(initialCount - delta, itemCounts[weapon.Id]);
        }

        [Test]
        public void UpdateEquipmentProperties()
        {
            var weapon = new Weapon
            {
                Id = "weapon_id_000",
                EquipmentProperties = new EquipmentProperties
                {
                    Level = 0,
                    Quality = 0,
                    Rarity = "Common"
                }
            };

            var equipmentMap = new Dictionary<string, IEquipment>
            {
                {weapon.Id, weapon}
            };

            var newProperties = new EquipmentProperties
            {
                Level = 5,
                Quality = 5,
                Rarity = "Pretty Rare"
            };


            var mockInventory = new Mock<IInventoryModule>();
            mockInventory.Setup(x => x.UpdateEquipmentProperties(It.IsAny<IEquipment>(), It.IsAny<EquipmentProperties>())).Callback<IItem, EquipmentProperties>((item, properties) =>
            {
                equipmentMap[item.Id].EquipmentProperties = properties;
            });

            var inventory = mockInventory.Object;
            Assert.AreNotEqual(newProperties, equipmentMap[weapon.Id].EquipmentProperties);
            inventory.UpdateEquipmentProperties(weapon, newProperties);
            Assert.AreEqual(newProperties, equipmentMap[weapon.Id].EquipmentProperties);
        }

        [Test]
        public void TestLoadout()
        {
            var hero = new Hero
            {
                Id = "hero_id_001",
            };

            var weapon = new Weapon
            {
                Id = "weapon_id_000",
            };

            var emblemDeparture = new EmblemDeparture
            {
                Id = "emblem_departure_id_000",
            };

            var loadoutModule = new LoadoutModule(null, new Mock<ICloudSyncService>().Object);
            var initialLoadout = loadoutModule.GetLoadout(hero);
            loadoutModule.UpdateLoadout(hero, weapon, emblemDeparture, null, null);
            var updatedLoadout = loadoutModule.GetLoadout(hero);
            Assert.AreNotEqual(initialLoadout, updatedLoadout);
        }

        [Test]
        public async Task InventoryWriteAndReadToCloudAsync()
        {
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(x => x.UserId).Returns("player_id_test");
            var cloudSyncService = FirestoreCloudSyncService.Create(mockAuthService.Object);

            var inventory = InventoryModule.Create(null, cloudSyncService);

            var defaultProperties = new EquipmentProperties
            {
                Level = 0,
                Quality = 0,
                Rarity = "Common"
            };

            var startingCurrency = inventory.GetCurrency(Currency.COIN_KEY);

            var weapon = new Weapon
            {
                Id = "weapon_id_000",
                EquipmentProperties = defaultProperties
            };

            var newProperties = new EquipmentProperties
            {
                Level = 5,
                Quality = 5,
                Rarity = "Pretty Rare"
            };

            inventory.AddItem(weapon, 3);
            inventory.UpdateEquipmentProperties(weapon, newProperties);
            inventory.AddCurrency(Currency.COIN_KEY, 5);

            await UniTask.WhenAll(inventory.WriteItemCountsAsync(), inventory.WriteItemLevelsAsync(), inventory.WriteCurrencyCountsAsync());
            await inventory.ReadFromCloudAsync();
            Assert.AreEqual(3, inventory.GetItemCount(weapon));
            Assert.AreNotEqual(defaultProperties.Level, inventory.GetEquipmentProperties(weapon)?.Level);
            Assert.AreNotEqual(defaultProperties.Quality, inventory.GetEquipmentProperties(weapon)?.Quality);
            Assert.AreNotEqual(defaultProperties.Rarity, inventory.GetEquipmentProperties(weapon)?.Rarity);
            Assert.AreNotEqual(startingCurrency, inventory.GetCurrency(Currency.COIN_KEY));
            Assert.AreEqual(weapon.EquipmentProperties.Level, inventory.GetEquipmentProperties(weapon)?.Level);
            Assert.AreEqual(weapon.EquipmentProperties.Quality, inventory.GetEquipmentProperties(weapon)?.Quality);
            Assert.AreEqual(weapon.EquipmentProperties.Rarity, inventory.GetEquipmentProperties(weapon)?.Rarity);
        }

        [Test]
        public async Task LoadoutWriteAndReadToCloudAsync()
        {
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(x => x.UserId).Returns("player_id_test");
            var cloudSyncService = FirestoreCloudSyncService.Create(mockAuthService.Object);

            var hero = new Hero
            {
                Id = "hero_id_001",
            };

            var weapon = new Weapon
            {
                Id = "weapon_id_000",
            };

            var emblemDeparture = new EmblemDeparture
            {
                Id = "emblem_departure_id_000",
            };

            var mockItemList = new Dictionary<string, IItem>
            {
                {hero.Id, hero},
                {weapon.Id, weapon},
                {emblemDeparture.Id, emblemDeparture},
            };

            IItem GetItem(string key) // helper function
            {
                if (key is null)
                {
                    return null;
                }

                return mockItemList.TryGetValue(key, out var value) ? value : null;
            }

            var mockItemDataModule = new Mock<IItemDataModule>();
            var key = "";
            mockItemDataModule.Setup(x => x.GetHeroFromId(It.IsAny<string>())).Callback<string>(r => key = r).Returns(() => GetItem(key) as Hero);
            mockItemDataModule.Setup(x => x.GetWeaponFromId(It.IsAny<string>())).Callback<string>(r => key = r).Returns(() => GetItem(key) as Weapon);
            mockItemDataModule.Setup(x => x.GetEmblemDeparture(It.IsAny<string>())).Callback<string>(r => key = r).Returns(() => GetItem(key) as EmblemDeparture);

            var loadoutModule = new LoadoutModule(mockItemDataModule.Object, cloudSyncService);
            loadoutModule.UpdateLoadout(hero, weapon, null, null, null);
            var beforeUpdate = loadoutModule.GetLoadout(hero);
            await loadoutModule.WriteItemCountsAsync();
            loadoutModule.UpdateLoadout(hero, weapon, emblemDeparture, null, null);
            var afterUpdate = loadoutModule.GetLoadout(hero);
            Assert.AreNotEqual(beforeUpdate.EmblemDeparture, afterUpdate.EmblemDeparture);
            await loadoutModule.ReadFromCloudAsync(); // read from the cloud, which is expected to get our old values
            afterUpdate = loadoutModule.GetLoadout(hero);
            Assert.AreEqual(beforeUpdate.EmblemDeparture, afterUpdate.EmblemDeparture);
        }

        [Test]
        public void UpdateCurrency()
        {
            var currencyModule = InventoryModule.Create(null, null);
            Assert.AreEqual(currencyModule.GetCurrency(Currency.COIN_KEY), 0);
            currencyModule.AddCurrency(Currency.COIN_KEY, 1000);
            Assert.AreEqual(currencyModule.GetCurrency(Currency.COIN_KEY), 1000);
            currencyModule.ReduceCurrency(Currency.COIN_KEY, 1000);
            Assert.AreEqual(currencyModule.GetCurrency(Currency.COIN_KEY), 0);
        }
    }
}
