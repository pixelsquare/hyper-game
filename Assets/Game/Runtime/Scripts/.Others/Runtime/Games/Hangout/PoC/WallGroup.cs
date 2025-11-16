using System.Collections.Generic;
using UnityEngine;

namespace Kumu.Kulitan.POC
{
    /// <summary>
    /// Throwaway script for PoC. 
    /// </summary>
    public class WallGroup : MonoBehaviour
    {
        [SerializeField] private GameObject wallParent;
        [SerializeField] private List<GameObject> otherObjectsToDisable;

        public void ToggleVisiblity(bool toggle)
        {
            wallParent.SetActive(toggle);
            foreach (var objects in otherObjectsToDisable)
            {
                objects.SetActive(toggle);
            }
        }
    }
}

