using UnityEngine;

namespace Santelmo.Rinsurv
{
    public interface IHex
    {
        public float Duration { get; }
        public Transform Transform { get; }
        public long Id { get; }

        public void OnHexActivate();        
    }
}
