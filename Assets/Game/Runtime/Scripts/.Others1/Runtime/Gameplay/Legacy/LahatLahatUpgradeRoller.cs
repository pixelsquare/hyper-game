using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class LahatLahatUpgradeRoller : MonoBehaviour
    {
        [SerializeField] private GameObject[] _sampleLahatLahat;
        [SerializeField] private int _rollAmount = 3;

        private const string OnRollLegacyEvent = GameEvents.Gameplay.OnRollLegacy;

        private int RollIds(int amount, out IEnumerable<int> ids)
        {
            var length = _sampleLahatLahat.Length;
            var set = new int[length];
            for (var i = 0; i < length; i++)
            {
                set[i] = i;
            }
            
            var random = new System.Random();
            var takeCount = length < amount ? length : amount; 

            ids = set.OrderBy(x => random.Next())
                     .Take(takeCount);
            return takeCount;
        }

        private int RollLahatLahat(out GameObject[] prefabs)
        {
            var count = RollIds(_rollAmount, out var ids);
            prefabs = new GameObject[count];

            var i = 0;
            foreach (var id in ids)
            {
                prefabs[i++] = _sampleLahatLahat[id];
            }

            return count;
        }

        private int RollSelection(out LegacyUpgradeSelection[] selection)
        {
            var count = RollLahatLahat(out var prefabs);
            selection = new LegacyUpgradeSelection[count];

            for (var i = 0; i < count; i++)
            {
                var prefab = prefabs[i];

                selection[i] = new LegacyUpgradeSelection
                {
                    _isNew = true,
                    _prefab = prefab,
                    _legacy = prefab.GetComponent<ILegacy>()
                };
            }

            return count;
        }

        private void OnMinibossKill(IMessage message)
        {
            BroadcastSelection();
        }

        [ContextMenu("Broadcast Selection")]
        private void BroadcastSelection()
        {
            RollSelection(out var selection);
            Dispatcher.SendMessageData(OnRollLegacyEvent, selection);
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(GameEvents.Gameplay.OnMinibossWaveFinish, OnMinibossKill);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnMinibossWaveFinish, OnMinibossKill, true);
        }
    }
}
