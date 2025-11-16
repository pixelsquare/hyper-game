using System;
using System.Linq;

namespace Kumu.Kulitan.Avatar
{
    [Unity.VisualScripting.IncludeInSettings(true)] // We need this attribute so the Visual Scripting can access this static class
    public static class UserInventoryData
    {
        private static AvatarItemState[] equippedItems = Array.Empty<AvatarItemState>();
        private static AvatarItemState[] ownedItems = Array.Empty<AvatarItemState>();
        private static UserWallet userWallet = UserWallet.Default;
        private static bool isInitialized;
        private static bool isBalanceUpdated;

        public static void Reset()
        {
            EquippedItems = Array.Empty<AvatarItemState>();;
            OwnedItems = Array.Empty<AvatarItemState>();
            UserWallet = UserWallet.Default;
            IsInitialized = false;
            IsBalanceUpdated = false;
        }

        public static AvatarItemState[] EquippedItems
        {
            get => equippedItems;
            set
            {
                equippedItems = value;
                OnValuesUpdated?.Invoke();
                OnEquippedItemsUpdated?.Invoke();
            }
        }

        public static AvatarItemState[] OwnedItems
        {
            get => ownedItems;
            set
            {
                ownedItems = value;
                OnValuesUpdated?.Invoke();
                OnOwnedItemsUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Setter used by FSM in AvatarCustomization.unity
        /// </summary>
        public static UserWallet UserWallet
        {
            get => userWallet;
            set
            {
                userWallet = value;
                OnValuesUpdated?.Invoke();
                OnWalletUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Setter used by FSM in AvatarCustomization.unity
        /// </summary>
        public static bool IsInitialized
        {
            get => isInitialized;
            set
            {
                isInitialized = value;
                OnValuesUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Setter used by FSM in AvatarCustomization.unity
        /// </summary>
        public static bool IsBalanceUpdated
        {
            get => isBalanceUpdated;
            set
            {
                isBalanceUpdated = value;
                OnValuesUpdated?.Invoke();
            }
        }

        public static Action OnValuesUpdated;
        public static Action OnWalletUpdated;
        public static Action OnOwnedItemsUpdated;
        public static Action OnEquippedItemsUpdated;

        /// <summary>
        ///     Checks if item exist in cached owned items.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool IsItemOwned(string itemId)
        {
            return OwnedItems.Any(x => x.itemId.Equals(itemId));
        }

        /// <summary>
        ///     Checks if item exist in cached equipped items.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool IsItemEquipped(string itemId)
        {
            return EquippedItems.Any(x => x.itemId.Equals(itemId));
        }
    }
}
