using System.Collections;
using Facebook.Unity;
using Kumu.Extensions;
using Unity.VisualScripting;
using UnityEngine;


namespace Kumu.Kulitan.VisualScripting
{
    public class UnitInitializeFacebook : WaitUnit
    {
        [DoNotSerialize] public ControlOutput errorTrigger;
        private bool isInitProcessDone;
        private bool hasError;
        
        protected override void Definition()
        {
            base.Definition();
            errorTrigger = ControlOutput(nameof(errorTrigger));
            Succession(enter, errorTrigger);
        }

        protected override IEnumerator Await(Flow flow)
        {
            "Initializing Facebook".Log();

            Init();
            yield return new WaitUntil(() => isInitProcessDone);
            
            if (hasError)
            {
                yield return errorTrigger;
            }

            yield return exit;
        }
        
        private void Init()
        {
            if (!FB.IsInitialized)
            {
                FB.Init(InitCallback);
                return;
            }
            FB.ActivateApp();
            "Facebook sdk initialized".Log();
        }

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                "Facebook sdk initialized".Log();
            }
            else
            {
                hasError = true;
                "Something went wrong while initializing facebook sdk".LogError();
            }

            isInitProcessDone = true;
        }
    }
}
