using System;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(fileName = "AvatarShapeConfig", menuName ="Config/KumuKulitan/Avatars/AvatarShapeConfig")]
    public class AvatarShape : ScriptableObject
    {
        [SerializeField] private Data[] config;
        [SerializeField] private Mesh mesh;

        public Data[] Config => config;
        public Mesh Mesh => mesh;

        [Serializable]
        public class Data
        {
            [SerializeField] private int blendIndex;
            [SerializeField] private float blendWeight;

            public int BlendIndex => blendIndex;
            public float BlendWeight => blendWeight;
        }
    }
}