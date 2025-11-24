using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Santelmo.Rinsurv
{
    /// <summary>
    /// Handles references to colliders in the scene in gameplay
    /// </summary>
    public class ColliderDirectory : MonoBehaviour
    {
        public static List<TilemapCollider2D> TilemapColliders { get; private set; }

        private void Awake()
        {
            TilemapColliders = new List<TilemapCollider2D>();
        }

        private void Start()
        {
            FindAllTilemapColliders();
        }

        private static void FindAllTilemapColliders()
        {
            TilemapColliders = FindObjectsByType<TilemapCollider2D>(FindObjectsSortMode.None).ToList();
        }
    }
}
