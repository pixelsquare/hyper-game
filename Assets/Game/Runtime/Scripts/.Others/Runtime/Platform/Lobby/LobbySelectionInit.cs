using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Lobby
{
    public class LobbySelectionInit : ListView<LobbySelectionView, LobbyConfig>
    {
        [ContextMenu("Display Lobbies")]
        public async void Display()
        {
            var request = new GetLobbyConfigsRequest();
            var response = await Services.LobbyService.GetLobbyConfigsAsync(request);
            Display(response.Result.lobbyConfigs);
        }

        protected override void OnCreate(LobbySelectionView element, LobbyConfig datum)
        {
            base.OnCreate(element, datum);
            element.name = $"{prefab.name}_{datum.id}";
        }
    }
}
