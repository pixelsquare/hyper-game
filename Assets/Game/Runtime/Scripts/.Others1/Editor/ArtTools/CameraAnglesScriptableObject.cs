using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Santelmo.Rinsurv.Editor
{
    [CreateAssetMenu(fileName = "CameraAngles", menuName = "Santelmo/ArtTools/SpriteGenerator/CameraAngles")]
    public class CameraAnglesScriptableObject : ScriptableObject
    {
        public List<CameraAngle> cameraAngles;
    }
}
