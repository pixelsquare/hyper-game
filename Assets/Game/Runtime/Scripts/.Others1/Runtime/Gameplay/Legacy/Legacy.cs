using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public abstract class Legacy : MonoBehaviour, ILegacy
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
        [SerializeField] private LegacySlot _legacySlot;
        
        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private LegacyLayer _legacyLayer;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private string[] _requiredLegacies;

        [BoxGroup("Information", centerLabel: true)]
        [AssetReference(typeof(Sprite))]
        [SerializeField] private string _iconName;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private Sprite _legacyTypeSprite;

        public virtual LegacyLayer LegacyLayer => _legacyLayer;
        public virtual string LegacyId => _id;
        public virtual string LegacyName => _name;
        public virtual string LegacyDescription => _description;
        public virtual string LegacyIconName => _iconName;
        public virtual Sprite LegacyTypeSprite => _legacyTypeSprite;

        public virtual LegacySlot LegacySlot => _legacySlot;
        public virtual uint MaxLevel { get; }
        public virtual uint CurrentLevel => _currentLevel;

        public string[] RequiredLegacies => _requiredLegacies;

        protected uint _currentLevel = 0;

        public abstract void LevelUp();

        private void GenerateNewId()
        {
            _id = GameUtil.NewGuid();
        }

        protected virtual void Reset()
        {
            _id = GameUtil.NewGuid();
        }

        protected virtual void OnValidate()
        {
            if (GameUtil.DidDuplicate(Event.current))
            {
                _id = GameUtil.NewGuid();
            }
        }
    }
}
