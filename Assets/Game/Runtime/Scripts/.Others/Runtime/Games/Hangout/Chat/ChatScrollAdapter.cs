using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using TMPro;

namespace Kumu.Kulitan.Hangout
{
	public class ChatScrollAdapter : OSA<BaseParamsWithPrefab, ChatMessageViewsHolder>
	{
		public SimpleDataHelper<ChatMessage> Data { get; private set; }

		[SerializeField] int maxMessages = 25;
		[SerializeField] MessageColorStruct messageColors;
		[SerializeField] List<ChatMessage> messageList = new List<ChatMessage>();

		//TODO - preparing in case needed
		public void Clear()
		{ 
			RemoveItemsFrom(0, Data.Count);

			messageList.Clear();
		}
		
		//OSA implementation
		protected override void Awake()
		{
			base.Awake();

			Data = new SimpleDataHelper<ChatMessage>(this);
		}
		
		protected override ChatMessageViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new ChatMessageViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}
		
		protected override void OnItemHeightChangedPreTwinPass(ChatMessageViewsHolder vh)
		{
			base.OnItemHeightChangedPreTwinPass(vh);

			Data[vh.ItemIndex].hasPendingVisualSizeChange = false;
		}
		
		//update visual elements
		protected override void UpdateViewsHolder(ChatMessageViewsHolder newOrRecycled)
		{
			ChatMessage model = Data[newOrRecycled.ItemIndex];

			string message = model.text;
			newOrRecycled.messageText.text = message;
			newOrRecycled.messageText.color = GetMessageColor(model.messageType);

			if (model.hasPendingVisualSizeChange)
			{
				newOrRecycled.MarkForRebuild();
				ScheduleComputeVisibilityTwinPass();
			}
		}
		
		protected override void RebuildLayoutDueToScrollViewSizeChange()
		{
			SetAllModelsHavePendingSizeChange();

			base.RebuildLayoutDueToScrollViewSizeChange();
		}

		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			if (changeMode == ItemCountChangeMode.RESET)
				SetAllModelsHavePendingSizeChange();

			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}

		private void SetAllModelsHavePendingSizeChange()
		{
			foreach (var model in Data)
				model.hasPendingVisualSizeChange = true;
		}
		
		public void RemoveItemsFrom(int index, int count)
		{
			Data.RemoveItems(index, count);
			Data.NotifyListChangedExternally();
		}
		
		//adds text to the scroll view
		public void AddTextToDisplay(string text, ChatMessage.MessageType messageType)
		{
			if (messageList.Count >= maxMessages)
			{
				messageList.Remove(messageList[0]);
				RemoveItemsFrom(0, 1);
			}

			ChatMessage newMessage = new ChatMessage();
			newMessage.messageType = messageType;
			newMessage.text = text;
			
			messageList.Add(newMessage);
			Data.InsertOne(Data.Count, newMessage);
			Data.NotifyListChangedExternally();

			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(WaitThenScrollToEnd());
			}
		}
		
		private Color GetMessageColor(ChatMessage.MessageType messageType)
		{
			Color color = messageColors.infoColor;

			switch (messageType)
			{
				case ChatMessage.MessageType.PlayerMessage:
					color = messageColors.playerColor;
					break;
				case ChatMessage.MessageType.ChannelMessage:
					color = messageColors.channelColor;
					break;
				case ChatMessage.MessageType.PeerMessage:
					color = messageColors.peerColor;
					break;
				case ChatMessage.MessageType.Error:
					color = messageColors.errorColor;
					break;
			}

			return color;
		}

		private IEnumerator WaitThenScrollToEnd()
		{
			yield return new WaitForSeconds(0.025f);
			
			SetNormalizedPosition(0);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (!IsInitialized)
			{
				return;
			}
			Data.NotifyListChangedExternally();
			StartCoroutine(WaitThenScrollToEnd());
		}
	}

	//model for the data
	[System.Serializable]
	public class ChatMessage
	{
		public string text;
		public MessageType messageType;
		
		public bool hasPendingVisualSizeChange = true;

		public enum MessageType
		{
			Info,
			Error,
			PlayerMessage,
			ChannelMessage,
			PeerMessage
		}
	}

	[System.Serializable]
	public struct MessageColorStruct
	{
		public Color infoColor, errorColor, playerColor, peerColor, channelColor;
	}
	
	//references the visual elements for the scroll view
	public class ChatMessageViewsHolder : BaseItemViewsHolder
	{
		public TextMeshProUGUI messageText;
		public Image messageContentPanelImage;

		private ContentSizeFitter sizeFitter { get; set; }
		
		private VerticalLayoutGroup layoutGroup;

		public override void CollectViews()
		{
			base.CollectViews();

			sizeFitter = root.GetComponent<ContentSizeFitter>();
			sizeFitter.enabled = false;
			root.GetComponentAtPath("MessageContentPanel", out layoutGroup);
			messageContentPanelImage = layoutGroup.GetComponent<Image>();
			messageContentPanelImage.transform.GetComponentAtPath("Message", out messageText);
		}

		public override void MarkForRebuild()
		{
			base.MarkForRebuild();
			if (sizeFitter)
				sizeFitter.enabled = true;
		}

		public override void UnmarkForRebuild()
		{
			if (sizeFitter)
				sizeFitter.enabled = false;
			base.UnmarkForRebuild();
		}
	}
}
