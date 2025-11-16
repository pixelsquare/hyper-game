using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Gifting;
using Kumu.Kulitan.Multiplayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#if !USE_OLD_API_METHODS
using Kumu.Extensions;
#endif

namespace Kumu.Kulitan.Backend
{
    public class VirtualGiftService : IVirtualGiftService // TODO: Request notification permission in sign in screen
    {
        public const string DATA_EVENT_KEY = "VirtualGiftReceivedInRoom";

        private readonly GetAllGiftItemsResultConverter getAllGiftItemsResultConverter = new();

        public Action<VirtualGiftEventInfo> VirtualGiftReceivedEvent { get; set; }

        public VirtualGiftService()
        {
            GlobalNotifier.Instance.SubscribeOn(RemoteDataNotificationReceivedEvent.EVENT_NAME, RemoteDataNotificationReceivedEventHandler);
        }

        public async Task<ServiceResultWrapper<GetVirtualGiftCostsResult>> GetVirtualGiftCostsAsync(GetVirtualGiftCostsRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetVirtualGiftCostsRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/gift");
            using var req = BackendUtil.Get(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetVirtualGiftCostsResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetVirtualGiftCostsResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetVirtualGiftCostsAsync(request));
                }

                var payloadDataAsString = payload.data.ToString();
                var res = JsonConvert.DeserializeObject<GetVirtualGiftCostsResult>(payloadDataAsString, getAllGiftItemsResultConverter);
                return new ServiceResultWrapper<GetVirtualGiftCostsResult>(res);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetVirtualGiftCostsResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "GetAllGiftItemCostsAsync failed", e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<SendVirtualGiftResult>> SendVirtualGiftAsync(SendVirtualGiftRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new SendVirtualGiftRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/gift/send");
            var reqBody = JsonConvert.SerializeObject(request);

            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                var allIds = request.giftees
                                    .Select(g => g.id)
                                    .Union(request.spectators)
                                    .Append(UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId)
                                    .ToArray();

                var isDistinct = allIds.Distinct().Count() == allIds.Length;

                if (!isDistinct)
                {
                    throw new ExitGames.Client.Photon.InvalidDataException("Ids are not distinct!");
                }

                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<SendVirtualGiftResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<SendVirtualGiftResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => SendVirtualGiftAsync(request));
                }

                var responseObject = JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
                {
                    updated_balance_timestamp = string.Empty,
                    updated_balance = Array.Empty<Currency>(),
                });

                var dateTimeWasParsed = DateTime.TryParse(responseObject.updated_balance_timestamp, out var timestamp);
                if (!dateTimeWasParsed)
                {
                    "Invalid datetime format".LogError();
                    return new ServiceResultWrapper<SendVirtualGiftResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Unknown error occured", "Invalid datetime format"));
                }

                return new ServiceResultWrapper<SendVirtualGiftResult>(new SendVirtualGiftResult
                {
                    updatedBalanceTimestamp = timestamp,
                    newWalletBalance = responseObject.updated_balance,
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<SendVirtualGiftResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "SendVirtualGiftAsync failed", e.Message));
            }
#endif
        }

        private void RemoteDataNotificationReceivedEventHandler(IEvent<string> obj)
        {
            if (BaseMatchmakingHandler.State != BaseMatchmakingHandler.RoomStatus.IN_ROOM)
            {
                return;
            }

            var remoteDataNotification = (RemoteDataNotificationReceivedEvent)obj;

            var message = remoteDataNotification.Message;

            if (!message.Data.TryGetValue("event", out var eventKey))
            {
                return;
            }

            if (eventKey != DATA_EVENT_KEY)
            {
                return;
            }

            var virtualGiftEventInfo = new VirtualGiftEventInfo // (cj) convert using json?
            {
                category = Enum.Parse<VirtualGiftData.VGType>(message.Data["category"]),
                giftees = JsonConvert.DeserializeObject<string[]>(message.Data["giftees"]),
                gifts = JsonConvert.DeserializeObject<Dictionary<string, string>>(message.Data["gifts"])
                                   .Select(kvp => new VirtualGiftGiftsData
                                   {
                                       id = kvp.Key, 
                                       quantity = Int32.Parse(kvp.Value)
                                   }).ToArray(),
                gifter = message.Data["gifter"]
            };

            VirtualGiftReceivedEvent?.Invoke(virtualGiftEventInfo);
        }
    }

    internal class GetAllGiftItemsResultConverter : JsonConverter<GetVirtualGiftCostsResult>
    {
        public override bool CanWrite => false;

        public override bool CanRead => true;

        public override void WriteJson(JsonWriter writer, GetVirtualGiftCostsResult value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override GetVirtualGiftCostsResult ReadJson(JsonReader reader, Type objectType, GetVirtualGiftCostsResult existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token.Type == JTokenType.Null)
            {
                return null;
            }

            var items = new List<VirtualGiftCostData>();
            foreach (var i in token["items"].ToObject<IDictionary<string, object>[]>())
            {
                items.Add(new VirtualGiftCostData
                {
                    id = i["id"].ToString(),
                    cost = JsonConvert.DeserializeObject<Currency>(i["cost"].ToString()),
                    markUpDownCost = Int32.Parse(i["mark_up_down_cost"].ToString())
                });
            }

            return new GetVirtualGiftCostsResult { itemCosts = items.ToArray() };
        }
    }
}
