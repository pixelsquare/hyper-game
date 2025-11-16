using System.Collections;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitRegisterFCMToken : Unit
    {
        [DoNotSerialize]
        public ValueOutput error;
        
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;
        
        [DoNotSerialize]
        public ControlOutput errorTrigger;

        protected override void Definition()
        {
            error = ValueOutput<ServiceError>(nameof(error));
            
            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), RunCoroutine);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            errorTrigger = ControlOutput(nameof(errorTrigger));
        
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator RunCoroutine(Flow flow)
        {
            var request = new RegisterFCMTokenRequest();
            request.token = FCMService.FcmToken;

            $"[UnitRegisterFCMToken] Registering token {FCMService.FcmToken}".Log();
                
            BackendUtil.SetFCMToken(FCMService.FcmToken);

            var task = Services.FCMService.RegisterFCMTokenAsync(request);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Result.HasError)
            {
                $"[UnitRegisterFCMToken] Error send token - {task.Result.Error}".Log();
                flow.SetValue(error, task.Result.Error);
                yield return errorTrigger;
                yield break;
            }

            $"[UnitRegisterFCMToken] Token registered.".Log();
            
            yield return outputTrigger;
        }
    }
}
