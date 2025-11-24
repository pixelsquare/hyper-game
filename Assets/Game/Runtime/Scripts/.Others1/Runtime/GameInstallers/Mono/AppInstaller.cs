using Zenject;

namespace Santelmo.Rinsurv
{
    using GameStateName = GameConstants.GameStates;
    using AppStateEvent = GameEvents.AppState;

    public class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISceneManager>()
                     .To<AddressableSceneManager>()
                     .AsSingle()
                     .NonLazy();

            Container.Bind<BootState>().AsSingle();
            Container.Bind<LoginState>().AsSingle();
            Container.Bind<AssetLoaderState>().AsSingle();
            Container.Bind<MainMenuState>().AsSingle();
            Container.Bind<GameState>().AsSingle();
            Container.Bind<GearState>().AsSingle();
            Container.Bind<HeroLoadoutState>().AsSingle();
            Container.Bind<InventoryState>().AsSingle();
            Container.Bind<MissionSelectState>().AsSingle();
            Container.Bind<ShopState>().AsSingle();
            Container.Bind<LoadGameState>().AsSingle();
            Container.Bind<UnloadGameState>().AsSingle();

            Container.Bind<IStateMachine>()
                     .FromMethod(CreateAppStateMachine)
                     .AsSingle();

            Container.Bind<ReactiveTextureProperty>().AsTransient();

            Container.BindInterfacesAndSelfTo<RegisterGlobalBindings>()
                     .AsSingle()
                     .CopyIntoAllSubContainers();
        }

        private IStateMachine CreateAppStateMachine()
        {
            return StateMachine.Builder
                               .AddState<BootState>(GameStateName.BootState)
                               .AddState<LoginState>(GameStateName.LoginState)
                               .AddState<AssetLoaderState>(GameStateName.AssetLoaderState)
                               .AddState(GameStateName.MainMenuState, CreateMainMenuStateMachine())
                               .AddState<GameState>(GameStateName.GameState)
                               .AddState<MissionSelectState>(GameStateName.MissionSelectState)
                               .AddState<LoadGameState>(GameStateName.LoadGameState)
                               .AddState<UnloadGameState>(GameStateName.UnloadGameState)
                               .AddTransition(GameStateName.BootState, GameStateName.AssetLoaderState)
                               .AddTransition(GameStateName.AssetLoaderState, GameStateName.LoginState)
                               .AddTransition(GameStateName.LoginState, GameStateName.MainMenuState)
                               .AddTransition(GameStateName.MainMenuState, GameStateName.LoginState, AppStateEvent.ToLoginScreenEvent)
                               .AddTransition(GameStateName.MainMenuState, GameStateName.MissionSelectState, AppStateEvent.ToMissionSelectScreenEvent)
                               .AddTransition(GameStateName.MissionSelectState, GameStateName.MainMenuState, AppStateEvent.ToMainMenuScreenEvent)
                               .AddTransition(GameStateName.GameState, GameStateName.UnloadGameState, AppStateEvent.ToMainMenuScreenEvent)
                               .AddTransition(GameStateName.MissionSelectState, GameStateName.LoadGameState)
                               .AddTransition(GameStateName.LoadGameState, GameStateName.GameState)
                               .AddTransition(GameStateName.GameState, GameStateName.UnloadGameState)
                               .AddTransition(GameStateName.UnloadGameState, GameStateName.MainMenuState)
                               .AddProvider(new AndroidOEMProvider())
                               .Build("app_state_machine", Container);
        }

        private IStateMachine CreateMainMenuStateMachine()
        {
            return StateMachine.Builder
                               .AddState<MainMenuState>(GameStateName.MainMenuState)
                               .AddState<HeroLoadoutState>(GameStateName.HeroLoadoutState)
                               .AddState<GearState>(GameStateName.GearState)
                               .AddState<InventoryState>(GameStateName.InventoryState)
                               .AddState<ShopState>(GameStateName.ShopState)
                               .AddTransition(GameStateName.MainMenuState, GameStateName.HeroLoadoutState, AppStateEvent.ToHeroesScreenEvent)
                               .AddTransition(GameStateName.MainMenuState, GameStateName.GearState, AppStateEvent.ToGearScreenEvent)
                               .AddTransition(GameStateName.MainMenuState, GameStateName.InventoryState, AppStateEvent.ToInventoryScreenEvent)
                               .AddTransition(GameStateName.MainMenuState, GameStateName.ShopState, AppStateEvent.ToShopScreenEvent)
                               .AddTransition(GameStateName.HeroLoadoutState, GameStateName.MainMenuState, AppStateEvent.ToMainMenuScreenEvent)
                               .AddTransition(GameStateName.HeroLoadoutState, GameStateName.InventoryState, AppStateEvent.ToInventoryScreenEvent)
                               .AddTransition(GameStateName.GearState, GameStateName.MainMenuState, AppStateEvent.ToMainMenuScreenEvent)
                               .AddTransition(GameStateName.InventoryState, GameStateName.MainMenuState, AppStateEvent.ToMainMenuScreenEvent)
                               .AddTransition(GameStateName.ShopState, GameStateName.MainMenuState, AppStateEvent.ToMainMenuScreenEvent)
                               .PlaysAutomatically(false)
                               .Build("main_menu_state_machine", Container);
        }
    }
}
