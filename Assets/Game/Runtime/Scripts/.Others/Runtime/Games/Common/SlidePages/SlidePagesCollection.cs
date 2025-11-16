using Kumu.Extensions;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [CreateAssetMenu(menuName = "Kumu/Tutorial/Slide Pages")]
    public class SlidePagesCollection : ScriptableObject
    {
        [SerializeField] private string id;
        
        [SerializeField] private SlidePageData[] data;

        public string Id => id;

        public int Count => data?.Length ?? 0;

        public SlidePageData GetDataAt(int i)
        {
            if (data == null)
            {
                "Pages is null!".LogError();
                return null;
            }
            
            if (i < 0 || i > data.Length)
            {
                $"Index {i} outside of bounds!".LogError();
                return null;
            }

            return data[i];
        }
    }
}
