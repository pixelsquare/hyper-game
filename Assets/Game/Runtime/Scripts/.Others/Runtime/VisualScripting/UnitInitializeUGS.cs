using System.Collections;
using Kumu.Extensions;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitInializeUGS : WaitUnit
    {
        private bool isInitProcessDone;

        protected override IEnumerator Await(Flow flow)
        {
            Init();
            yield return new WaitUntil(() => isInitProcessDone);
            "Finished initializing Unity Gaming Services!".Log();

            yield return exit;
        }

        private async void Init()
        {
            // Init app
            "Initializing Unity gaming services...".Log();
            await UnityServices.InitializeAsync(new InitializationOptions()); // uses default "production" UGS environment
            isInitProcessDone = true;
        }
    }
}
