using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class ToggleGroupEventDispatcher : MonoBehaviour
    {
        [Serializable]
        public class ToggleEvent
        {
            public Toggle toggle;
            public UnityEvent toggleEvent;
        }

        [SerializeField] private ToggleEvent[] toggleEvents;

        /// <summary>
        ///     FSM
        /// </summary>
        /// <param name="toggle"></param>
        public void DispatchEvent(Toggle toggle)
        {
            var e = Array.Find(toggleEvents, a => a.toggle == toggle);

            if (e != null)
            {
                e.toggleEvent?.Invoke();
            }
        }
    }
}
