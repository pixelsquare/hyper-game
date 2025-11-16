using System;
using System.Collections.Generic;
using UnityEngine;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    public class PopupManager : SingletonMonoBehaviour<PopupManager>
    {
        [SerializeField] private RectTransform popupContainer;
        [SerializeField] private SerializableDictionary<PopupType, GameObject> popupMapping;

        private List<IPopup> activePopups = new();

        public enum PopupType
        {
            GenericTextPopup,
            GenericErrorPopup,
            ConfirmationPopup,
            UsernamePopup,
            MatchmakingPopup,
            CreateRoomPopup,
            RoomLayoutPopup,
            ReportPopup,
            NotificationPopup,
            VGPlayerListPopup
        }

        public void SetPopupContainer(RectTransform container)
        {
            popupContainer = container;
        }

        public void CloseAllPopups()
        {
            var popups = new List<IPopup>();
            popups.AddRange(activePopups);

            foreach (IPopup popup in popups)
            {
                // Prevents invoking of popup events since we're already clearing out popups. Fixes the ticket below.
                // https://kumutech.atlassian.net/jira/software/c/projects/UBE/boards/108?modal=detail&selectedIssue=UBE-1386
                popup.OnOpened = null;
                popup.OnClosed = null;
                popup.Close();
            }
            activePopups.Clear();
        }

        public void RemoveActivePopup(IPopup popupToRemove)
        {
            activePopups.Remove(popupToRemove);
        }

        public IPopup OpenTextPopup(string title, string message, string button, int priority = 0)
        {
            if (!IsPopupPriority(priority))
            {
                return null;
            }

            var popup = (GenericTextPopup)CreatePopup(PopupType.GenericTextPopup, priority);
            popup.SetDetails(title, message, button);
            popup.Open();
            return popup;
        }

        public IPopup OpenErrorPopup(string title, string message, string button, int priority = 0)
        {
            if (!IsPopupPriority(priority))
            {
                return null;
            }
            
            var popup = (GenericTextPopup)CreatePopup(PopupType.GenericErrorPopup, priority);
            popup.SetDetails(title, message, button);
            popup.Open();
            return popup;
        }

        public IPopup OpenConfirmationPopup(string title, string message, string confirmButton, 
            string cancelButton, int priority = 0)
        {
            if (!IsPopupPriority(priority))
            {
                return null;
            }

            var popup = (ConfirmationPopup)CreatePopup(PopupType.ConfirmationPopup, priority);
            popup.SetDetails(title, message, confirmButton, cancelButton);
            popup.Open();
            return popup;
        }

        public IPopup ShowUsernamePopup(Action<string> submitAction, int priority = 0)
        {
            if (!IsPopupPriority(priority))
            {
                return null;
            }

            var popup = (UsernamePopup)CreatePopup(PopupType.UsernamePopup, priority);
            popup.AddCallback(submitAction);
            popup.Open();
            return popup;
        }

        public IPopup ShowMatchmakingPopup(Action cancelAction, int priority = 0)
        {
            if (!IsPopupPriority(priority))
            {
                return null;
            }

            var popup = (MatchmakingPopup)CreatePopup(PopupType.MatchmakingPopup, priority);
            popup.AddCallback(cancelAction);
            popup.Open();
            return popup;
        }
        
        public IPopup ShowCreateRoomPopup(Action<RoomDetails, LevelConfigScriptableObject> submitAction, int priority = 0)
        {
            if (!IsPopupPriority(priority))
            {
                return null;
            }

            var popup = (CreateRoomPopup)CreatePopup(PopupType.CreateRoomPopup, priority);
            popup.AddCallback(submitAction);
            popup.Open();
            return popup;
        }
        
        public IPopup ShowRoomLayoutPopup(Action<RoomLayoutConfig> submitAction, int priority = 0)
        {      
            if (!IsPopupPriority(priority))
            {
                return null;
            }

            var popup = (RoomLayoutPopup)CreatePopup(PopupType.RoomLayoutPopup, priority);
            popup.AddCallback(submitAction);
            popup.Open();
            return popup;
        }

        public void OpenScenePopup(string popupName, UnityAction onSceneLoaded)
        {
            SceneLoadingManager.Instance.LoadSceneAsAdditive(popupName, onSceneLoaded);
        }

        public void OpenScenePopup(string popupName)
        {
            SceneLoadingManager.Instance.LoadSceneAsAdditive(popupName);
        }

        public void CloseScenePopup(string popupName)
        {
            SceneLoadingManager.Instance.UnloadAdditiveScene(popupName);
        }

        public IPopup CreatePopup(PopupType popupType, int priority)
        {
            var popupToSpawn = popupMapping[popupType];
            var popupInstance = Instantiate(popupToSpawn, popupContainer);

            var popup = popupInstance.GetComponent(typeof(IPopup)) as IPopup;
            popup.Priority = priority;
            activePopups.Add(popup);
            return popup;
        }

        // Todo: revise to add queuing system
        // Currently, method only checks for previously opened popup
        public bool IsPopupPriority(int priority)
        {
            foreach (var popup in activePopups)
            {
                if (priority > popup.Priority)
                {
                    popup.Close();
                    return true;
                }
                
                if (priority == popup.Priority)
                {   
                    return true;
                }

                if (priority < popup.Priority)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
