using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public struct LegacyUpgradeSelection
    {
        public bool _isNew;
        public GameObject _prefab;
        public ILegacy _legacy;
    }

    public class LegacyUpgradeRoller : MonoBehaviour
    {
        [SerializeField] private LegacySlotTracker _legacySlotTracker;
        [SerializeField] private LegacyLayerConfig[] _legacyLayerConfigs;
        [SerializeField] private int _amount = 3;

        private const string OnRollLegacyEvent = GameEvents.Gameplay.OnRollLegacy;

        private LazyInject<ExpTracker> _expTracker;

        [Inject]
        private void Construct(LazyInject<ExpTracker> expTracker)
        {
            _expTracker = expTracker;
        }

        private void OnLevelUp(uint level)
        {
            var legacySelection = RollSelection(_amount);
            Dispatcher.SendMessageData(OnRollLegacyEvent, legacySelection);
        }

        private LegacyUpgradeSelection[] RollSelection(int amount)
        {
            var selection = new List<LegacyUpgradeSelection>();
            var slots = new Stack<LegacySlot>(RollSlots());
            var legacySelection = RecurseSelection(selection, slots, amount);
            return legacySelection;
        }

        private LegacyUpgradeSelection[] RecurseSelection(List<LegacyUpgradeSelection> selection, Stack<LegacySlot> slots, int amount, LegacySlot prevSlot = LegacySlot.None)
        {
            var slot = prevSlot;
            var hasSlot = (prevSlot != LegacySlot.None) || slots.TryPop(out slot);

            if (!hasSlot || amount < 1)
            {
                return selection.ToArray();
            }

            var legacyLayer = RollLayer();

            if (slot == LegacySlot.Passive)
            {
                if (!legacyLayer.TryRollPassive(out var passivePrefab)
                    || !passivePrefab.TryGetComponent<ILegacy>(out var legacyPassive))
                { // this Legacy is invalid; reroll for a new Passive
                    return RecurseSelection(selection, slots, amount, LegacySlot.Passive);
                }

                if (!_legacySlotTracker.LegacyPassives.TryGetValue(legacyPassive.LegacyId, out var legacy))
                { // this Legacy has not been obtained; if legacy requirement is met add it to the selection
                    if (HasRequiredPassiveLegacy(legacyPassive))
                    {
                        selection.Add(new LegacyUpgradeSelection
                        {
                            _isNew = true,
                            _legacy = legacyPassive,
                            _prefab = passivePrefab
                        });

                        --amount;
                    }
                }
                else if (legacy.CurrentLevel < legacy.MaxLevel)
                { // this Legacy has been obtained, but can still be upgraded; add it to the selection
                    selection.Add(new LegacyUpgradeSelection
                    {
                        _isNew = false,
                        _legacy = legacy
                    });
                    --amount;
                }
                else
                { // this Legacy has been obtained, but cannot be upgraded; reroll for a new Passive legacy
                    return RecurseSelection(selection, slots, amount, LegacySlot.Passive);
                }
            }
            else if (_legacySlotTracker.LegacyActives.TryGetValue(slot, out var legacy)
                     && legacy.CurrentLevel < legacy.MaxLevel)
            { // this Active Legacy has been obtained, and can be upgraded; add it to the selection
                selection.Add(new LegacyUpgradeSelection
                {
                    _isNew = false,
                    _legacy = legacy
                });

                --amount;
            }
            else if (legacyLayer.TryGetActive(slot, out var legacyPrefab))
            { // this Active Legacy has not been obtained; add it to the selection

                var _legacy = legacyPrefab.GetComponent<ILegacy>();

                if (!HasRequiredPassiveLegacy(_legacy))
                {
                    return RecurseSelection(selection, slots, amount);
                }

                selection.Add(new LegacyUpgradeSelection
                {
                    _isNew = true,
                    _prefab = legacyPrefab,
                    _legacy = _legacy
                });

                --amount;
            }

            return RecurseSelection(selection, slots, amount);
        }

        private LegacyLayerConfig RollLayer()
        {
            var idx = Random.Range(0, _legacyLayerConfigs.Length);
            var legacyLayer = _legacyLayerConfigs[idx];
            return legacyLayer;
        }

        private IEnumerable<LegacySlot> RollSlots()
        {
            var random = new System.Random();
            var randomSet = new[] { LegacySlot.Strike, LegacySlot.Cast, LegacySlot.Move, LegacySlot.Deflect, LegacySlot.Smite, LegacySlot.Passive }
                   .OrderBy(x => random.Next());
            return randomSet;
        }

        private bool HasRequiredPassiveLegacy(ILegacy legacy)
        {
            if ((legacy.RequiredLegacies == null))
            {
                return true;
            }

            // if legacy has no requirement
            if (legacy.RequiredLegacies.Length <= 0)
            {
                return true;
            }

            foreach (var reqLeg in legacy.RequiredLegacies)
            {
                if(_legacySlotTracker.LegacyPassives.ContainsKey(reqLeg))
                {
                    continue;
                }
                
                if(_legacySlotTracker.LegacyActives.Values.Any(x => x.LegacyId.Equals(reqLeg)))
                {
                    continue;
                }
                return false;
            }
               
            return true;
        }

        private void OnEnable()
        {
            _expTracker.Value.OnLevelUpEvent += OnLevelUp;
        }

        private void OnDisable()
        {
            _expTracker.Value.OnLevelUpEvent -= OnLevelUp;
        }
    }
}
