using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class ResetApplicationVariables : MonoBehaviour
    {
        private void Start()
        {
            Unity.VisualScripting.Variables.Application.Set("IsInventoryInitialized", false);
            Unity.VisualScripting.Variables.Application.Set("IsShopInitialized", false);
        }
    }
}
