using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class Shard : IItem
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

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name => _name;
        public string Description => _description;
        public string FlavorText => _flavorText;
        public string IconSpriteName => _iconSpriteName;
        public string ItemTypeName => "Hero Shard";

        private void GenerateNewId()
        {
            _id = GameUtil.NewGuid();
        }
    }
}
