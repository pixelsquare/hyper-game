using System.Collections.Generic;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public delegate void OnHexApply(IHex hex);
    public delegate void OnHexRemove(IHex hex);

    public class HexSystem : MonoBehaviour
    {
        public OnHexApply OnHexApplyEvent;
        public OnHexRemove OnHexRemoveEvent;

        private Dictionary<long, HexState> _activeHexes;
        private Stack<long> _inactiveHexes;

        private class HexState
        {
            public IHex _hex;
            public float _duration;
        }

        private void Update()
        {
            foreach (var pair in _activeHexes)
            {
                var hex = pair.Value._hex;

                if (pair.Value._duration > 0f)
                {
                    _activeHexes[pair.Key]._duration -= Time.deltaTime;

                    if (hex is IHexUpdate hexUpdate)
                    {
                        hexUpdate.OnHexUpdate();
                    }
                }
                else
                {
                    if (hex is IHexRemove hexRemove)
                    {
                        hexRemove.OnHexRemove();
                    }

                    _inactiveHexes.Push(pair.Key);
                }
            }
        }

        private void LateUpdate()
        {
            while (_inactiveHexes.TryPop(out var hex))
            {
                _activeHexes.Remove(hex);
            }
        }

        private void OnApply<T>(T hex) where T : IHex
        {
            var id = hex.Id;

            if (_activeHexes.TryGetValue(id, out var oldHexState))
            {
                if (oldHexState._hex is IHexRefresh oldHexRefresh)
                {
                    oldHexRefresh.OnHexRefresh(hex);
                }

                _activeHexes[hex.Id]._duration = hex.Duration;
            }
            else if (hex is IHexApply hexApply)
            {
                hexApply.OnHexApply();
                _activeHexes.Add(id, new HexState
                {
                    _hex = hex,
                    _duration = hex.Duration
                });
            }
        }

        private void OnRemove(IHex hex)
        {
            if (hex is IHexRemove hexRemove)
            {
                hexRemove.OnHexRemove();
            }

            _activeHexes.Remove(hex.Id);
        }

        private void Awake()
        {
            _activeHexes = new Dictionary<long, HexState>();
            _inactiveHexes = new Stack<long>();
        }

        private void OnEnable()
        {
            OnHexApplyEvent += OnApply;
            OnHexRemoveEvent += OnRemove;
        }

        private void OnDisable()
        {
            OnHexApplyEvent -= OnApply;
            OnHexRemoveEvent -= OnRemove;
        }

        public bool IsEnemyHexed<T>(Transform hitTransform) where T : IHex
        {
            foreach (var hexInstance in _activeHexes.Values)
            {
                if (!(hexInstance._hex is T))
                {
                    continue;
                }

                var hex = (T)hexInstance._hex;
                if (hex.Transform == hitTransform)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
