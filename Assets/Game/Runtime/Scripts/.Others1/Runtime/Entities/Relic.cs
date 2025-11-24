using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public abstract class Relic : IItem, INature
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

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name => _name;
        public string Description => _description;
        public string FlavorText => _flavorText;
        public string IconSpriteName => _iconSpriteName;
        public string ItemTypeName => "Relic";
        public Nature Nature { get; set; }

        private void GenerateNewId()
        {
            _id = GameUtil.NewGuid();
        }
    }

    [Serializable]
    public class RelicVictory : Relic
    {
    }

    [Serializable]
    public class RelicDeparture : Relic
    {
    }

    [Serializable]
    public class RelicPursuit : Relic
    {
    }

    [Serializable]
    public class RelicChange : Relic
    {
    }
}
