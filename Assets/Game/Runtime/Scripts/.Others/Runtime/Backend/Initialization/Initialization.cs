namespace Kumu.Kulitan.Backend
{
    public static class Services
    {
        public static IAuthService AuthService { get; set; }

        public static IUserProfileService UserProfileService { get; set; }

        public static IShopService ShopService { get; set; }

        public static IInventoryService InventoryService { get; set; }

        public static ISocialService SocialService { get; set; }

        public static IAgoraService AgoraService { get; set; }

        public static IVirtualGiftService VirtualGiftService { get; set; }

        public static IModerationService ModerationService { get; set; }

        public static IFCMService FCMService { get; set; }

        public static ILobbyService LobbyService { get; set; }
        
        public static ICurrencyShopService CurrencyShopService { get; set; }

        public static void Initialize()
        {
#if USES_MOCKS
            AuthService = MockedServiceMono.CreateNewInstance<MockedAuthServiceMono>();
            UserProfileService = MockedServiceMono.CreateNewInstance<MockedUserProfileServiceMono>();
            ShopService = MockedServiceMono.CreateNewInstance<MockedShopServiceMono>();
            InventoryService = MockedServiceMono.CreateNewInstance<MockedInventoryServiceMono>();
            SocialService = MockedServiceMono.CreateNewInstance<MockedSocialServiceMono>();
            AgoraService = MockedServiceMono.CreateNewInstance<MockedAgoraServiceMono>();
            VirtualGiftService = MockedServiceMono.CreateNewInstance<MockedVirtualGiftServiceMono>();
            ModerationService = MockedServiceMono.CreateNewInstance<MockedModerationServiceMono>();
            LobbyService = MockedServiceMono.CreateNewInstance<MockedLobbyServiceMono>();
            CurrencyShopService = MockedServiceMono.CreateNewInstance<MockedCurrencyShopServiceMono>();
#else
            AuthService = new FirebaseAuthService();
            UserProfileService = new UserProfileService();
            ShopService = new ShopService();
            InventoryService = new InventoryService();
            SocialService = new SocialService();
            AgoraService = new AgoraService();
            VirtualGiftService = new VirtualGiftService();
            ModerationService = new ModerationService();
            LobbyService = new LobbyService();
            CurrencyShopService = new CurrencyShopService();
#endif
            FCMService = new FCMService();
        }
    }

    public static class Validators
    {
        public static IInputFormatValidator<UserNameFormatDetails> UserNameValidator { get; private set; }

        public static IInputFormatValidator<UserNickNameFormatDetails> UserNickNameValidator { get; private set; }

        public static IInputFormatValidator OtpValidator { get; private set; }

        public static IInputFormatValidator MobileValidator { get; private set; }

        public static IInputFormatValidator UsernameLinkValidator { get; private set; }

        public static IInputFormatValidator EmailValidator { get; private set; }


        public static void Initialize()
        {
            UserNameValidator = new UserNameInputFormatValidator();
            UserNickNameValidator = new UserNickNameInputFormatValidator();
            OtpValidator = new OtpInputFormatValidator();
            MobileValidator = new MobileInputFormatValidator();
            UsernameLinkValidator = new UsernameLinkValidator();
            EmailValidator = new EmailValidator();
        }
    }
}
