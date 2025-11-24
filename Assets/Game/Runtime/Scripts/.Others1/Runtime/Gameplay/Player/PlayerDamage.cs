using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Santelmo.Rinsurv
{
    public class PlayerDamage : MonoBehaviour
    {
        [SerializeField] private float _numOfInvincibilityFlashes = 10f;
        [SerializeField] private float _flashDuration = 0.07f;
        [SerializeField] private SpriteRenderer _visual;
        [SerializeField] private Color _originColor;
        [SerializeField] private Color _flashColor;

        private Rigidbody2D _rigidbody;
        private IHittable _playerHittable;
        private PlayerStats _playerStats;

        private void OnHit(Transform hitTransform)
        {
            if (!_playerHittable.IsHittable
                || !hitTransform.TryGetComponent<UnitEnemy>(out var unitEnemy))
            {
                return;
            }

            var enemyDamage = (uint) unitEnemy.Stats._attack;
            var totalDamage = _playerStats.CalcTotalReceivedDamage(enemyDamage);
            _playerHittable.Hit(totalDamage, hitTransform.position);            
        }

        private IEnumerator InvincibilityEffect(IHittable hittable)
        {
            var currentFlash = 0;
            _rigidbody.simulated = hittable.IsHittable = false;
            
            while (currentFlash < _numOfInvincibilityFlashes)
            {
                _visual.color = _flashColor;
                yield return new WaitForSeconds(_flashDuration);

                _visual.color = _originColor;
                yield return new WaitForSeconds(_flashDuration);

                currentFlash++;
            }

            _rigidbody.simulated = hittable.IsHittable = true;
        }

        private void OnUnitDamage(uint damage, uint currenthealth, uint maxhealth, Vector3 origin)
        {
            if (_playerHittable.IsHittable)
            {
                StartCoroutine(InvincibilityEffect(_playerHittable));
            }
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerHittable = GetComponent<IHittable>();
            _playerStats = GetComponent<UnitPlayer>().Stats;
        }

        private void Start()
        {
            Assert.IsNotNull(_visual, "_visual is null, please attach a sprite renderer!");
        }

        private void OnEnable()
        {
            GetComponent<IDamageEvent>().OnUnitDamage += OnUnitDamage;
        }

        private void OnDisable()
        {
            GetComponent<IDamageEvent>().OnUnitDamage -= OnUnitDamage;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            OnHit(col.transform);
        }
    }
}
