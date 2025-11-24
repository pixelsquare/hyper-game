using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

namespace Santelmo.Rinsurv.Editor
{
    [CreateAssetMenu(fileName = "SpriteGeneratorSources", menuName = "Santelmo/ArtTools/SpriteGenerator/SpriteGeneratorSources")]
    public class SpriteGeneratorSourcesScriptableObject : ScriptableObject
    {
        [SerializeField] private GameObject cameraPrefab;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private CameraAnglesScriptableObject cameraAnglesScriptableObject;

        public GameObject GetCameraPrefab()
        {
            return cameraPrefab;
        }
        public RenderTexture GetRenderTexture()
        {
            return renderTexture;
        }
        public CameraAnglesScriptableObject GetCameraAnglesScriptableObject()
        {
            return cameraAnglesScriptableObject;
        }
    }
}
