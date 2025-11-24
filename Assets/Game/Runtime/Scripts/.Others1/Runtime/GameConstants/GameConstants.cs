namespace Santelmo.Rinsurv
{
    public static class GameConstants
    {
        public static class SceneNames
        {
            public const string BootScene = "BootScene";
            public const string AssetLoaderScene = "AssetLoaderScene";
            public const string ManagerScene = "ManagerScene";
            public const string WorldScene = "WorldScene";
            public const string UiScene = "UIScene";
        }

        public static class GameStates
        {
            public const string BootState = "BootState";
            public const string LoginState = "LoginState";
            public const string AssetLoaderState = "AssetLoaderState";
            public const string MainMenuState = "MainMenuState";
            public const string GameState = "GameState";
            public const string LoadGameState = "LoadGameState";
            public const string UnloadGameState = "UnloadGameState";
            public const string MissionSelectState = "MissionSelectState";
            public const string ShopState = "ShopState";
            public const string GearState = "GearState";
            public const string InventoryState = "InventoryState";
            public const string HeroLoadoutState = "HeroLoadoutState";
        }

        public static class SaveKeys
        {
            public const string InstallDateUtc = "InstallDateUtc";
            public const string SfxVolumeKey = "SfxVolume";
            public const string BgmVolumeKey = "BgmVolume";
            public const string ActiveHeroKey = "ActiveHero";
            public const string HeroItemEquippedKey = "HeroItemEquippedKey";
        }

        public static class GameValues
        {
            public const int HitCollisions = 50;
        }

        public static class AnimatorParameters
        {
            public const string IsOn = "isOn";
            public const string LegacyLayer = "layer";
        }
    }
}
