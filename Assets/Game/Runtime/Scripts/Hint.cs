using R3;
using UnityEngine;

namespace Game
{
    public class Hint : MonoBehaviour
    {
        [SerializeField] private ImageProp image;
        [SerializeField] private TextProp text;

        public ReactiveProperty<Color> Color => image.Color;
        public ReactiveProperty<string> Text => text.Text;
    }
}
