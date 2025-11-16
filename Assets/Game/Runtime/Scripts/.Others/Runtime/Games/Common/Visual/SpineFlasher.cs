using System.Collections;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    /// Handles and lerps color flashes with the appropriate shader
    /// </summary>
    public class SpineFlasher : MonoBehaviour
    {
        [SerializeField] private Renderer spineRenderer;
        [SerializeField] private string flashPropertyName;
        [SerializeField] private float flashDuration;
        [SerializeField] private float flashDelay;
        
        private MaterialPropertyBlock propertyBlock;
        private int flashAlphaId;
        private float targetValue;
        private bool isFlashing;

        public void Flash()
        {
            StartCoroutine(StartFlash());
        }

        private IEnumerator StartFlash()
        {
            yield return new WaitForSeconds(flashDelay);
            var lerpTime = 0f;
            while (lerpTime < flashDuration)
            {
                lerpTime += Time.deltaTime;
                var percentage = lerpTime / flashDuration;
                SetFlashAmount(1f - percentage);
                yield return null;

            }
            SetFlashAmount(0);
        }

        private void SetFlashAmount(float value)
        {
            propertyBlock.SetFloat(flashAlphaId, value);
            spineRenderer.SetPropertyBlock(propertyBlock);
        }

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            spineRenderer.GetPropertyBlock(propertyBlock);
            flashAlphaId = Shader.PropertyToID(flashPropertyName);
        }
    }
}
