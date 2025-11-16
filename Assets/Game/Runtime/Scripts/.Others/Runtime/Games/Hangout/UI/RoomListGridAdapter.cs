using System.Collections.Generic;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.DataHelpers;
using Kumu.Kulitan.Multiplayer;

namespace Kumu.Kulitan.Hangout
{
	public class RoomListGridAdapter : GridAdapter<GridParams, RoomListGridItemViewsHolder>
	{
		public SimpleDataHelper<RoomDetailsHolder> Data { get; private set; }
		private List<RoomListGridItemViewsHolder> activeHolders;
		
		public List<RoomListGridItemViewsHolder> ActiveRoomViewHolders => activeHolders;

		protected override void Start()
		{
			Data = new SimpleDataHelper<RoomDetailsHolder>(this);
			activeHolders = new List<RoomListGridItemViewsHolder>();
			
			base.Start();
		}
		
		protected override void UpdateCellViewsHolder(RoomListGridItemViewsHolder newOrRecycled)
		{
			activeHolders.Add(newOrRecycled);

			var roomData = Data[newOrRecycled.ItemIndex];

			var roomButton = newOrRecycled.myButton;
			roomButton.UpdatePlayerCount(roomData.roomCount, roomData.maxPlayersCount);
			roomButton.SetRoomName(roomData.roomName);
			roomButton.SetOwnerName(roomData.roomOwner);
			roomButton.LoadIcon(roomData.previewIconAddress);
			roomButton.RoomId = roomData.roomId;
			roomButton.SceneName = roomData.sceneName;
			roomButton.LevelConfig = roomData.levelConfig;
		}

		protected override void OnBeforeRecycleOrDisableCellViewsHolder(RoomListGridItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			base.OnBeforeRecycleOrDisableCellViewsHolder(inRecycleBinOrVisible, newItemIndex);
			activeHolders.Remove(inRecycleBinOrVisible);
		}

		public void SetItems(IList<RoomDetailsHolder> items)
		{
			Data.ResetItems(items);
		}
		
		public void EnableButtons(bool isEnabled)
		{
			foreach (var holder in activeHolders)
			{
				holder.myButton.SetButtonInteractable(isEnabled);
			}
		}
	}
	
	//room details model
	public class RoomDetailsHolder
	{
		public string roomId;
		public string roomName;
		public string roomOwner;
		public string sceneName;
		public int roomCount;
		public int maxPlayersCount;
		public string previewIconAddress;
		public LevelConfigScriptableObject levelConfig;

		public RoomDetailsHolder(string _roomId, string _roomName, string _sceneName, string _roomOwner, int _roomCount, 
			int _maxPlayersCount, string _previewIconAddress, LevelConfigScriptableObject _levelConfig)
		{
			roomId = _roomId;
			roomName = _roomName;
			sceneName = _sceneName;
			roomOwner = _roomOwner;
			roomCount = _roomCount;
			maxPlayersCount = _maxPlayersCount;
			previewIconAddress = _previewIconAddress;
			levelConfig = _levelConfig;
		}
	}
	
	//holder class for cell's view objects
	public class RoomListGridItemViewsHolder : CellViewsHolder
	{
		public HangoutLobbyRoomButton myButton;
		
		public override void CollectViews()
		{
			base.CollectViews();
			myButton = root.GetComponent<HangoutLobbyRoomButton>();
		}
	}
}
