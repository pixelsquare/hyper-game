using System;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [Serializable]
    public class SlidePageData
    {
        [SerializeField] private string displayText;
        [SerializeField] private Sprite displayImage;
            
        public string DisplayText => displayText;
        public Sprite DisplayImage => displayImage;
    }
}
