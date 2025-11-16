using Kumu.Kulitan.Events;
using Quantum;

namespace Kumu.Kulitan.Hangout
{
    public class PlayerAvatarLoadedEvent : Event<string>
    {
        public const string EVENT_NAME = "OnPlayerAvatarLoadedEvent";

        public PlayerAvatarLoadedEvent(EntityRef playerEntity, EntityView entView, HangoutAvatarItems avatarItems) : base(EVENT_NAME)
        {
            PlayerEntity = playerEntity;
            PlayerEntityView = entView;
            AvatarItems = avatarItems;
        }

        public EntityRef PlayerEntity { get; }
        public EntityView PlayerEntityView { get; }
        public HangoutAvatarItems AvatarItems { get; }
    }
}
