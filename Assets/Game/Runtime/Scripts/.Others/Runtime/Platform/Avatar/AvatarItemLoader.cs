using System;
using System.Linq;
using System.Threading.Tasks;
using Hangout;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Hangout;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarItemLoader : MonoBehaviour
    {
        private static bool isLoading;

        [SerializeField] private EntityView entityView;
        [SerializeField] private SwatchTable swatchTable;
        [SerializeField] private Transform modelTransform;

#if !ADDRESSABLES_ENABLED
        [SerializeField] private AvatarCustomizer avatarCustomizer;
#else
        private AvatarCustomizer avatarCustomizer;
#endif

        public EntityRef EntityRef { set => entityRef = value; }

        private ItemDatabase itemDatabase;
        private EntityRef entityRef;
        private State state = State.Default;

        [Flags]
        private enum State
        {
            Default = 0b0000,
            EntityLoaded = 0b0001,
            ModelLoaded = 0b0010,
            ItemsReady = 0b0011,
            ItemsLoaded = 0b0100,
        }

        public void Initialize()
        {
            itemDatabase = ItemDatabase.Current;
            entityRef = entityView != null ? entityView.EntityRef : entityRef;
            state |= State.EntityLoaded;

            var f = QuantumRunner.Default.Game.Frames.Verified;
            var hangoutPlayer = f.Get<HangoutPlayer>(entityRef);
            var isLocal = QuantumRunner.Default.Game.PlayerIsLocal(hangoutPlayer.player);

            if (isLocal)
            {
                var itemStates = GetLocalAvatarItems();
                SendLoadAvatarCommand(itemStates);
            }
        }

        public void DeInitialize()
        {
            state -= State.EntityLoaded;
            QuantumEvent.UnsubscribeListener(this);
        }

#if ADDRESSABLES_ENABLED
        public void InitializeModel(GameObject modelObj)
        {
            avatarCustomizer = modelObj.GetComponentInChildren<AvatarCustomizer>();
            modelObj.transform.parent = modelTransform;
            modelObj.transform.localPosition = Vector3.down;
            modelObj.transform.localRotation = Quaternion.identity;
            state |= State.ModelLoaded;
        }
#endif

        private HangoutItemState[] GetLocalAvatarItems()
        {
            var itemStates = from equip in UserInventoryData.EquippedItems
                             where itemDatabase.HasItem(equip.itemId)
                             let idx = itemDatabase.GetItemIndex(equip.itemId)
                             let itemConfig = itemDatabase.GetItem(idx)
                             let itemType = AvatarItemUtil.ToAvatarItemType(itemConfig.GetTypeCode()) 
                             let color = equip.hasColor ? equip.Color :
                                         swatchTable.TryGetDefaultColor(itemType, out var defaultColor) ? defaultColor :
                                         Color.white
                             select AvatarItemState.ToHangoutItemState(itemDatabase, equip, color);
            return itemStates.ToArray();  
        }

        private void PollLoadAvatars()
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;

            if (f.TryGet<HangoutAvatarItems>(entityRef, out var avatarItems))
            {
                state |= State.ItemsLoaded;
                LoadAvatarItems(f, avatarItems);

                GlobalNotifier.Instance.Trigger(new PlayerAvatarLoadedEvent(entityRef, entityView, avatarItems));
            }
        }

        private void SendLoadAvatarCommand(HangoutItemState[] itemStates)
        {
            var loadAvatarCommand = new LoadAvatarCommand()
            {
                itemStates = itemStates,
            };

            QuantumRunner.Default.Game.SendCommand(loadAvatarCommand);
        }

        public void LoadAvatarItems(Frame f, HangoutAvatarItems avatarItems)
        {
            if (avatarCustomizer) // necessary due to async instantiation via addressables
            {
                avatarCustomizer.DeselectAll();
            }

            var list = f.ResolveList(avatarItems.itemStates);

            foreach (var item in list)
            {
                SelectItem(item);
            }
        }

        private async Task SelectItem(HangoutItemState itemState)
        {
            var color = new Color(
                itemState.r.AsFloat,
                itemState.g.AsFloat,
                itemState.b.AsFloat,
                itemState.a.AsFloat
            );

            var itemConfig = itemDatabase.GetItem(itemState.itemIdx);

            try // todo: move away from try-catch; should expect valid inputs instead of checking here
            {
                await avatarCustomizer.SelectItem(itemConfig, color);
            }
            catch (Exception e)
            {
                $"{entityRef.Index.WrapColor(Color.red)} failed to load item: {itemConfig.Data.itemId.WrapColor(color)}".LogError();
                e.Message.LogError();
                e.StackTrace.LogError();
            }
        }

        private void ResetLoadFlag(EventOnAvatarItemsChanged eventData)
        {
            if (entityView != null && eventData.entity == entityView.EntityRef)
            {
                state -= state & State.ItemsLoaded;
            }
        }

        private void Update()
        {
            if (state == State.ItemsReady)
            {
                PollLoadAvatars();
            }
        }

        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnAvatarItemsChanged>(this, ResetLoadFlag);
        }

        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
    }
}
