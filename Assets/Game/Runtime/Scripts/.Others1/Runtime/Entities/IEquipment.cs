using System;
using Firebase.Firestore;

namespace Santelmo.Rinsurv
{
    public interface IEquipment : IItem
    {
        public EquipmentRarity Rarity { get; set; }
        public EquipmentProperties EquipmentProperties { get; set; }
        public HeroStats HeroStats { get; set; }

        public void UpgradeQuality();
        public void UpgradeLevel();
    }

    [Serializable][FirestoreData]
    public class EquipmentProperties
    {
        public EquipmentProperties()
        {
        }

        public EquipmentProperties(EquipmentProperties props)
        {
            Level = props.Level;
            Quality = props.Quality;
            Rarity = props.Rarity;
        }

        [FirestoreProperty]
        public int Level { get; set; }
        [FirestoreProperty]
        public int Quality { get; set; }
        [FirestoreProperty]
        public string Rarity { get; set; }
    }

    public enum EquipmentQuality
    {
        Common = 0,
        Good = 1,
        Great = 2,
        Excellent = 3,
        Legendary = 4
    }

    public enum EquipmentRarity
    {
        None = 0,
        Any = ~None,
        B = 1 << 0,
        A = 1 << 1,
        S = 1 << 2,
        SS = 1 << 3,
        SSS = 1 << 4
    }
}
