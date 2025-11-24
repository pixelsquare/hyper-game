using UnityEngine;

namespace Santelmo.Rinsurv
{
    public abstract class BaseConfig<T> : ScriptableObject where T : class, IAsset
    {
        public abstract T Config { get; }

        protected virtual void Reset()
        {
            Config.Id = GameUtil.NewGuid();
        }

        protected virtual void OnValidate()
        {
            if (GameUtil.DidDuplicate(Event.current))
            {
                Config.Id = GameUtil.NewGuid();
            }
        }
    }
}
