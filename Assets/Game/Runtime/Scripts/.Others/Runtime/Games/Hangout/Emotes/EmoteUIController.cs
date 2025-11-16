using Hangout;
using Quantum;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Hangout
{
    public class EmoteUIController : MonoBehaviour
    {
        [SerializeField] private EmoteUIButton emoteUIButtonPrefab;
        [SerializeField] private GameObject emoteSelection;
        [SerializeField] private GameObject emotePanel;
        [SerializeField] private GameObject emoteCloserButton;

#if !ADDRESSABLES_ENABLED
        [SerializeField] private AnimationConfig animationConfig;
#else
        [SerializeField] private AssetReferenceT<AnimationConfig> animationConfigRef;
        private AnimationConfig animationConfig;
#endif

        private EmoteCommand emoteCommand = new EmoteCommand();

        public void OpenEmoteSelectionPanel(bool toggle)
        {
            // TODO: [jm] Change to animation
            emoteSelection.SetActive(toggle);
            emoteCloserButton.SetActive(toggle);
        }

        private void SendEmoteCommand(AssetGuid animationGuid)
        {
            emoteCommand.animationGuid = animationGuid.Value;
            QuantumRunner.Default.Game.SendCommand(emoteCommand);
        }

        private async void Initialize()
        {
#if ADDRESSABLES_ENABLED
            animationConfig = await animationConfigRef.LoadAssetAsync<AnimationConfig>().Task;
#endif

            var animLen = animationConfig.CharacterAnimations.Count;

            for (var i = 0; i < animLen; i++)
            {
                var animationData = animationConfig.CharacterAnimations[i];
                if (animationData is EmotesAnimationAsset emoteAnimAsset)
                {
                    var emoteButton = Instantiate(emoteUIButtonPrefab, emotePanel.transform);
                    emoteButton.Initialize(emoteAnimAsset.Settings.Guid, animationData);
                    emoteButton.OnEmoteSelected = SendEmoteCommand;
                }
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
#if ADDRESSABLES_ENABLED
            animationConfigRef.ReleaseAsset();
#endif
        }
    }
}
