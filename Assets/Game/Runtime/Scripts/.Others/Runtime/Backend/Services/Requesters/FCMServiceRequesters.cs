using System.Threading.Tasks;
using Firebase.Messaging;
using Kumu.Extensions;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class RegisterFCMTokenRequester : Requester<RegisterFCMTokenRequest, RegisterFCMTokenResult>
    {
        public RegisterFCMTokenRequester(RegisterFCMTokenRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            $"FCM token: {BackendUtil.GlobalHeaders["x-ube-fcm-token"]}".Log();

            return BackendUtil.Put(BackendUtil.GetFullUrl("/api/v3/player/auth/fcm"));
        }

        public override RegisterFCMTokenResult FormResultFromPayload(HttpResponseObject payload)
        {
            var result = new RegisterFCMTokenResult();

            return result;
        }
    }
}
