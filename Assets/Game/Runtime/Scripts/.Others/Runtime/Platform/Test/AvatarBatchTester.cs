using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarBatchTester : MonoBehaviour
    {
        [SerializeField] private Config[] configs;
        [SerializeField] private DefaultItemData defaultItemData;
        [SerializeField] private SwatchTable defaultColors;

        private async void Start()
        {
            foreach (var config in configs)
            {
                foreach (var pair in defaultItemData.AllItemPairs())
                {
                    var itemConfig = pair.Value;
                    var itemType = pair.Key;

                    if (defaultColors.TryGetDefaultColor(itemType, out var color))
                    {
                        itemConfig.SetStateColor(color);
                    }
                    
                    await config.Equip(itemConfig);
                }

                await config.Customize();
            }
        }

        [Serializable]
        private class Config
        {
            [SerializeField] private AvatarCustomizer avatarCustomizer;
            [SerializeField] private ItemDataState[] customItems;
            
            public async Task Customize()
            {
                foreach (var item in customItems)
                {
                    await item.Equip(avatarCustomizer);
                }
            }

            public async Task Equip(AvatarItemConfig itemConfig)
            {
                await avatarCustomizer.SelectItem(itemConfig);
            }
        }

        [Serializable]
        private class ItemDataState
        {
            [SerializeField] private AvatarItemConfig itemConfig;
            [SerializeField] private Color color = Color.white;

            public async Task Equip(AvatarCustomizer avatarCustomizer)
            {
                itemConfig.SetStateColor(color);
                await avatarCustomizer.SelectItem(itemConfig);
            }
        }
    }
}
