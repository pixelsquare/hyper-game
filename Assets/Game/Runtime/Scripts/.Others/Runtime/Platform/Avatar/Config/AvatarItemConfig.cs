using System;
using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Avatar
{
    public abstract class AvatarItemConfig : ScriptableObject
    {
        [SerializeField] protected AvatarItemData data;
        [SerializeField] protected AssetReferenceSprite spriteRef;
        [SerializeField] private bool unpurchaseable;

        [NonSerialized] protected AvatarItemState state;

        public AvatarItemData Data => data;
        public AvatarItemState State => state;
        public AssetReferenceSprite SpriteRef => spriteRef;
        public bool Unpurchaseable => unpurchaseable;

        /// <summary>
        /// Return the string value of the Avatar part this item is slotted into.
        /// </summary>
        /// <returns></returns>
        public abstract string GetTypeCode();
        public abstract Task<bool> TryDownloadAddressables();

        public void SetStateColor(Color color)
        {
            state.Color = color;
            state.hasColor = true;
        }

        public void SetItemCost(Currency cost, int markUpDownCost)
        {
            data.cost = cost;
            data.markUpDownCost = markUpDownCost;

            if (cost.amount != markUpDownCost)
            {
                data.cost.amount = markUpDownCost;
            }
        }

        public void ResetStateColor()
        {
            state.Color = Color.white;
            state.hasColor = false;
        }

        private void OnEnable()
        {
            state = new AvatarItemState
            {
                itemId = data.itemId,
                typeCode = GetTypeCode(),
                hasColor = false,
                Color = Color.white
            };
        }
    }
}
