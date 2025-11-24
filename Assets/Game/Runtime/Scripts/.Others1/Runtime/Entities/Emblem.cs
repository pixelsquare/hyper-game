using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public abstract class Emblem : IEquipment, INature
    {
        [BoxGroup("Information", centerLabel: true)]
        [InlineButton("GenerateNewId", "Reset")]
        [SerializeField] protected string _id;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] protected string _name;

        [BoxGroup("Information", centerLabel: true)]
        [MultiLineProperty(5)]
        [SerializeField] protected string _description;

        [BoxGroup("Information", centerLabel: true)]
        [MultiLineProperty(5)]
        [SerializeField] private string _flavorText;

        [AssetReference(typeof(Sprite))]
        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] protected string _iconSpriteName;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] protected Sprite _iconTypeSprite;

        [BoxGroup("Properties", centerLabel: true)]
        [SerializeField] protected Nature _nature;

        [BoxGroup("Properties", centerLabel: true)]
        [SerializeField] protected EquipmentRarity _rarity = EquipmentRarity.B;

        [BoxGroup("Bonus Properties", centerLabel: true)][HideLabel]
        [SerializeField] protected HeroStats _heroStats;

        protected EquipmentProperties _equipmentProperties = new();

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name => _name;
        public string Description => _description;
        public string FlavorText => _flavorText;
        public string IconSpriteName => _iconSpriteName;
        public Sprite IconTypeSprite => _iconTypeSprite;
        public virtual string ItemTypeName => "Emblem";

        public Nature Nature
        {
            get => _nature;
            set => _nature = value;
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

        public HeroStats HeroStats
        {
            get => _heroStats;
            set => _heroStats = value;
        }

        public void UpgradeQuality()
        {
            _equipmentProperties.Quality = Mathf.Clamp(EquipmentProperties.Quality + 1, (int)EquipmentQuality.Common, (int)EquipmentQuality.Legendary + 1);
        }

        public void UpgradeLevel()
        {
            _equipmentProperties.Level++;
        }

        private void GenerateNewId()
        {
            _id = GameUtil.NewGuid();
        }
    }

    [Serializable]
    public class EmblemDeparture : Emblem, ICloneable
    {
        [BoxGroup("Modifiers", centerLabel: true)]
        [SerializeField] private float _moveSpeed;

        [BoxGroup("Modifiers", centerLabel: true)]
        [SerializeField] private float _evasionChance;

        [BoxGroup("Modifiers", centerLabel: true)]
        [SerializeField] private float _cooldownModifier;

        public EmblemDeparture()
        {
        }

        public EmblemDeparture(EmblemDeparture baseItem)
        {
            _name = baseItem.Name;
            _id = baseItem.Id;
            _iconSpriteName = baseItem.IconSpriteName;
            _description = baseItem.Description;
            _equipmentProperties = baseItem.EquipmentProperties;
            _moveSpeed = baseItem.MoveSpeed;
            _evasionChance = baseItem.EvasionChance;
            _cooldownModifier = baseItem.CooldownModifier;
        }

        public override string ItemTypeName => "Emblem of Departure";
        public float MoveSpeed => _moveSpeed;
        public float EvasionChance => _evasionChance;
        public float CooldownModifier => _cooldownModifier;

        public object Clone()
        {
            return new EmblemDeparture(this);
        }

        public static EmblemDeparture operator-(EmblemDeparture a, EmblemDeparture b)
        {
            var emblem = new EmblemDeparture(a);
            emblem._moveSpeed -= b._moveSpeed;
            emblem._evasionChance -= b._evasionChance;
            emblem._cooldownModifier -= b._cooldownModifier;
            return emblem;
        }
    }

    [Serializable]
    public class EmblemPursuit : Emblem, ICloneable
    {
        [BoxGroup("Modifiers", centerLabel: true)]
        [SerializeField] private float _criticalChance;

        [BoxGroup("Modifiers", centerLabel: true)]
        [SerializeField] private float _criticalDamage;

        [BoxGroup("Modifiers", centerLabel: true)]
        [SerializeField] private float _lootDistance;

        public EmblemPursuit()
        {
        }

        public EmblemPursuit(EmblemPursuit baseItem)
        {
            _name = baseItem.Name;
            _id = baseItem.Id;
            _iconSpriteName = baseItem.IconSpriteName;
            _description = baseItem.Description;
            _equipmentProperties = baseItem.EquipmentProperties;
            _criticalChance = baseItem.CriticalChance;
            _criticalDamage = baseItem.CriticalDamage;
            _lootDistance = baseItem.LootDistance;
        }

        public override string ItemTypeName => "Emblem of Pursuit";
        public float CriticalChance => _criticalChance;
        public float CriticalDamage => _criticalDamage;
        public float LootDistance => _lootDistance;

        public object Clone()
        {
            return new EmblemPursuit(this);
        }

        public static EmblemPursuit operator-(EmblemPursuit a, EmblemPursuit b)
        {
            var emblem = new EmblemPursuit(a);
            emblem._criticalChance -= b._criticalChance;
            emblem._criticalDamage -= b._criticalDamage;
            emblem._lootDistance -= b._lootDistance;
            return emblem;
        }
    }

    [Serializable]
    public class EmblemChange : Emblem, ICloneable
    {
        [BoxGroup("Modifiers", centerLabel: true)]
        [SerializeField] private float _health;

        [BoxGroup("Modifiers", centerLabel: true)]
        [SerializeField] private float _armor;

        public EmblemChange()
        {
        }

        public EmblemChange(EmblemChange baseItem)
        {
            _name = baseItem.Name;
            _id = baseItem.Id;
            _iconSpriteName = baseItem.IconSpriteName;
            _description = baseItem.Description;
            _equipmentProperties = baseItem.EquipmentProperties;
            _health = baseItem.Health;
            _armor = baseItem.Armor;
        }

        public override string ItemTypeName => "Emblem of Change";
        public float Health => _health;
        public float Armor => _armor;

        public object Clone()
        {
            return new EmblemChange(this);
        }

        public static EmblemChange operator-(EmblemChange a, EmblemChange b)
        {
            var emblem = new EmblemChange(a);
            emblem._health -= b._health;
            emblem._armor -= b._armor;
            return emblem;
        }
    }
}
