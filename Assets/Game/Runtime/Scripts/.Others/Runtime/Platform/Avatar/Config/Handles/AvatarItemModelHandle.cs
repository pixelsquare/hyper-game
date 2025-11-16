using Kumu.Extensions;
using System.Collections;
using System.Linq;
using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarItemModelHandle : AvatarItemModelHandleBase
    {
        [SerializeField] private SkinnedMeshRenderer skinnedMesh;
        [SerializeField] private string colorProperty = "_BaseColor";
        [SerializeField] private string[] textureProperties = new string[] { "_BaseMap" };

        private bool HasMaterial => cachedMaterial != null;

        private int colorPropertyId;
        private int[] texturePropertyIds;
        private Color cachedColor;
        private Material cachedMaterial;
        private IMaterialInstanceProvider materialInstanceProvider;
        
        public override void SetMaterial(Material sourceMaterial)
        {
            var mat = materialInstanceProvider.ProvideMaterialInstance(sourceMaterial);
            
            skinnedMesh.sharedMaterial = mat;
            cachedMaterial = mat;
        }

        public override void SetTexture(params Texture[] textures)
        {
            var texLength = textures.Length;
            var propsLength = texturePropertyIds.Length;

            if (texLength != propsLength)
            {
                throw new System.ArgumentException($"AvatarItemModelHandle.SetTexture parameter mismatch for {name.WrapColor(Color.magenta)}: {$"{texLength}".WrapColor(Color.red)} supplied / {$"{propsLength}".WrapColor(Color.green)} expected.");
            }

            for (var i = 0; i < texLength && i < propsLength; i++)
            {
                var tex = textures[i];
                var propId = texturePropertyIds[i];
                skinnedMesh.sharedMaterial.SetTexture(propId, tex);
            }
        }
        
        public override void SetColor(Color color)
        {
            if (HasMaterial)
            {
                cachedColor = color;
                cachedMaterial.SetColor(colorPropertyId, color);
            }
            else
            {
                CoroutineRunner.RunCoroutine(AwaitSetColor(color));
            }
        }

        public override void SetMesh(Mesh mesh)
        {
            skinnedMesh.sharedMesh = mesh;
        }

        public override bool HasMesh()
        {
            return skinnedMesh && skinnedMesh.sharedMesh;
        }

        public override Bounds MeshBounds()
        {
            return !HasMesh() ? default : skinnedMesh.sharedMesh.bounds;
        }

        public override void SetMeshEnabled(bool isEnabled)
        {
            skinnedMesh.enabled = isEnabled;
        }

        public bool TryGetColor(out Color color)
        {
            color = Color.white;

            if (!HasMaterial)
            {
                return false;
            }

            color = cachedColor;
            return true;
        }

        private IEnumerator AwaitSetColor(Color color)
        {
            yield return new WaitUntil(() => HasMaterial);
            SetColor(color);
        }

        private void Reset()
        {
            skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        }

        private void Awake()
        {
            materialInstanceProvider = new MaterialInstanceProvider();
            var texProps = from texProp in textureProperties
                select Shader.PropertyToID(texProp);
            texturePropertyIds = texProps.ToArray();
        }

        private void Start()
        {
            colorPropertyId = Shader.PropertyToID(colorProperty);
        }

        private void OnDestroy()
        {
            materialInstanceProvider.ReleaseMaterialInstance();
        }
    }
}
