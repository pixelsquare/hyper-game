using System.Collections.Generic;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.POC
{
    /// <summary>
    ///     Throwaway script for PoC.
    /// </summary>
    public class WallDisabler : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform = null;
        [SerializeField] private Vector3 playerOffset = Vector3.down * 3f;
        [SerializeField] private float castRadius = 1;
        [SerializeField] private LayerMask hitMask = 1 << 7;
        [SerializeField] private int hitCount = 10;
        [SerializeField] private int updateFrequency = 10;
        [SerializeField] private List<WallGroup> wallGroups = null;
        
        private Transform targetPlayer = null;
        private Slot<string> eventSlot = null;
        private Collider[] hitColliders = null;

#if UNITY_EDITOR
        private bool toggle = true;
        
        [ContextMenu("Toggle Display")]
        public void ToggleDisplay()
        {
            toggle = !toggle;
            foreach (var wallGroup in wallGroups)
            {
                wallGroup.ToggleVisiblity(toggle);
            }
        }
#endif

        private void OnPlayerInitialized(IEvent<string> callback)
        {
            var playerInitEvent = (QuantumPlayerJoinedEvent)callback;
            if (playerInitEvent.IsLocal)
            {
                targetPlayer = playerInitEvent.PlayerTransform;
            }
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerInitialized);
        }

        private void Start()
        {
            hitColliders = new Collider[hitCount];
            var mainCam = GameObject.FindWithTag("HangoutCamera");
            cameraTransform = mainCam.transform;
        }
        
        private void OnDestroy()
        {
            eventSlot.Dispose();
        }

        private void FixedUpdate()
        {
            if (Time.frameCount % updateFrequency != 0)
            {
                return;
            }
            
            if (targetPlayer == null)
            {
                return;
            }

            var startPos = cameraTransform.position;
            var endPos = targetPlayer.position + playerOffset;
            Debug.DrawLine(startPos, endPos, Color.red);

            var colliderCount = Physics.OverlapCapsuleNonAlloc(startPos, endPos, castRadius, hitColliders, hitMask);
            
            wallGroups.ForEach(a => a.ToggleVisiblity(true));

            for (var i = 0; i < colliderCount; i++)
            {
                var colliderGo = hitColliders[i].gameObject;
                var wallGroupGoIdx = wallGroups.FindIndex(a => a.gameObject == colliderGo);
            
                if (wallGroupGoIdx <= -1)
                {
                    continue;
                }
                
                wallGroups[wallGroupGoIdx].ToggleVisiblity(false);
            }
        }
    }
}
