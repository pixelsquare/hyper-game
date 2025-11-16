using Hangout;
using Quantum;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Kumu.Kulitan.Hangout
{
    [RequireComponent(typeof(Button))]
    public class InteractiveControllerButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;

        public AssetGuid ObjectGuid { get; private set; }

        private readonly PlayInteractiveCommand playInteractiveCommand = new();

        public void Initialize(AssetGuid assetGuid, Sprite icon, UnityAction onButtonPressed)
        {
            ObjectGuid = assetGuid;
            buttonImage.overrideSprite = icon;
            gameObject.SetActive(true);
            button.onClick.AddListener(onButtonPressed);
        }

        public void Deinitialize()
        {
            ObjectGuid = AssetGuid.None;
            buttonImage.overrideSprite = null;
            gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }

        public void Reset()
        {
            ObjectGuid = AssetGuid.None;
        }

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }
    }
}
