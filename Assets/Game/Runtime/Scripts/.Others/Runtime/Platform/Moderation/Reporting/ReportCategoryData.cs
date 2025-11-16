using System;
using UnityEngine;

namespace Kumu.Kulitan.Moderation
{
    [CreateAssetMenu(fileName = "Report Category Data", menuName = "Moderation/Report Category Data")]
    public class ReportCategoryData : ScriptableObject
    {
        public string categoryName;
        public string[] subcategories = Array.Empty<string>();
    }
}
