namespace Santelmo.Rinsurv
{
    /// <summary>
    /// Centralized location for game events.
    /// </summary>
    public static class GameEvents
    {
        public static class StateMachine
        {
            public const string OnStateChangedEvent = "StateMachine.OnStateChangedEvent";
        }

        public static class AppState
        {
            public const string OnGameStartEvent = "State.OnGameStartEvent";
            public const string OnGameEndedEvent = "State.OnGameEndedEvent";
            public const string OnLoadGameStartEvent = "State.OnLoadGameStartEvent";
            public const string OnLoadGameEndedEvent = "State.OnLoadGameEndedEvent";
            public const string OnUnloadGameStartEvent = "State.OnUnloadGameStartEvent";
            public const string OnUnloadGameEndedEvent = "State.OnUnloadGameEndedEvent";

            public const string OnGoogleLoginEvent = "State.OnGoogleLoginEvent";
            public const string OnAppleLoginEvent = "State.OnAppleLoginEvent";
            public const string OnFacebookLoginEvent = "State.OnFacebookLoginEvent";
            public const string OnGuestLoginEvent = "State.OnGuestLoginEvent";
            public const string OnUserSignOutEvent = "State.OnUserSignOutEvent";
            public const string OnSignInErrorEvent = "State.OnSignInErrorEvent";

            public const string ToPreviousScreenEvent = "StateMachine.ToPreviousScreenEvent";
            public const string ToLoginScreenEvent = "StateMachine.ToLoginScreenEvent";
            public const string ToHeroesScreenEvent = "StateMachine.ToHeroesScreenEvent";
            public const string ToGearScreenEvent = "StateMachine.ToGearScreenEvent";
            public const string ToInventoryScreenEvent = "StateMachine.ToInventoryScreenEvent";
            public const string ToShopScreenEvent = "StateMachine.ToShopScreenEvent";
            public const string ToMainMenuScreenEvent = "StateMachine.ToMainMenuScreenEvent";
            public const string ToMissionSelectScreenEvent = "StateMachine.ToMissionSelectScreenEvent";

            public const string OnLoadGameplayObjectsEvent = "State.OnLoadGameplayObjects";
        }

        public static class UserInterface
        {
            public const string OnStartDownloadAssets = "UserInterface.OnStartDownloadAssets";
            public const string OnInventoryFilterSelected = "UserInterface.OnInventoryFilterSelected";
            public const string OnActiveHeroSelected = "UserInterface.OnActiveHeroSelected";
            public const string OnActiveMissionSelected = "UserInterface.OnActiveMissionSelected";
            public const string OnActiveLevelSelected = "UserInterface.OnActiveLevelSelected";
            public const string OnActiveLegacySelected = "UserInterface.OnActiveLegacySelected";
        }

        public static class Gameplay
        {
            public const string OnEquipLegacy = "Gameplay.OnEquipLegacy";
            public const string OnSelectLegacy = "Gameplay.OnSelectLegacy";
            public const string OnRollLegacy = "Gameplay.OnRollLegacy";
            public const string OnMinibossWaveFinish = "Gameplay.OnFinishMinibossWave";
            public const string OnFinaleWaveStart = "Gameplay.OnFinaleWaveStart";
            public const string OnBossSpawn = "Gameplay.OnBossSpawn";
            public const string OnGameWin = "Gameplay.OnGameWin";
            public const string OnGameLose = "Gameplay.OnGameLose";
            public const string OnGameQuit = "Gameplay.OnGameQuit";
            
            public const string ModifyCooldownReduction = "ModifyCooldownReduction";
            public const string ModifyThunderLegacyDamage = "ModifyThunderLegacyDamage";
            public const string ModifyBaklawEnlargement = "ModifyBaklawEnlargement";
        }
    }
}
