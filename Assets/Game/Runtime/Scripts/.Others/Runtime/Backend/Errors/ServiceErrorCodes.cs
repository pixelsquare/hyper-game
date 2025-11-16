namespace Kumu.Kulitan.Backend
{
    public class ServiceErrorCodes
    {
        // Service layer errors
        public const int UNKNOWN_ERROR = 1000;
        public const int NETWORK_UNREACHABLE_ERROR = 1001;
        public const int NETWORK_TIMEOUT_ERROR = 1002;
        public const int NETWORK_UNKNOWN_ERROR = 1003;

        // Backend errors
        public const int APP_VERSION_MISMATCH = 2001;
        public const int APP_IN_MAINTENANCE = 2002;
        public const int MOBILE_ALREADY_REGISTERED = 2003;
        public const int API_ENDPOINT_DOES_NOT_EXIST = 2004;
        public const int INVALID_DATA = 2005;
        public const int MOBILE_NOT_REGISTERED = 2006;
        public const int INVALID_OTP = 2007;
        public const int INTERNAL_SERVER_ERROR = 2008; // also used for INVALID_STORE
        public const int INCORRECT_METHOD = 2009;
        public const int INVALID_TOKEN = 2011;
        public const int SERVER_CRITICAL_ERROR = 2010;
        public const int JWT_TOKEN_INVALID = 2011;
        public const int AGORA_RTM_TOKEN_ERROR = 2013;
        public const int PAYLOAD_TOO_LARGE = 2014;
        public const int RATE_LIMIT_REACHED = 2015;
        public const int DATABASE_ERROR = 2016;
        public const int INVALID_API_KEY = 2017;
        public const int INVALID_DEVICE_ID = 2018;
        public const int USERNAME_EXHAUSTED = 2020;
        public const int TRANSACTION_FAILED = 2021;
        public const int TOKEN_EXPIRED = 2022;
        public const int PROFILE_ALREADY_EXISTS = 2023;
        public const int PROFILE_DOES_NOT_EXIST = 2024;
        public const int PLAYER_NOT_FOUND = 2025;
        public const int SERVER_CACHING_ERROR = 2026;
        public const int ITEM_NOT_FOUND = 2027;
        public const int ITEM_NOT_OWNED = 2028;
        public const int INSUFFICIENT_WALLET_BALANCE = 2029;
        public const int ITEM_ALREADY_OWNED = 2030;
        public const int EXPECTED_COSTS_MISMATCH = 2031;
        public const int AGORA_RTC_TOKEN_ERROR = 2032;
        public const int ITEMS_OCCUPY_SAME_SLOT = 2033;
        public const int SSO_ERROR = 2034;
        public const int ACCOUNT_BANNED_LOGIN = 2071;
        public const int ACCOUNT_BANNED_AGORA = 2072;
        public const int ROOM_ID_DOES_NOT_EXIST = 2074;
        public const int FAKE_STORE_IN_PRODUCTION = 2081;
        public const int FAILED_TO_INCREMENT_WALLET_BALANCE = 2082;
        public const int PRODUCT_ID_MISMATCH = 2084;
        public const int FAILED_TO_VERIFY_RECEIPT = 2085;
        public const int DUPLICATE_TRANSACTION_ID = 2086;
        
        // Non-native errors
        public const int FIREBASE_ERROR = 3000;
    }
}
