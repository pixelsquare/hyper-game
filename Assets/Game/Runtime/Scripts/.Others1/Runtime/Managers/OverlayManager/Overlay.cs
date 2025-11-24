using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class Overlay : MonoBehaviour
    {
        [SerializeField] private RinawaText _rinawaText;

        public void SetMessage(string message)
        {
            if (_rinawaText == null)
            {
                return;
            }
            
            _rinawaText.text = message;
        }
    }
}
