using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class PickupField : MonoBehaviour
    {
        [SerializeField] private float _range = 1f;
        [SerializeField] private float _radius = .25f;

        public float Range => _range;
        public float Radius => _radius;

        private LazyInject<UnitPlayer> _unitPlayer;

        [Inject]
        private void Construct(LazyInject<UnitPlayer> unitPlayer)
        {
            _unitPlayer = unitPlayer;
        }

        private void OnModifyStats(PlayerStats stats)
        {
            _range = stats._lootDistance;
        }

        private void OnEnable()
        {
            _unitPlayer.Value.OnModifyStatsEvent += OnModifyStats;
        }

        private void OnDisable()
        {
            _unitPlayer.Value.OnModifyStatsEvent -= OnModifyStats;
        }

        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, _radius);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position, _range);
        }
    }
}
