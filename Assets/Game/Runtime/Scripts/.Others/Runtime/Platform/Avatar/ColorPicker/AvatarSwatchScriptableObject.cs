using System.Collections.Generic;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Scriptable Objects/AvatarColorSwatch")]
    public class AvatarSwatchScriptableObject : ScriptableObject
    {
        [SerializeField] private string swatchName = "swatch";
        [SerializeField] private List<Color> colors = new List<Color>();

        public string SwatchName => swatchName;
        public List<Color> Colors => colors;
    }
}
