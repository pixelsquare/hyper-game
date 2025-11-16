using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class UIBasicToggle : MonoBehaviour
    {
        [SerializeField] private Button[] toggleButtons;
        [SerializeField] private Transform rotatingElement;
        [SerializeField] private CanvasGroup[] objectsToEnable;
        [SerializeField] private CanvasGroup[] objectsToDisable;
        [SerializeField] private bool hasButtonAnimation;
        [SerializeField] private float duration = 0.3f;
        
        private bool isOn = false;
        private bool toggleEnabled = true;

        private void OnToggled()
        {
            if (!toggleEnabled)
            {
                return;
            }

            toggleEnabled = false;
            isOn = !isOn;

            if (hasButtonAnimation)
            {
                rotatingElement.DORotate(isOn ? Vector3.forward : Vector3.back * 90f, duration);
            }

            if (isOn)
            {
                EnableObjects();
                return;
            }
            
            DisableObjects();
        }

        private void EnableObjects()
        {
            foreach (var obj in objectsToEnable)
            {
                obj.gameObject.SetActive(true);
                obj.DOFade(1, duration)
                    .OnComplete(() => toggleEnabled = true);
            }
            
            foreach (var obj in objectsToDisable)
            {
                obj.DOFade(isOn ? 0 : 1, duration)
                    .OnComplete(() =>
                    {
                        obj.gameObject.SetActive(false);
                        toggleEnabled = true;
                    });
            }

            if (hasButtonAnimation)
            {
                rotatingElement.DORotate(Vector3.zero, duration);
            }
        }

        private void DisableObjects()
        {
            foreach (var obj in objectsToEnable)
            {
                obj.DOFade(isOn ? 0 : 1, duration)
                    .OnComplete(() =>
                    {
                        obj.gameObject.SetActive(false);
                        toggleEnabled = true;
                    });
            }
            
            foreach (var obj in objectsToDisable)
            {
                obj.gameObject.SetActive(true);
                obj.DOFade(1, duration)
                    .OnComplete(() =>
                    {
                        toggleEnabled = true;
                    });
            }
            
            if (hasButtonAnimation)
            {
                rotatingElement.DORotate(Vector3.forward * 90f, duration);
            }
        }

        private void OnEnable()
        {
            foreach (var t in toggleButtons)
            {
                t.onClick.AddListener(OnToggled);
            }
        }

        private void OnDisable()
        {
            foreach (var t in toggleButtons)
            {
                t.onClick.RemoveListener(OnToggled);
            }
        }

        private void Start()
        {
            DisableObjects();
        }
    }
}
