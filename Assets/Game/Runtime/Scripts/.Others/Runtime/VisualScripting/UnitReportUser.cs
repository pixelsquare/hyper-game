using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitReportUser : UnitServiceBase<ReportUserResult>
    {
        [DoNotSerialize]
        public ValueInput info;

        [DoNotSerialize]
        public ValueInput userAccountId;

        [DoNotSerialize]
        public ValueInput selectedCategory;

        [DoNotSerialize]
        public ValueInput selectedSubcategory;

        [DoNotSerialize]
        public ValueInput shouldBlock;

        protected override Task<ServiceResultWrapper<ReportUserResult>> GetServiceOperation()
        {
            var request = new ReportUserRequest
            {
                accountId = flow.GetValue<string>(userAccountId),
                category = flow.GetValue<string>(selectedCategory),
                subcategory = flow.GetValue<string>(selectedSubcategory),
                info = flow.GetValue<string>(info),
                shouldBlock = flow.GetValue<bool>(shouldBlock)
            };

            return Services.ModerationService.ReportUserAsync(request);
        }

        protected override void Definition()
        {
            base.Definition();
            info = ValueInput<string>(nameof(info));
            userAccountId = ValueInput<string>(nameof(userAccountId));
            selectedCategory = ValueInput<string>(nameof(selectedCategory));
            selectedSubcategory = ValueInput<string>(nameof(selectedSubcategory));
            shouldBlock = ValueInput<bool>(nameof(shouldBlock));
        }
    }
}
