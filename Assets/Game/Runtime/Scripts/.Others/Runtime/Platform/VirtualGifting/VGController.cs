using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Gifting
{
    public class VGController : MonoBehaviour
    {
        private struct GiftJob
        {
            public VirtualGiftGiftsData[] giftData;
            public VirtualGiftGifteeData[] gifteeData;
            public string[] spectators;
        }
        
        public static VirtualGiftData.VGType vgSelectedType;

        [SerializeField] private VirtualGiftGiftsData[] vgData;
        [SerializeField] private VirtualGiftGifteeData[] vgGifteeData;
        [SerializeField] private UnityEvent onPlayerSelected;

        private Queue<GiftJob> giftJobs = new();
        private VGGiftListController listController;
        private VGPlayerListController playerListController;
        private Slot<string> eventSlot;

        public VirtualGiftDatabase VgDatabase => VirtualGiftDatabase.Current;

        private static string LocalId => UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId;

        public VirtualGiftGiftsData[] Gifts => vgData;

        public VirtualGiftGifteeData[] Giftees
        {
            get => vgGifteeData;
            set => vgGifteeData = value;
        }

        public VGGiftListController vgGiftListController => listController;

        private void OnVGQueued(IEvent<string> obj)
        {
            var eventInfo = (VGSelectedEvent)obj;
            var giftData = eventInfo.VgSendData;
            var gifteeData = vgSelectedType == VirtualGiftData.VGType.Shared ? VGPlayerListController.allPlayersGifteeData.ToArray() : vgGifteeData;
            var spectators = playerListController.GetPlayerIds()
                                                 .Except(gifteeData.Select(giftee => giftee.id).Append(LocalId))
                                                 .ToArray();
            
            giftJobs.Enqueue(new GiftJob
            {
                giftData = giftData,
                gifteeData = gifteeData,
                spectators = spectators
            });
        }
        
        // Processes jobs as they come in
        private IEnumerator ProcessQueueCoroutine()
        {
            while (true)
            {
                if (!giftJobs.TryDequeue(out var giftJob))
                {
                    yield return null;
                    continue;
                }

                yield return ProcessTaskCoroutine(giftJob);
            }
        }

        private IEnumerator ProcessTaskCoroutine(GiftJob giftJob)
        {
            ServiceResultWrapper<SendVirtualGiftResult> result;
            
            if (giftJob.gifteeData.Length <= 0 || giftJob.giftData.Length <= 0)
            {
                "giftees or gifts data is empty".LogError();
                result = new ServiceResultWrapper<SendVirtualGiftResult>(new ServiceError(ServiceErrorCodes.INVALID_DATA, "Gifts or giftee array is empty!"));
            }
            else
            {
                var gifteeIds = giftJob.gifteeData.Select(gifteeData => gifteeData.id).ToArray();

                var request = new SendVirtualGiftRequest
                {
                    giftees = gifteeIds,
                    gifts = giftJob.giftData,
                    category = VirtualGiftData.VGType.Individual,
                    spectators = giftJob.spectators
                };

                var task = Services.VirtualGiftService.SendVirtualGiftAsync(request);
                yield return new WaitUntil(() => task.IsCompleted || task.IsCanceled);

                if (task.IsCanceled)
                {
                    "Task was cancelled!".LogError();
                    result = new ServiceResultWrapper<SendVirtualGiftResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Gifting task was cancelled!"));
                    result = task.Result;
                    yield break;
                }

                result = task.Result;
            }

            if (result.HasError)
            {
                $"Send gifts attempt failed. Error details:\n{result.Error}".LogError();
                var refund = VgDatabase.ComputeCosts(giftJob.giftData);
                listController.RefundToCachedWalletBalance(refund);
                GlobalNotifier.Instance.Trigger(VGFailedToSendEvent.EVENT_NAME);
                yield break;
            }

            listController.SetGiftAvailability(true);
        }

        private void OnVGTypeSelected(IEvent<string> callback)
        {
            var eventInfo = (VGTypeSelectedEvent)callback;
            vgSelectedType = eventInfo.Type;
        }

        private void OnVGPlayerSelected(IEvent<string> callback)
        {
            var eventInfo = (
                VGPlayerSelected)callback;
            vgGifteeData = eventInfo.VGGifteeData;
            onPlayerSelected?.Invoke();
        }

        private void Awake()
        {
            listController = FindObjectOfType<VGGiftListController>();
            playerListController = FindObjectOfType<VGPlayerListController>();
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(VGTypeSelectedEvent.EVENT_NAME, OnVGTypeSelected);
            eventSlot.SubscribeOn(VGSelectedEvent.EVENT_NAME, OnVGQueued);
            eventSlot.SubscribeOn(VGPlayerSelected.EVENT_NAME, OnVGPlayerSelected);
        }

        private void Start()
        {
            StartCoroutine(ProcessQueueCoroutine());
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
