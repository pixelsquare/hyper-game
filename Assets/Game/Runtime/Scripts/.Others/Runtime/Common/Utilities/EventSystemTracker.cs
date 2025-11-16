using UnityEngine;
using UnityEngine.EventSystems;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    /// Component that handles destroying redundant EventSystems.
    /// Place this in a scene that is expected to be used as an Additive scene, and as a single scene when testing.
    /// </summary>
    [RequireComponent(typeof(EventSystem))]
    public class EventSystemTracker : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;

        private void Start()
        {
            if (EventSystem.current != eventSystem)
            {
                Destroy(gameObject);
            }
        }

        private void Reset()
        {
            eventSystem = GetComponent<EventSystem>();
        }
    }
}