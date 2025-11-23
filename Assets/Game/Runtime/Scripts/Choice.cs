using R3;
using UnityEngine;

namespace Game
{
    public class Choice : MonoBehaviour
    {
        [SerializeField] private ButtonProp button;

        public ReactiveProperty<string> Text => button.Text;
        public ReactiveProperty<Color> Color => button.Color;
        public ReactiveProperty<bool> IsInteractable => button.IsInteractable;
        public Subject<Unit> OnClickEvent => button.OnClickEvent;
    }
}
