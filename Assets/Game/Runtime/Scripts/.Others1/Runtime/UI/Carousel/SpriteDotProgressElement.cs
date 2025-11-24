using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class SpriteDotProgressElement : MonoBehaviour
    {
        [SerializeField] private RinawaImage _image;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _deactiveSprite;
        [SerializeField] private RinawaButton _button;

        public RinawaButton Button => _button;

        public void SetActive(bool isActive)
        {
            _image.sprite = isActive ? _activeSprite : _deactiveSprite;
        }
    }
}
