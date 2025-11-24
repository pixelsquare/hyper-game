using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class QuadTreeInstance : MonoBehaviour
    {
        public static Quadtree Quadtree { get; private set; }

        private void Awake()
        {
            // TODO: establish the correct playing rect for the world
            Quadtree = new Quadtree(0, new Rect(Vector2.zero, Vector2.one * 100f));
        }
    }
}