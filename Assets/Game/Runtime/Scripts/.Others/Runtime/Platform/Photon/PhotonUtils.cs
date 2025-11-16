using Kumu.Kulitan.Common;
using Kumu.Kulitan.Hangout;
using Newtonsoft.Json;
using Photon.Realtime;

namespace Kumu.Kulitan.Multiplayer
{
    public static class PhotonUtils
    {
        public static RoomDetails GetRoomDetails(this Room room)
        {
            room.CustomProperties.TryGetValue(Constants.ROOM_DETAILS_PROP_KEY,out var roomDetailsObj);
            var result = JsonConvert.DeserializeObject<RoomDetails>(roomDetailsObj.ToString());

            return result;
        }

        public static PlayerDetails GetHostDetails(this Room room)
        {
            room.CustomProperties.TryGetValue(Constants.HOST_DETAILS_PROP_KEY, out var hostDetailsObj);
            var result = JsonConvert.DeserializeObject<PlayerDetails>(hostDetailsObj.ToString());

            return result;
        }
    }
}
