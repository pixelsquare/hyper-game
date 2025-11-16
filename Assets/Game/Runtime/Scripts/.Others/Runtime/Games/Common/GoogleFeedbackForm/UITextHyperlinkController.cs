using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UITextHyperlinkController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Color32 clickedHyperLinkTextColor = new (172, 132, 119, 255);
        private TextMeshProUGUI text;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);
            if (linkIndex == -1)
            {
                return;
            }

            var linkInfo = text.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());

            ChangeHyperlinkColor(linkInfo);
        }

        private void ChangeHyperlinkColor(TMP_LinkInfo linkInfo)
        {
            var linkFirstCharIndex = linkInfo.linkTextfirstCharacterIndex;
            var linkCharCount = linkInfo.linkTextLength;
            var linkLastCharIndex = linkFirstCharIndex + linkCharCount;
            
            for (var i = linkFirstCharIndex; i < linkLastCharIndex; i++)
            {
                var charInfo = text.textInfo.characterInfo[i];
                if (char.IsWhiteSpace(charInfo.character))
                {
                    continue;
                }

                var meshIndex = charInfo.materialReferenceIndex;

                var vertexIndex = charInfo.vertexIndex;

                var vertexColors = text.textInfo.meshInfo[meshIndex].colors32;

                vertexColors[vertexIndex + 0] = clickedHyperLinkTextColor;
                vertexColors[vertexIndex + 1] = clickedHyperLinkTextColor;
                vertexColors[vertexIndex + 2] = clickedHyperLinkTextColor;
                vertexColors[vertexIndex + 3] = clickedHyperLinkTextColor;
            }
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }
    }
}

