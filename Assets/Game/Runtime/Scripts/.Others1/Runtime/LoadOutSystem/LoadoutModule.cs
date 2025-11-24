using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using Santelmo.Rinsurv.Backend;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class LoadoutModule : IGlobalBinding
    {
        private readonly IItemDataModule _itemDataModule;
        private readonly ICloudSyncService _cloudSyncService;
        private readonly Dictionary<Hero, Loadout> _loadoutMap = new();
        private Loadout _currentLoadout;

        private CancellationTokenSource readCancellationTokenSource = new();
        private CancellationTokenSource writeCancellationTokenSource = new();

        private const string LOAD_OUT_KEY = "loadouts";

        [Inject]
        public LoadoutModule(IItemDataModule dataModule, ICloudSyncService cloudSyncService)
        {
            _itemDataModule = dataModule;
            _cloudSyncService = cloudSyncService;
        }

        public Loadout GetLoadout(Hero hero)
        {
            return _loadoutMap.TryGetValue(hero, out var heroLoadout) ? heroLoadout : new Loadout();
        }

        public void Initialize(IEnumerable<LoadoutData> loadouts)
        {
            foreach (var loadout in loadouts)
            {
                if (_itemDataModule.GetHeroFromId(loadout.Hero) is { } heroInstance)
                {
                    _loadoutMap[heroInstance] = new Loadout
                    {
                        Hero = _itemDataModule.GetHeroFromId(loadout.Hero ?? string.Empty),
                        Weapon = _itemDataModule.GetWeaponFromId(loadout.Weapon ?? string.Empty),
                        EmblemDeparture = _itemDataModule.GetEmblemDeparture(loadout.EmblemDeparture ?? string.Empty),
                        EmblemPursuit = _itemDataModule.GetEmblemPursuit(loadout.EmblemPursuit ?? string.Empty),
                        EmblemChange = _itemDataModule.GetEmblemChange(loadout.EmblemChange ?? string.Empty)
                    };
                }
            }
        }

        public void UpdateLoadout(Hero hero, Weapon weapon, EmblemDeparture emblemDeparture,
                                  EmblemPursuit emblemPursuit, EmblemChange emblemChange)
        {
            var loadout = new Loadout();

            if (_loadoutMap.TryGetValue(hero, out var value))
            {
                loadout = value;
            }

            loadout.Hero = hero;
            loadout.Weapon = weapon;
            loadout.EmblemDeparture = emblemDeparture;
            loadout.EmblemPursuit = emblemPursuit;
            loadout.EmblemChange = emblemChange;
            _loadoutMap[hero] = loadout;
        }

        public async UniTask ReadFromCloudAsync()
        {
            if (_cloudSyncService is null)
            {
                Debug.LogError($"No {typeof(ICloudSyncService)} found");
                return;
            }

            readCancellationTokenSource?.Cancel();
            readCancellationTokenSource?.Dispose();
            readCancellationTokenSource = new CancellationTokenSource();

            try
            {
                var readData = await _cloudSyncService.ReadAsync<Dictionary<string, LoadoutData>>(LOAD_OUT_KEY, readCancellationTokenSource.Token);
                foreach (var (key, value) in readData)
                {
                    var hero = _itemDataModule.GetHeroFromId(key ?? string.Empty);
                    var loadout = new Loadout
                    {
                        Hero = hero,
                        Weapon = _itemDataModule.GetWeaponFromId(value.Weapon ?? string.Empty),
                        EmblemDeparture = _itemDataModule.GetEmblemDeparture(value.EmblemDeparture ?? string.Empty),
                        EmblemPursuit = _itemDataModule.GetEmblemPursuit(value.EmblemPursuit ?? string.Empty),
                        EmblemChange = _itemDataModule.GetEmblemChange(value.EmblemChange ?? string.Empty)
                    };

                    _loadoutMap[hero] = loadout;
                }
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e);
            }
        }

        public async UniTask WriteItemCountsAsync()
        {
            if (_cloudSyncService is null)
            {
                Debug.LogError($"No {typeof(ICloudSyncService)} found");
                return;
            }

            writeCancellationTokenSource?.Cancel();
            writeCancellationTokenSource?.Dispose();
            writeCancellationTokenSource = new CancellationTokenSource();

            var dataToSync = new Dictionary<string, LoadoutData>();

            foreach (var loadoutPair in _loadoutMap)
            {
                dataToSync[loadoutPair.Key.Id] = loadoutPair.Value.ToLoadoutData();
            }

            try
            {
                await _cloudSyncService.WriteAsync(LOAD_OUT_KEY, dataToSync, writeCancellationTokenSource.Token);
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public struct Loadout
    {
        public Hero Hero { get; set; }
        public Weapon Weapon { get; set; }
        public EmblemDeparture EmblemDeparture { get; set; }
        public EmblemPursuit EmblemPursuit { get; set; }
        public EmblemChange EmblemChange { get; set; }

        public LoadoutData ToLoadoutData()
        {
            return new LoadoutData
            {
                Hero = Hero?.Id,
                Weapon = Weapon?.Id,
                EmblemDeparture = EmblemDeparture?.Id,
                EmblemPursuit = EmblemPursuit?.Id,
                EmblemChange = EmblemChange?.Id
            };
        }
    }

    /// <summary>
    ///     Used for data storing
    /// </summary>
    [Serializable][FirestoreData]
    public struct LoadoutData
    {
        [FirestoreProperty]
        public string Hero { get; set; }
        [FirestoreProperty]
        public string Weapon { get; set; }
        [FirestoreProperty]
        public string EmblemDeparture { get; set; }
        [FirestoreProperty]
        public string EmblemPursuit { get; set; }
        [FirestoreProperty]
        public string EmblemChange { get; set; }
    }
}
