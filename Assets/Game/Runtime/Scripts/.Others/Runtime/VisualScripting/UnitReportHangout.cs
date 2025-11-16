using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitReportHangout : UnitServiceBase<ReportHangoutResult>
    {
        [DoNotSerialize]
        public ValueInput info;

        [DoNotSerialize]
        public ValueInput userAccountId;

        [DoNotSerialize]
        public ValueInput photonRoomId;

        [DoNotSerialize]
        public ValueInput photonRoomName;

        [DoNotSerialize]
        public ValueInput selectedCategory;

        [DoNotSerialize]
        public ValueInput selectedSubcategory;


        [DoNotSerialize]
        public ValueInput shouldBlock;

        protected override Task<ServiceResultWrapper<ReportHangoutResult>> GetServiceOperation()
        {
            var variableDecl = Variables.Object(flow.stack.gameObject);

            var request = new ReportHangoutRequest
            {
                accountId = flow.GetValue<string>(userAccountId),
                photonRoomId = flow.GetValue<string>(photonRoomId),
                photonRoomName = flow.GetValue<string>(photonRoomName),
                category = flow.GetValue<string>(selectedCategory),
                subcategory = flow.GetValue<string>(selectedSubcategory),
                info = flow.GetValue<string>(info),
                shouldBlock = flow.GetValue<bool>(shouldBlock)
            };

            return Services.ModerationService.ReportHangoutAsync(request);
        }

        protected override void Definition()
        {
            base.Definition();
            info = ValueInput<string>(nameof(info));
            userAccountId = ValueInput<string>(nameof(userAccountId));
            photonRoomId = ValueInput<string>(nameof(photonRoomId));
            photonRoomName = ValueInput<string>(nameof(photonRoomName));
            selectedCategory = ValueInput<string>(nameof(selectedCategory));
            selectedSubcategory = ValueInput<string>(nameof(selectedSubcategory));
            shouldBlock = ValueInput<bool>(nameof(shouldBlock));
        }
    }
}
