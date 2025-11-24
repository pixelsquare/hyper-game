using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Santelmo.Rinsurv.Backend
{
    [CreateAssetMenu(menuName = "Santelmo/Firebase/Google Services Config", fileName = "GoogleServicesConfig")]
    public class GoogleServicesConfig : ScriptableObject
    {
        public string GoogleWebClientId;
    }
}
