using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Kumu.Kulitan.Common
{
    public class CutoutMask : Image
    {
        public override Material materialForRendering
        {
            get
            {
                var material = new Material(base.materialForRendering);
                material.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
                return material;
            }
        }
    }
}
