namespace Kumu.Kulitan.Backend
{
    public static class MockedErrors
    {
        public static readonly ServiceError appInMaintenanceError = new(ServiceErrorCodes.APP_IN_MAINTENANCE, "Under maintenance. Please come back later.");

        public static readonly ServiceError mobileAlreadyRegisteredError = new(ServiceErrorCodes.MOBILE_ALREADY_REGISTERED, "Mobile number already registered.");

        public static readonly ServiceError mobileNotRegisteredError = new(ServiceErrorCodes.MOBILE_NOT_REGISTERED, "Mobile number has not been registered.");

        public static readonly ServiceError invalidDataError = new(ServiceErrorCodes.INVALID_DATA, "Invalid data.");

        public static readonly ServiceError invalidOtpError = new(ServiceErrorCodes.INVALID_OTP, "OTP is invalid");

        public static readonly ServiceError tokenExpiredError = new(ServiceErrorCodes.TOKEN_EXPIRED, "Token expired.");

        public static readonly ServiceError profileAlreadyExistsError = new(ServiceErrorCodes.PROFILE_ALREADY_EXISTS, "Profile already exists.");

        public static readonly ServiceError playerNotFoundError = new(ServiceErrorCodes.PLAYER_NOT_FOUND, "No player found connected to the mobile number.");

        public static readonly ServiceError itemsNotFoundError = new(ServiceErrorCodes.ITEM_NOT_FOUND, "Item not found.");

        public static readonly ServiceError itemsNotOwnedError = new(ServiceErrorCodes.ITEM_NOT_OWNED, "Item not owned.");

        public static readonly ServiceError usernameExhaustedError = new(ServiceErrorCodes.USERNAME_EXHAUSTED, "Username is already taken.");

        public static readonly ServiceError insufficientWalletBalanceError = new(ServiceErrorCodes.INSUFFICIENT_WALLET_BALANCE, "Insufficient wallet balance.");

        public static readonly ServiceError expectedCostsMismatchError = new(ServiceErrorCodes.EXPECTED_COSTS_MISMATCH, "Expected costs mismatch.");

        public static readonly ServiceError itemsAlreadyOwnedError = new(ServiceErrorCodes.ITEM_ALREADY_OWNED, "Item already owned.");

        public static readonly ServiceError transactionFailedError = new(ServiceErrorCodes.TRANSACTION_FAILED, "Transaction failed.");
        
        public static readonly ServiceError roomIdDoesNotExistError = new(ServiceErrorCodes.ROOM_ID_DOES_NOT_EXIST, "Room ID does not exist.");
    }
}
