using System.Collections.Generic;
using Kumu.Extensions;

namespace Kumu.Kulitan.Avatar
{
    public static class AvatarSaveLoadUtil
    {
        public static bool TryConvertToAvatarItemState(string[] itemIds, out AvatarItemState[] avatarItemStates)
        {
            if (itemIds == null || itemIds.Length == 0)
            {
                avatarItemStates = null;
                return false;
            }

            var itemDatabase = ItemDatabase.Current;
            var itemIdsLen = itemIds.Length;

            var stateList = new List<AvatarItemState>();

            for (var i = 0; i < itemIdsLen; i++)
            {
                if (!itemDatabase.TryGetItem(itemIds[i], out var itemConfig))
                {
                    $"Failed to get item from database. [{itemIds[i]}]".LogError();
                    continue;
                }
                
                stateList.Add(itemConfig.State);
            }


            avatarItemStates = stateList.ToArray();
                
            return true;
        }
    }
}
