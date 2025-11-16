using System.Collections;
using Kumu.Kulitan.Backend;
using System.Threading.Tasks;
using Kumu.Extensions;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitFetchDiamondPrices : WaitUnit
    {
        private CurrencyShopProductCostData[] costData;

        [DoNotSerialize]
        public ValueOutput successOutput;

        [DoNotSerialize]
        public ValueOutput error;

        [DoNotSerialize]
        public ControlOutput errorTrigger;

        private Flow flow;
        private Task<ServiceResultWrapper<GetCurrencyShopProductsResult>> task;

        private VariableDeclarations objectDeclarations;
        private VariableDeclarations sceneDeclarations;

        protected override void Definition()
        {
            base.Definition();
            error = ValueOutput<ServiceError>(nameof(error));
            errorTrigger = ControlOutput(nameof(errorTrigger));
            successOutput = ValueOutput<CurrencyShopProductCostData[]>(nameof(successOutput));
            Succession(enter, errorTrigger);
        }

        protected sealed override IEnumerator Await(Flow flow)
        {
            this.flow = flow;
            objectDeclarations = Variables.Object(flow.stack.gameObject);
            sceneDeclarations = Variables.Scene(flow.stack.scene);

            "Fetching diamond prices".Log();
            task = Services.CurrencyShopService.GetCurrencyShopProductsAsync(new GetCurrencyShopProductsRequest());

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Result.HasError)
            {
                "Fetching diamond prices has an error".LogError();

                flow.SetValue(error, task.Result.Error);
                yield return errorTrigger;
                yield break;
            }

            var items = task.Result.Result.items;

            if (items.Length <= 0)
            {
                "No items found".LogError();
                yield return errorTrigger;
                yield break;
            }

            "Fetching diamond prices complete".Log();

            flow.SetValue(successOutput, task.Result.Result.items);
            BeforeExit();
            yield return exit;
        }

        private void BeforeExit()
        {
            flow.SetValue(successOutput, task.Result.Result.items);
        }
    }
}
