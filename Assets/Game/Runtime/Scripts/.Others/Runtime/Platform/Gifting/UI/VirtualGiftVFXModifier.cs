using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftVFXModifier : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI multiplierText;

        public void SetMultiplierText(int newCount)
        {
            if (newCount < 2)
            {
                multiplierText.gameObject.SetActive(false);
            }
            else
            {
                multiplierText.gameObject.SetActive(true);
                multiplierText.text = "x" + newCount;
            }
        }
    }
}
