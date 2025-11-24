using UnityEngine;
using Zenject;
using Facebook.Unity;

namespace Santelmo.Rinsurv
{
    public class AppStartup : MonoBehaviour
    {
        [Inject] private IStateMachine _stateMachine;

        //TODO: Move Facebook initialization somewhere
        #region Facebook Initialization
        
        void Awake ()
        {
            if (!FB.IsInitialized) 
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback);
            } else {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
        }

        private void InitCallback ()
        {
            if (FB.IsInitialized) 
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            } 
            else 
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }
        
        #endregion

        private void OnEnable()
        {
            _stateMachine.Play();
        }
    }
}
