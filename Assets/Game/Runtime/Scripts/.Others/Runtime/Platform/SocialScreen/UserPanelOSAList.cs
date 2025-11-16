using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

namespace Kumu.Kulitan.UI
{
	public class UserPanelOSAList : OSA<BaseParamsWithPrefab, SocialUserPanelViewsHolder>
	{
		public SimpleDataHelper<UserProfileForSocialScreen> Data { get; private set; }

		#region OSA implementation
		protected override void Start()
		{
			Data = new SimpleDataHelper<UserProfileForSocialScreen>(this); 
			base.Start();
		}

		protected override SocialUserPanelViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new SocialUserPanelViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			return instance;
		}

		protected override void UpdateViewsHolder(SocialUserPanelViewsHolder newOrRecycled)
		{
			var model = Data[newOrRecycled.ItemIndex];
			newOrRecycled.SocialUserPanel.Initialize(model);
		}

		#endregion

		#region data manipulation
		public void AddItemsAt(int index, IList<UserProfileForSocialScreen> items)
		{
			Data.InsertItems(index, items);
		}

		public void RemoveItemsFrom(int index, int count)
		{
			Data.RemoveItems(index, count);
		}

		public void SetItems(IList<UserProfileForSocialScreen> items)
		{
			Data.ResetItems(items);
		}
		#endregion

		/// <remarks>
		/// Only use this for pagination
		/// </remarks>
		/// <param name="newItems"></param>
		public void OnDataRetrieved(IList<UserProfileForSocialScreen> newItems)
		{
			Data.InsertItemsAtEnd(newItems);
		}
	}
	
	// This class keeps references to an item's views.
	// Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
	public class SocialUserPanelViewsHolder : BaseItemViewsHolder
	{
		public SocialUserPanel SocialUserPanel;

		// Retrieving the views from the item's root GameObject
		public override void CollectViews()
		{
			base.CollectViews();
			SocialUserPanel = root.GetComponent<SocialUserPanel>();
		}
	}
}
