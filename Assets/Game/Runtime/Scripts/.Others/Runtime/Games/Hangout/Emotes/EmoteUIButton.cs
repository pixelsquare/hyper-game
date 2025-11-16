using System;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Tracking;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class EmoteUIButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI displayText;

        private Quantum.AssetGuid animGuid;
        private string emoteId;

        public Action<Quantum.AssetGuid> OnEmoteSelected = null;

        public void OnEmoteButtonSelected()
        {
            OnEmoteSelected?.Invoke(animGuid);
            
            var emoteUseEvent = new EmoteUseEvent
            (
                "",
                1, 
                emoteId
            );
            
            GlobalNotifier.Instance.Trigger(emoteUseEvent);
        }

        public void Initialize(Quantum.AssetGuid animationGuid, AnimationDataAsset animationData)
        {
            animGuid = animationGuid;
            displayText.text = animationData.DisplayName;
            emoteId = animationData.name.CamelToSnakeCase();
        }
    }
}
