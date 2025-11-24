using System.Collections;
using NUnit.Framework;

namespace Santelmo.Rinsurv.Editor.Tests
{
    public class EquipmentModuleEditorTests
    {
        private static Weapon GetWeapon(EquipmentQuality quality) => new()
        {
            EquipmentProperties = new EquipmentProperties
            {
                Quality = (int) quality
            }
        };
        
        private static Weapon GetWeapon(int level) => new()
        {
            EquipmentProperties = new EquipmentProperties
            {
                Level = level,
            },
            Id = $"weaponWithLevel{level}",
        };

        private static EmblemDeparture GetEmblemDeparture(EquipmentQuality quality) => new()
        {
            EquipmentProperties = new EquipmentProperties
            {
                Quality = (int) quality
            }
        };
        
        public static IEnumerable EquipmentMergingInputOutput
        {
            get
            {
                yield return new TestCaseData(GetWeapon(EquipmentQuality.Common), GetWeapon(EquipmentQuality.Common), GetWeapon(EquipmentQuality.Good)).SetName("Common Weapon + Common Weapon");
                yield return new TestCaseData(GetWeapon(EquipmentQuality.Common), GetEmblemDeparture(EquipmentQuality.Common), null).SetName("Common Weapon + Common EmblemDeparture");
                yield return new TestCaseData(GetWeapon(EquipmentQuality.Common), GetWeapon(EquipmentQuality.Good), null).SetName("Common Weapon + Good Weapon");
                yield return new TestCaseData(GetWeapon(EquipmentQuality.Good), GetWeapon(EquipmentQuality.Good), GetWeapon(EquipmentQuality.Great)).SetName("Good Weapon + Good Weapon");
                yield return new TestCaseData(GetWeapon(EquipmentQuality.Good), GetEmblemDeparture(EquipmentQuality.Good), GetWeapon(EquipmentQuality.Great)).SetName("Good Weapon + Good EmblemDeparture");
                yield return new TestCaseData(GetWeapon(EquipmentQuality.Great), GetWeapon(EquipmentQuality.Great), GetWeapon(EquipmentQuality.Excellent)).SetName("Great Weapon + Great Weapon");
                yield return new TestCaseData(GetWeapon(EquipmentQuality.Great), GetEmblemDeparture(EquipmentQuality.Great), GetWeapon(EquipmentQuality.Excellent)).SetName("Great Weapon + Great EmblemDeparture");
                yield return new TestCaseData(GetEmblemDeparture(EquipmentQuality.Great), GetWeapon(EquipmentQuality.Great), GetEmblemDeparture(EquipmentQuality.Excellent)).SetName("Great EmblemDeparture + Great Weapon");
                yield return new TestCaseData(GetEmblemDeparture(EquipmentQuality.Excellent), GetWeapon(EquipmentQuality.Excellent), null).SetName("Excellent EmblemDeparture + Excellent Weapon");
                yield return new TestCaseData(GetWeapon(EquipmentQuality.Excellent), GetWeapon(EquipmentQuality.Excellent), GetWeapon(EquipmentQuality.Legendary)).SetName("Excellent Weapon + Excellent Weapon");
                yield return new TestCaseData(GetWeapon(EquipmentQuality.Legendary), GetWeapon(EquipmentQuality.Legendary), null).SetName("Legendary Weapon + Legendary Weapon");
            }
        }

        [TestCaseSource(nameof(EquipmentMergingInputOutput))]
        public void CanMergeEquipment(IEquipment baseEquipment, IEquipment addOnEquipment, IEquipment expectedOutput)
        {
            var equipmentModule = new EquipmentModule(null, null);

            var resultingEquipment = equipmentModule.CanMergeEquipment(baseEquipment, addOnEquipment);
            if (expectedOutput is null)
            {
                Assert.IsNull(resultingEquipment);
            }
            else
            {
                Assert.AreEqual(expectedOutput.EquipmentProperties.Quality, resultingEquipment.EquipmentProperties.Quality);
            }
        }
        
        public static IEnumerable EquipmentLevelInputOutput
        {
            get
            {
                yield return new TestCaseData(GetWeapon(0), 1000, GetWeapon(1)).SetName("Level 0 to 1, 1000 coins");
                yield return new TestCaseData(GetWeapon(4), 10000000, GetWeapon(5)).SetName("Level 4 to 5, 10000000 coins");
                yield return new TestCaseData(GetWeapon(0), 500, null).SetName("Level 0 to null, 500 coins");
                yield return new TestCaseData(GetWeapon(10), 100000000, null).SetName("Level 10 to null, 100000000 coins");
            }
        }

        [TestCaseSource(nameof(EquipmentLevelInputOutput))]
        public void CanLevelUpEquipment(IEquipment baseEquipment, int coins, IEquipment expectedOutput)
        {
            var inventoryModule = InventoryModule.Create(null, null);
            var equipmentModule = new EquipmentModule(inventoryModule, inventoryModule);
            inventoryModule.AddCurrency(Currency.COIN_KEY, coins);
            inventoryModule.AddItem(baseEquipment, 1);

            var startingCoins = inventoryModule.GetCurrency(Currency.COIN_KEY);
            var potentialEquipmentAndCost = equipmentModule.CanLevelUpEquipment(baseEquipment);

            if (expectedOutput is null)
            {
                Assert.IsNull(potentialEquipmentAndCost.equipment);
                return;
            }

            Assert.IsTrue(potentialEquipmentAndCost.cost <= inventoryModule.GetCurrency(Currency.COIN_KEY));
            Assert.AreEqual(expectedOutput.EquipmentProperties.Level, potentialEquipmentAndCost.equipment.EquipmentProperties.Level);
            var actualEquipment = equipmentModule.LevelUpEquipment(baseEquipment);
            Assert.NotNull(actualEquipment);
            Assert.AreEqual(expectedOutput.EquipmentProperties.Level, actualEquipment.EquipmentProperties.Level);
            Assert.AreEqual(startingCoins - potentialEquipmentAndCost.cost, inventoryModule.GetCurrency(Currency.COIN_KEY));
            Assert.AreEqual(1, inventoryModule.GetItemCount(actualEquipment));
        }
    }
}
