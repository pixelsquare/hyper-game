using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Unity.VisualScripting;
using UnityEngine;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitInitializeFirebase : WaitUnit
    {
        private const int TimeoutPeriodInMilliseconds = 30000;

        [DoNotSerialize] public ControlOutput errorTrigger;
        private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        private bool hasError;

        private FirebaseRemoteConfig RemoteConfig => FirebaseRemoteConfig.DefaultInstance;

        protected override void Definition()
        {
            base.Definition();
            errorTrigger = ControlOutput(nameof(errorTrigger));
            Succession(enter, errorTrigger);
        }

        protected override IEnumerator Await(Flow flow)
        {
            "[FCMNotificationHandler] Checking Firebase dependencies...".Log();

            var cancellationTokenSource = new CancellationTokenSource(TimeoutPeriodInMilliseconds);
            var task = Task.Run(Init, cancellationTokenSource.Token);
            yield return new WaitUntil(() => task.IsCompleted);
            
            cancellationTokenSource.Dispose();

            if (hasError)
            {
                yield return errorTrigger;
            }

            yield return exit;
        }
        
        private async void Init()
        {
            try
            {
                // Init app
                await FirebaseApp.CheckAndFixDependenciesAsync()
                                 .ContinueWithOnMainThread(task =>
                                 {
                                     dependencyStatus = task.Result;
                                     if (dependencyStatus != DependencyStatus.Available)
                                     {
                                         hasError = true;
                                         $"[FCMNotificationHandler] Could not resolve all Firebase dependencies: {dependencyStatus}".LogError();
                                         return;
                                     }

                                     $"[FCMNotificationHandler] Dependencies resolved: {dependencyStatus}".Log();
                                 });

                // Init FCM
                Services.FCMService.Init();
                while (!Services.FCMService.IsInitialized)
                {
                    await Task.Yield();
                }

                // Init remote config
                var remoteConfigDefaults = new System.Collections.Generic.Dictionary<string, object>
                {
                    { Constants.HIDE_FACEBOOK_SIGNIN_KEY, true }
                };
                await RemoteConfig.SetDefaultsAsync(remoteConfigDefaults);
                "[RemoteConfig] Initialized!".Log();

                // Init analytics
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                // Finished initializing firebase
                "Finished initializing firebase.".Log();
            }
            catch (OperationCanceledException e)
            {
                hasError = true; 
                $"Firebase initialization timed out! Exception: {e}".LogError();
            }
            catch(Exception e)
            {
                hasError = true; 
                $"Error initializing FCM. Exception: {e}".LogError();
            }
        }
    }
}
