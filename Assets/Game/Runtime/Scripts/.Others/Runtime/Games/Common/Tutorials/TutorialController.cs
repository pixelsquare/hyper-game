using System.Collections;
using System.Collections.Generic;
using Kumu.Extensions;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.Common
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private float delay = 1f;
        
        [SerializeField] private SerializableDictionary<string, SlidePagesCollection> scene2PagesMap;

        [SerializeField] private SlidePagesCollection gameScenePages;

        private Dictionary<string, SlidePagesCollection> evtId2PagesMap = new Dictionary<string, SlidePagesCollection>();
        private WaitForSeconds waitForSeconds;
        private Slot<string> eventSlot = new Slot<string>(GlobalNotifier.Instance);

        private void HandleActiveSceneChangedEvent(Scene arg0, Scene arg1)
        {
            if (!scene2PagesMap.TryGetValue(arg1.name, out var slideCollection))
            {
                return;
            }

            if (Tutorial.IsViewedPreviously(slideCollection))
            {
                GlobalNotifier.Instance.Trigger(new Tutorial.TutorialDismissedEvent());
                return;
            }

            StartCoroutine(InvokeDelayedCoroutine(slideCollection));
        }

        private IEnumerator InvokeDelayedCoroutine(SlidePagesCollection slidePagesCollection)
        {
            yield return waitForSeconds;

            Tutorial.Show(slidePagesCollection);
        }

        private void HandleShowTutorialEvent(IEvent<string> evt)
        {
            var eventData = (Tutorial.ShowTutorialEvent)evt;
            var tutorialId = eventData.TutorialId;
            
            if (!evtId2PagesMap.TryGetValue(tutorialId, out var slidePagesCollection))
            {
                $"Tutorial with id {eventData.TutorialId} is not assigned!".LogError();
                return;
            }
            
            Tutorial.Show(slidePagesCollection);
        }

        private void HandleDismissedTutorialEvent(IEvent<string> obj)
        {
            Tutorial.Close();
        }

        private void GameSceneLoadedEventHandler(IEvent<string> obj)
        {
            if (Tutorial.IsViewedPreviously(gameScenePages))
            {
                return;
            }
            
            Tutorial.Show(gameScenePages);
        }

        private void SceneLoadingEventHandler(IEvent<string> obj)
        {
            Tutorial.Close();
        }

        private void Start()
        {
            waitForSeconds = new WaitForSeconds(delay);
            foreach (var kvp in scene2PagesMap)
            {
                var id = kvp.Value.Id;
                var wasAdded = evtId2PagesMap.TryAdd(id, kvp.Value);
                if (!wasAdded)
                {
                    $"Page with {id} already exists in the dictionary".LogError();
                }
            }
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += HandleActiveSceneChangedEvent;
            eventSlot.SubscribeOn(Tutorial.ShowTutorialEvent.EVENT_NAME, HandleShowTutorialEvent);
            eventSlot.SubscribeOn(Tutorial.TutorialDismissedEvent.EVENT_NAME, HandleDismissedTutorialEvent);
            eventSlot.SubscribeOn(GameSceneLoadedEvent.EVENT_NAME, GameSceneLoadedEventHandler);
            eventSlot.SubscribeOn(SceneLoadingEvent.EVENT_NAME, SceneLoadingEventHandler);
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= HandleActiveSceneChangedEvent;
            eventSlot.UnSubscribeFor(Tutorial.ShowTutorialEvent.EVENT_NAME, HandleShowTutorialEvent);
            eventSlot.UnSubscribeFor(Tutorial.TutorialDismissedEvent.EVENT_NAME, HandleDismissedTutorialEvent);
            eventSlot.UnSubscribeFor(GameSceneLoadedEvent.EVENT_NAME, GameSceneLoadedEventHandler);
            eventSlot.UnSubscribeFor(SceneLoadingEvent.EVENT_NAME, SceneLoadingEventHandler);
        }
    }
}
