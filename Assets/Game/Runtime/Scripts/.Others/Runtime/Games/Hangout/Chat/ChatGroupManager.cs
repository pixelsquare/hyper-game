using System.Collections;
using Kumu.Kulitan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
using UniSoftwareKeyboardArea;
#endif

namespace Kumu.Kulitan.Hangout
{
    public class ChatGroupManager : MonoBehaviour
    {
        [Header("Chat group items")]
        [SerializeField] private RectTransform rootElement;
        [SerializeField] private Image rootElementImage;
        [SerializeField] private GameObject draggableCloser;
        [SerializeField] private TMP_InputField textInput;
        [SerializeField] private Button buttonEnterText;
        [SerializeField] private ChatScrollAdapter enableModeChatAdapter;
        [SerializeField] private GameObject chatScrollbar;
        [SerializeField] private float enabledRootElementHeight = 1300;
        [SerializeField] private float disabledRootElementHeight = 1500;
        [SerializeField] private float waitTime = 0.3f;

        [Header("Other references")] 
        [SerializeField] private GameObject buttonOpenGroup;
        [SerializeField] private GameObject joystick;
        [SerializeField] private UICollapsibleMenuController menuController;
        [SerializeField] private GameObject muteButton;
        [SerializeField] private GameObject zoomButton;

        private float kbHeight;
        private bool isKeyboardShowing;

        public void AddTextToDisplay(string text, ChatMessage.MessageType messageType)
        {
            enableModeChatAdapter.AddTextToDisplay(text, messageType);
        }

        public void ToggleChatGroup(bool isEnabled)
        {
            draggableCloser.SetActive(isEnabled);
            rootElementImage.gameObject.SetActive(isEnabled);
            rootElement.sizeDelta = new Vector2(rootElement.sizeDelta.x, isEnabled ? enabledRootElementHeight : disabledRootElementHeight);
            textInput.gameObject.SetActive(isEnabled);
            chatScrollbar.SetActive(isEnabled);

            buttonOpenGroup.SetActive(!isEnabled);
            joystick.SetActive(!isEnabled);
            muteButton.SetActive(!isEnabled);
            zoomButton.SetActive(!isEnabled);

            if (!isEnabled && gameObject.activeInHierarchy)
            {
                StartCoroutine(WaitThenScrollToEnd());
            }
            
            if(isEnabled)
            {
                menuController.CollapseMenu();
            }
            
            menuController.gameObject.SetActive(!isEnabled);
        }

        public void ForceEndPosition()
        {
            enableModeChatAdapter.SetNormalizedPosition(0);
        }
        
        public void ToggleInputAvailability(bool isEnabled)
        {
            textInput.interactable = isEnabled;
            buttonEnterText.interactable = isEnabled;
        }

        public void AdjustForVirtualKeyboard(bool isShowing)
        {
#if UNITY_ANDROID || UNITY_IPHONE
            if (isShowing)
            {
                StartCoroutine(GetKeyboardHeight());
            }
            else
            {
                rootElement.anchoredPosition = new Vector2(rootElement.anchoredPosition.x, 0);
            }
#endif
        }
        
        private IEnumerator GetKeyboardHeight()
        {
#if UNITY_IOS
            yield return new WaitForSeconds(waitTime);
            kbHeight = TouchScreenKeyboard.area.height;

#elif UNITY_ANDROID && !UNITY_EDITOR
            yield return new WaitForSeconds(waitTime);
            kbHeight = SoftwareKeyboardArea.GetHeight(true);
#else
            yield return new WaitForSeconds(0);
#endif
            rootElement.anchoredPosition = new Vector2(rootElement.anchoredPosition.x, kbHeight);
        }
        
        private IEnumerator WaitThenScrollToEnd()
        {
            yield return new WaitForSeconds(0.025f);
            
            enableModeChatAdapter.SetNormalizedPosition(0);
        }

        private void LateUpdate()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            //ensure group is moved if keyboard is visible
            if (TouchScreenKeyboard.visible)
            {
                kbHeight = SoftwareKeyboardArea.GetHeight(true);
                rootElement.anchoredPosition = new Vector2(rootElement.anchoredPosition.x, kbHeight);
            }
            else
            {
                rootElement.anchoredPosition = new Vector2(rootElement.anchoredPosition.x, 0);
            }
#endif
#if UNITY_IPHONE
            //ensure group is moved if keyboard is visible
            if (TouchScreenKeyboard.visible)
            {
                kbHeight = TouchScreenKeyboard.area.height;
                rootElement.anchoredPosition = new Vector2(rootElement.anchoredPosition.x, kbHeight);
            }
            else
            {
                rootElement.anchoredPosition = new Vector2(rootElement.anchoredPosition.x, 0);
            }
#endif
        }
    }
}
