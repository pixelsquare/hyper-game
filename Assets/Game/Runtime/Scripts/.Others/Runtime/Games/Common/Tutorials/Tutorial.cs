using System.Linq;
using Kumu.Extensions;
using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.Common
{
    public static class Tutorial
    {
        public const string PREFS_KEY = "TutorialsViewed";
        
        public const string SCENE_NAME = "GenericTutorialPopup";

        private static bool SceneExists => SceneManager.GetSceneByName(SCENE_NAME).isLoaded;

        public static void Show(SlidePagesCollection slidePagesCollection)
        {
            if (SceneExists)
            {
                return;
            }
            
            SceneManager.LoadSceneAsync(SCENE_NAME, LoadSceneMode.Additive).completed += op =>
            {
                var genericTutorial = Object.FindObjectOfType<SlidePagesView>();
                genericTutorial.SetData(slidePagesCollection, SlidePagesViewButtonLabels.Default, () =>
                {
                    SetViewed(slidePagesCollection);
                    Close();
                });
            };
        }

        public static void Close()
        {
            if (!SceneExists)
            {
                return;
            }

            SceneManager.UnloadSceneAsync(SCENE_NAME);
            
            GlobalNotifier.Instance.Trigger(TutorialDismissedEvent.EVENT_NAME);
        }
        
        public static void SetViewed(SlidePagesCollection slidePagesCollection)
        {
            var tutorialsViewed = PlayerPrefs.GetString(PREFS_KEY).Split(",").ToHashSet();
            
            tutorialsViewed.Add(slidePagesCollection.Id);
            
            PlayerPrefs.SetString(PREFS_KEY, string.Join(',', tutorialsViewed));
            
            $"Recording tutorial {slidePagesCollection.Id} as viewed!".Log();
        }

        public static bool IsViewedPreviously(SlidePagesCollection slidePagesCollection)
        {
            if (!PlayerPrefs.HasKey(PREFS_KEY))
            {
                return false;
            }

            var tutorialsViewed = PlayerPrefs.GetString(PREFS_KEY).Split(',');

            return tutorialsViewed.Contains(slidePagesCollection.Id);
        }

        public static void ResetViewed()
        {
            PlayerPrefs.DeleteKey(PREFS_KEY);
        }

        public class ShowTutorialEvent : Event<string>
        {
            public const string EVENT_NAME = "ShowTutorial";

            public ShowTutorialEvent(string tutorialId) : base(EVENT_NAME)
            {
                TutorialId = tutorialId;
            }
            
            public string TutorialId { get; }
        }

        public class TutorialDismissedEvent : Event<string>
        {
            public const string EVENT_NAME = "TutorialDismissed";
            
            public TutorialDismissedEvent() : base(EVENT_NAME)
            {
            }
        }
    }
}
