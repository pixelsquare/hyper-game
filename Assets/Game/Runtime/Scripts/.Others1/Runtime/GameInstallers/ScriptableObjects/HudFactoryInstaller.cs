using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Hud Factory Installer", fileName = "HudFactoryInstaller")]
    public class HudFactoryInstaller : ScriptableObjectInstaller, IInitializable
    {
        [Header("Game State HUD")]
        [SerializeField] private AssetReferenceGameObject _loginHudRef;
        [SerializeField] private AssetReferenceGameObject _mainMenuHudRef;
        [SerializeField] private AssetReferenceGameObject _missionSelectRef;
        [SerializeField] private AssetReferenceGameObject _gameplayHudRef;

        [Header("Main Menu HUD")]
        [SerializeField] private AssetReferenceGameObject _heroLoadoutHudRef;
        [SerializeField] private AssetReferenceGameObject _gearHudRef;
        [SerializeField] private AssetReferenceGameObject _inventoryHudRef;
        [SerializeField] private AssetReferenceGameObject _shopHudRef;
        [SerializeField] private AssetReferenceGameObject _settingsHudRef;
        [SerializeField] private AssetReferenceGameObject _missionResultHudRef;
        [SerializeField] private AssetReferenceGameObject _inGamePauseHudRef;
        [SerializeField] private AssetReferenceGameObject _legaciesHudRef;
        [SerializeField] private AssetReferenceGameObject _itemEquipHudRef;
        [SerializeField] private AssetReferenceGameObject _legacyPickerHudRef;
        [SerializeField] private AssetReferenceGameObject _loadGameHudRef;
        [SerializeField] private AssetReferenceGameObject _unloadGameHudRef;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HudFactoryInstaller>()
                     .AsSingle();

            Container.Bind<Dictionary<HudType, AssetReferenceGameObject>>()
                     .FromMethod(CreateMapping)
                     .AsSingle()
                     .WhenInjectedInto<HudFactory>();

            Container.BindFactory<HudType, Transform, BaseHud, HudFactory>()
                     .AsSingle()
                     .IfNotBound();
        }

        public void Initialize()
        {
            Container.TryResolve<HudFactory>()?
                   .Initialize();
        }

        private Dictionary<HudType, AssetReferenceGameObject> CreateMapping()
        {
            var mapping = new Dictionary<HudType, AssetReferenceGameObject>();
            mapping[HudType.Login] = _loginHudRef;
            mapping[HudType.MainMenu] = _mainMenuHudRef;
            mapping[HudType.MissionsSelect] = _missionSelectRef;
            mapping[HudType.Game] = _gameplayHudRef;
            mapping[HudType.HeroLoadout] = _heroLoadoutHudRef;
            mapping[HudType.Gear] = _gearHudRef;
            mapping[HudType.Inventory] = _inventoryHudRef;
            mapping[HudType.Shop] = _shopHudRef;
            mapping[HudType.Settings] = _settingsHudRef;
            mapping[HudType.MissionResult] = _missionResultHudRef;
            mapping[HudType.InGamePause] = _inGamePauseHudRef;
            mapping[HudType.Legacies] = _legaciesHudRef;
            mapping[HudType.ItemEquip] = _itemEquipHudRef;
            mapping[HudType.LegacyPicker] = _legacyPickerHudRef;
            mapping[HudType.LoadGame] = _loadGameHudRef;
            mapping[HudType.UnloadGame] = _unloadGameHudRef;
            return mapping;
        }
    }
}
