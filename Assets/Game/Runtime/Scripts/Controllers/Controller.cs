using UnityEngine;

namespace Game
{
    public class Controller<T> : MonoBehaviour where T : View
    {
        [SerializeField] protected T view;
    }
}
