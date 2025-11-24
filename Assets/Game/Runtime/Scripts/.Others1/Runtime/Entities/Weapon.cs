using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public class Weapon : IEquipment, INature, ICloneable
    {
        [BoxGroup("Information", centerLabel: true)]
        [InlineButton("GenerateNewId", "Reset")]
        [SerializeField] private string _id;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private string _name;

        [BoxGroup("Information", centerLabel: true)]
        [MultiLineProperty(5)]
        [SerializeField] private string _description;

        [BoxGroup("Information", centerLabel: true)]
        [MultiLineProperty(5)]
        [SerializeField] private string _flavorText;

        [AssetReference(typeof(Sprite))]
        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private string _iconSpriteName;

        [BoxGroup("Properties", centerLabel: true)]
        [SerializeField] protected EquipmentRarity _rarity = EquipmentRarity.B;

        [BoxGroup("Bonus Properties", centerLabel: true)][HideLabel]
        [SerializeField] protected HeroStats _heroStats;

        protected EquipmentProperties _equipmentProperties;

        public Weapon()
        {
        }

        public Weapon(Weapon baseItem)
        {
            _name = baseItem.Name;
            _id = baseItem.Id;
            _iconSpriteName = baseItem.IconSpriteName;
            _description = baseItem.Description;
            _flavorText = baseItem.FlavorText;
            _equipmentProperties = baseItem.EquipmentProperties;
            _equipmentProperties.Rarity = baseItem.EquipmentProperties.Rarity;
            Attack = baseItem.Attack;
            Type = baseItem.Type;
        }

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name => _name;
        public string Description => _description;
        public string FlavorText => _flavorText;
        public string IconSpriteName => _iconSpriteName;
        public string ItemTypeName => "Weapon upgrade material";

        // TODO: Maybe store in some weapon stats?
        public Nature Nature
        {
            get => Nature.Victory;
            set => throw new NotImplementedException();
        }

        public EquipmentRarity Rarity
        {
            get => _rarity;
            set => _rarity = value;
        }

        public EquipmentProperties EquipmentProperties
        {
            get => _equipmentProperties;
            set => _equipmentProperties = value;
        }

        public float Attack { get; }

        public string Type { get; }

        public HeroStats HeroStats
        {
            get => _heroStats;
            set => _heroStats = value;
        }

        public void UpgradeQuality()
        {
            _equipmentProperties.Quality = Mathf.Clamp(_equipmentProperties.Quality + 1, (int)EquipmentQuality.Common, (int)EquipmentQuality.Legendary + 1);
        }

        public void UpgradeLevel()
        {
            _equipmentProperties.Level++;
        }

        public object Clone()
        {
            return new Weapon(this);
        }

        private void GenerateNewId()
        {
            _id = GameUtil.NewGuid();
        }
    }
}
