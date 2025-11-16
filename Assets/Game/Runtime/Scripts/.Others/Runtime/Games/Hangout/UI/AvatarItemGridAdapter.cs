using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.DataHelpers;
using Kumu.Kulitan.Avatar;

namespace Kumu.Kulitan.Hangout
{
	public class AvatarItemGridAdapter : GridAdapter<GridParams, AvatarItemGridItemViewsHolder>
	{
		public SimpleDataHelper<AvatarItemConfig> Data { get; private set; }

		[SerializeField] private ItemSelection itemSelect;
		[SerializeField] private ItemSelectionController selectionController;

		public List<AvatarItemGridItemViewsHolder> activeHolders { get; private set; }

		#region GridAdapter implementation
		protected override void Start()
		{
			Data = new SimpleDataHelper<AvatarItemConfig>(this);
			activeHolders = new List<AvatarItemGridItemViewsHolder>();
			
			base.Start();
		}

		//called when cell is created/becomes visible - do initialization here
		protected override void UpdateCellViewsHolder(AvatarItemGridItemViewsHolder newOrRecycled)
		{
			activeHolders.Add(newOrRecycled);
			
			var itemData = Data[newOrRecycled.ItemIndex];
			var avatarItem = newOrRecycled.myAvatarItem;
			avatarItem.ClearListeners();
			avatarItem.ShowLoadingIcon();
			avatarItem.wantedSprite = itemData.SpriteRef;
			var isOwned = UserInventoryData.IsItemOwned(itemData.Data.itemId);
			var isSelected = selectionController.IsItemSelected(itemData.Data.itemId);
			avatarItem.Initialize(itemData, true);
			avatarItem.AddListenerOnToggle(isSelected => itemSelect.OnItemSelected(itemData, isSelected));
			avatarItem.ToggleWithoutNotify(isSelected);
			avatarItem.ToggleIsOwned(isOwned);
			avatarItem.transform.SetAsLastSibling();
			selectionController.AddListenerOnSelect(itemData.Data.itemId, avatarItem.OnSelectAction);
		}

		//called before a cell is hidden/recycled - do de-init here
		protected override void OnBeforeRecycleOrDisableCellViewsHolder(AvatarItemGridItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			base.OnBeforeRecycleOrDisableCellViewsHolder(inRecycleBinOrVisible, newItemIndex);
			activeHolders.Remove(inRecycleBinOrVisible);
			var avatarItem = inRecycleBinOrVisible.myAvatarItem;
			avatarItem.ClearListeners();
			avatarItem.ShowLoadingIcon();
			avatarItem.ToggleWithoutNotify(false);

			if (Data.Count > 0)
			{
				var itemData = Data[inRecycleBinOrVisible.ItemIndex];
				selectionController.RemoveListenerOnSelect(itemData.Data.itemId, avatarItem.OnSelectAction);
			}
		}
		#endregion
		
		#region data manipulation
		public void AddItemsAt(int index, IList<AvatarItemConfig> items)
		{
			Data.List.InsertRange(index, items);
			Data.NotifyListChangedExternally();
		}

		public void RemoveItemsFrom(int index, int count)
		{
			Data.List.RemoveRange(index, count);
			Data.NotifyListChangedExternally();
		}
		#endregion

		public void NotifyOwnedItems()
		{
			foreach (var display in activeHolders)
			{
				var isOwned = UserInventoryData.IsItemOwned(display.myAvatarItem.AvatarItemId);
				display.myAvatarItem.ToggleIsOwned(isOwned);
			}
		}
		
		public void NotifyNewItems(IEnumerable<AvatarItemConfig> avatarItemConfigs)
		{
			foreach (var display in activeHolders)
			{
				var isOwned = avatarItemConfigs.Any(item => item.Data.itemId.Equals(display.myAvatarItem.AvatarItemId));
				display.myAvatarItem.ToggleIsNew(isOwned);
			}
		}
	}
	
	//holder class for cell's view objects
	public class AvatarItemGridItemViewsHolder : CellViewsHolder
	{
		public AvatarItem myAvatarItem;
		
		public override void CollectViews()
		{
			base.CollectViews();
			myAvatarItem = root.GetComponent<AvatarItem>();
		}
	}
}
