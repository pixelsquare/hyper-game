using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class ItemComparisonPopup : BasePopup
    {
        [SerializeField] private ItemDetailAdvanced _itemDetailAdvanced;
        [SerializeField] private ItemPassivePopulator _itemStatUpgradePopulator;
        [SerializeField] private RinawaButton _equipButton;

        // Hard reference. Ideally must be stored somewhere accessible by the game and UI.
        // Always sync with `LegacyUpgradeRoller`.
        [SerializeField] private LegacyLayerConfig[] _legacyLayerConfigs;
        [SerializeField] private Sprite _legacyUnlockedSprite;

        public new UniTask<IItem> Task { get; protected set; }

        private CompositeDisposable _compositeDisposable = new();
        private bool _didEquip;

        public ItemComparisonPopup Setup(IItem currentItem, IItem selectedItem)
        {
            InitializeItemDetail(currentItem, selectedItem);
            InitializePassiveLayers();

            Task = UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.WaitUntil(() => _isClosed);
                return _didEquip ? selectedItem : currentItem;
            });
            return this;
        }

        private void InitializeItemDetail(IItem currentItem, IItem selectedItem)
        {
            if (currentItem != null)
            {
                var itemStats = new List<ItemStats>();
                CompareItemStatsNonAlloc(currentItem, selectedItem, itemStats);
                _itemDetailAdvanced.Setup(selectedItem, itemStats);
            }
            else
            {
                _itemDetailAdvanced.Setup(selectedItem);
            }
        }

        private void InitializePassiveLayers()
        {
            var layerLen = _legacyLayerConfigs.Length;
            var layerChance = 1f / layerLen; // Always equally divided chance for the legacy layers.

            var unlockedPassives = Enumerable.Range(0, layerLen).Select(x =>
            {
                var layer = _legacyLayerConfigs[x];
                return new ItemStats(_legacyUnlockedSprite, GameUtil.FormatString($"{layer.name} Legacy Chance", length: 35), layerChance * 100, 0);
            });

            _itemStatUpgradePopulator.Setup(unlockedPassives);
        }

        private void CompareItemStatsNonAlloc(IItem currentItem, IItem selectedItem, List<ItemStats> itemStats)
        {
            if (itemStats == null)
            {
                throw new NullReferenceException($"{nameof(itemStats)}");
            }

            var currentItemType = currentItem.GetType();
            var selectedItemType = selectedItem.GetType();

            if (currentItemType != selectedItemType)
            {
                GameUtil.GetItemStatsNonAlloc(selectedItem, itemStats);
                return;
            }

            // TODO: Beware of changing the length of `GameUtil.FormatString`
            // string format consistency with `GameUtil.GetItemStatsNonAlloc`.
            switch (currentItem)
            {
                case EmblemChange emblemChange:
                    var newEmblemChange = selectedItem as EmblemChange;
                    var emblemChangeDiff = newEmblemChange - emblemChange;
                    itemStats.Add(new ItemStats(null, GameUtil.FormatString("Health"), emblemChange.Health, emblemChangeDiff.Health));
                    itemStats.Add(new ItemStats(null, GameUtil.FormatString("Armor"), emblemChange.Armor, emblemChangeDiff.Armor));
                    break;
                case EmblemDeparture emblemDeparture:
                    var newEmblemDeparture = selectedItem as EmblemDeparture;
                    var emblemDepatureDiff = newEmblemDeparture - emblemDeparture;
                    itemStats.Add(new ItemStats(null, GameUtil.FormatString("Move Speed", length: 37), emblemDeparture.MoveSpeed, emblemDepatureDiff.MoveSpeed));
                    itemStats.Add(new ItemStats(null, GameUtil.FormatString("Evasion Chance", length: 34), emblemDeparture.EvasionChance, emblemDepatureDiff.EvasionChance));
                    itemStats.Add(new ItemStats(null, GameUtil.FormatString("Cooldown Modifier"), emblemDeparture.CooldownModifier, emblemDepatureDiff.CooldownModifier));
                    break;
                case EmblemPursuit emblemPursuit:
                    var newEmblemPursuit = selectedItem as EmblemPursuit;
                    var emblemPursuitDiff = newEmblemPursuit - emblemPursuit;
                    itemStats.Add(new ItemStats(null, GameUtil.FormatString("Critical Chance", length: 31), emblemPursuit.CriticalChance, emblemPursuitDiff.CriticalChance));
                    itemStats.Add(new ItemStats(null, GameUtil.FormatString("Critical Damage"), emblemPursuit.CriticalDamage, emblemPursuitDiff.CriticalDamage));
                    itemStats.Add(new ItemStats(null, GameUtil.FormatString("Loot Distance", length: 32), emblemPursuit.LootDistance, emblemPursuitDiff.LootDistance));
                    break;
            }
        }

        private void OnEnable()
        {
            _equipButton.OnClickAsObservable()
                        .Subscribe(_ => _didEquip = true)
                        .AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
