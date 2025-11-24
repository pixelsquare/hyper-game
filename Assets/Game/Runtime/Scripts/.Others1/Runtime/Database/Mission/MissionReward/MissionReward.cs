using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public class MissionReward : IItem
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

        [BoxGroup("Reward Detail", centerLabel: true)]
        [SerializeField] private int _amount;

        [BoxGroup("Reward Detail", centerLabel: true)]
        [SerializeField] private Sprite _icon;

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name => _name;
        public string Description => _description;
        public string FlavorText => _flavorText;
        public string IconSpriteName => _iconSpriteName;
        public string ItemTypeName => "Reward";
        public int Amount => _amount;
        public Sprite Icon => _icon;

        private void GenerateNewId()
        {
            _id = GameUtil.NewGuid();
        }
    }
}
