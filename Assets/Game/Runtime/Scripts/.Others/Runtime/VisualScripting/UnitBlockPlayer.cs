using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitBlockPlayer : UnitServiceBase<BlockPlayerResult>
    {
        [DoNotSerialize]
        public ValueInput accountId;
        
        [DoNotSerialize]
        public ValueInput playerId;

        protected override Task<ServiceResultWrapper<BlockPlayerResult>> GetServiceOperation()
        {
            var request = new BlockPlayerRequest
            {
                userId = flow.GetValue<string>(accountId)
            };

            return Services.ModerationService.BlockPlayerAsync(request);
        }

        protected override void BeforeExit()
        {
            var accountId = flow.GetValue<string>(this.accountId);
            var playerId = flow.GetValue<uint>(this.playerId);
            GlobalNotifier.Instance.Trigger(new PlayerBlockedEvent(accountId, playerId, true));
        }

        protected override void Definition()
        {
            base.Definition();
            accountId = ValueInput<string>(nameof(accountId));
            playerId = ValueInput<uint>(nameof(playerId));
        }
    }
}
