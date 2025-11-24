using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public class Hero : IItem
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
        [SerializeField] private string _avatarSpriteName;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private VideoClip _attackPreviewClip;

        [BoxGroup("Stats", centerLabel: true)][HideLabel]
        [SerializeField] private HeroStats _heroStats;

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name => _name;
        public string Description => _description;
        public string FlavorText => _flavorText;
        public string IconSpriteName => _avatarSpriteName;
        public VideoClip AttackPreviewClip => _attackPreviewClip;
        public string ItemTypeName => "Hero";
        public string[] ItemEquipped { get; set; } = new string[4];
        public HeroStats HeroStats => _heroStats;

        private void GenerateNewId()
        {
            _id = GameUtil.NewGuid();
        }
    }
}
