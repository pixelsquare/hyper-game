using System.Linq;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Kumu.Kulitan.Tracking;
using Newtonsoft.Json;
using Quantum;
using UnityEngine;
using Constants = Kumu.Kulitan.Common.Constants;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveObjectView : MonoBehaviour
    {
        [SerializeField] private EntityView entityView;
        [SerializeField] private float playerProximityThreshold;
        [SerializeField] private QuantumStaticBoxCollider3D[] ignoredColliders;

        public EntityRef ModelEntityRef => entityView.EntityRef;

        private Transform playerToTrack;
        private bool canTrackPlayer;
        private bool inProximity;
        private Slot<string> eventSlot;

        private void OnObjectInteractedLocal(EventOnObjectInteractedLocal eventData)
        {   
            canTrackPlayer = !eventData.isInteracting;

            if (!canTrackPlayer)
            {
                ChangeUIVisibility(false);
            }
        }

        private void CheckUIVisibility()
        {
            if (transform == null || playerToTrack == null)
            {
                return;
            }

            var objDistance = Vector3.Distance(playerToTrack.position, entityView.transform.position);
            var objectPosition = entityView.transform.position;
            
            var allHits = Physics.RaycastAll(objectPosition, 
                playerToTrack.position - objectPosition, objDistance);
            var hasBlocker = false;

            foreach (var hit in allHits)
            {
                var hitQuantumCollider = hit.collider.gameObject.GetComponent<QuantumStaticBoxCollider3D>();
                
                if (hitQuantumCollider != null && 
                    !hit.collider.bounds.Contains(entityView.transform.position) &&
                    !ignoredColliders.Contains(hitQuantumCollider))
                {
                    hasBlocker = true;
                    break;
                }
            }

            var playerDistance = Vector3.Distance(playerToTrack.position, transform.position);
            
            if (!inProximity && 
                playerDistance <= playerProximityThreshold && 
                !hasBlocker)
            {
                ChangeUIVisibility(true);
            }
            else if (inProximity && playerDistance >= playerProximityThreshold)
            {
                ChangeUIVisibility(false);
            }
            else if (inProximity && playerDistance <= playerProximityThreshold)
            {
                ChangeUIVisibility(true);
            }
        }

        private void ChangeUIVisibility(bool isVisible)
        {
            GlobalNotifier.Instance.Trigger(new PlayerNearInteractiveObjEvent(entityView.EntityRef, isVisible));
            inProximity = isVisible;
        }

        private void OnPlayerInitialized(IEvent<string> callback)
        {
            if (callback is not QuantumPlayerJoinedEvent playerJoinedEvent || !playerJoinedEvent.IsLocal)
            {
                return;
            }

            playerToTrack = playerJoinedEvent.PlayerTransform;
            canTrackPlayer = true;
        }

        private void TrackInteract(EventOnObjectInteractedLocal eventData)
        {
            if (eventData.isInteracting
                && eventData.objInteracted == entityView.EntityRef
                && TryGetHangoutId(out var hangoutId))
            {
                var localProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();                                

                var interactEvent = new InteractEvent
                (
                    
                    localProfile.accountId,
                    1, 
                    hangoutId,
                    name
                );
                
                GlobalNotifier.Instance.Trigger(interactEvent);
            }
        }

        private bool TryGetHangoutId(out string hangoutId) // todo: convert into a utility method
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            hangoutId = null;
            
            if (currentRoom == null
                || !currentRoom.CustomProperties.TryGetValue(Constants.ROOM_DETAILS_PROP_KEY, out var roomDetailsObj))
            {
                "Failed to send Interaction analytics.".LogError();
                return false;
            }

            var roomDetails = JsonConvert.DeserializeObject<RoomDetails>(roomDetailsObj.ToString());
            hangoutId = roomDetails.layoutName;

            return true;
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerInitialized);
        }

        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnObjectInteractedLocal>(this, OnObjectInteractedLocal);
            QuantumEvent.Subscribe<EventOnObjectInteractedLocal>(this, TrackInteract);
        }
        
        public void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }

        private void Update()
        {
            if (canTrackPlayer)
            {
                CheckUIVisibility();
            }
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
