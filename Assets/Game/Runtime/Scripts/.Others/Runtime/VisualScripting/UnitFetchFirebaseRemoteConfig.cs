using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitFetchFirebaseRemoteConfig : WaitUnit
    {
        private FirebaseRemoteConfig RemoteConfig => FirebaseRemoteConfig.DefaultInstance;
        
        protected override IEnumerator Await(Flow flow)
        {
            yield return null;

            var task = FetchRemoteConfigAsync();

            while (!task.IsCompleted)
            {
                yield return null;
            }
            
            yield return exit;
        }

        private async Task FetchRemoteConfigAsync()
        {
            var timeSpan = TimeSpan.Zero;
#if UBE_RELEASE
                timeSpan =  new TimeSpan(12, 0, 0); // 12 hours
#endif
            try
            {
                await RemoteConfig.FetchAsync(timeSpan);

                if (RemoteConfig.Info.LastFetchStatus != LastFetchStatus.Success)
                {
                    $"[RemoteConfig] Fetch failed!".LogError();
                    return;
                }
                
                var fetchedRemotely = await RemoteConfig.ActivateAsync();
                var hideFacebook = RemoteConfig.GetValue(Constants.HIDE_FACEBOOK_SIGNIN_KEY);
                $"[RemoteConfig] Fetched config. Remote:{fetchedRemotely} Params:[HideFacebook: {hideFacebook.BooleanValue}]".Log();
                // Debug.Break();
            }
            catch (Exception e)
            {
                $"[RemoteConfig] Failed to fetch remote config values! Exception: {e}".LogError();
            }
        }
    }
}
