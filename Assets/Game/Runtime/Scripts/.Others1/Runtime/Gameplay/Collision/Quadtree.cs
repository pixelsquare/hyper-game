using System.Collections.Generic;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class Quadtree {
        private const int MAX_OBJECTS = 10;
        private const int MAX_LEVELS = 5;

        private readonly int level;
        private readonly List<GameObject> objects;
        private Rect bounds;
        private readonly Quadtree[] nodes;

        public Quadtree(int level, Rect bounds) 
        {
            this.level = level;
            this.bounds = bounds;
            objects = new List<GameObject>();
            nodes = new Quadtree[4];
        }

        private static Rect GetAABB(GameObject go) 
        {
            if (go.GetComponent<Collider2D>() != null) 
            {
                var bounds2D = go.GetComponent<Collider2D>().bounds;
                return new Rect(
                    bounds2D.min.x,
                    bounds2D.min.y,
                    bounds2D.size.x,
                    bounds2D.size.y
                );
            }
            
            if (go.GetComponent<Collider>() != null) 
            {
                return new Rect(
                    go.transform.position.x - go.GetComponent<Collider>().bounds.extents.x,
                    go.transform.position.y - go.GetComponent<Collider>().bounds.extents.y,
                    go.GetComponent<Collider>().bounds.size.x,
                    go.GetComponent<Collider>().bounds.size.y
                );
            }

            if (go.GetComponent<MeshFilter>() != null) 
            {
                return new Rect(
                    go.transform.position.x - go.GetComponent<MeshFilter>().mesh.bounds.extents.x,
                    go.transform.position.y - go.GetComponent<MeshFilter>().mesh.bounds.extents.y,
                    go.GetComponent<MeshFilter>().mesh.bounds.size.x,
                    go.GetComponent<MeshFilter>().mesh.bounds.size.y
                );
            }
            return new Rect();
        }

        private void Split() 
        {
            var subWidth = bounds.width * 0.5f;
            var subHeight = bounds.height * 0.5f;
            var x = bounds.x;
            var y = bounds.y;

            nodes[0] = new Quadtree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new Quadtree(level + 1, new Rect(x, y, subWidth, subHeight));
            nodes[2] = new Quadtree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new Quadtree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private int[] GetIndices(GameObject go) 
        {
            var indices = new List<int>();
            var aabb = GetAABB(go);

            var verticalMidpoint = bounds.x + (bounds.width * 0.5f);
            var horizontalMidpoint = bounds.y + (bounds.height * 0.5f);

            var topQuadrant = aabb.yMin > horizontalMidpoint;
            var bottomQuadrant = aabb.yMax < horizontalMidpoint;

            if (aabb.xMin < verticalMidpoint && aabb.xMax < verticalMidpoint) 
            {
                if (topQuadrant) 
                {
                    indices.Add(2);
                }
                if (bottomQuadrant) 
                {
                    indices.Add(1);
                }
            }
            if (aabb.xMin > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    indices.Add(3);
                }
                if (bottomQuadrant)
                {
                    indices.Add(0);
                }
            }

            return indices.ToArray();
        }
        
        public void Move(GameObject go) 
        {
            if (Remove(go))
            {
                Insert(go);
            }
        }

        public void Insert(GameObject go) 
        {
            if (nodes[0] != null)
            {
                var indices = GetIndices(go);

                foreach (var index in indices)
                {
                    nodes[index].Insert(go);
                    return;
                }
            }

            objects.Add(go);

            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS) 
            {
                if (nodes[0] == null)
                {
                    Split();
                }

                var i = 0;
                while (i < objects.Count)
                {
                    var indices = GetIndices(objects[i]);
                    if (indices.Length != 0)
                    {
                        foreach (var index in indices) 
                        {
                            nodes[index].Insert(objects[i]);
                        }
                        objects.RemoveAt(i);
                    } 
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public bool Remove(GameObject go) 
        {
            if (objects.Remove(go))
            {
                return true;
            }

            if (nodes[0] != null)
            {
                var indices = GetIndices(go);
                foreach (var index in indices)
                {
                    if (nodes[index].Remove(go))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<GameObject> Retrieve(GameObject go) 
        {
            var returnObjects = new List<GameObject>();

            var indices = GetIndices(go);
            if (nodes[0] != null)
            {
                foreach (var index in indices)
                {
                    returnObjects.AddRange(nodes[index].Retrieve(go));
                }
            }

            foreach (GameObject obj in objects)
            {
                if (obj != go)
                {
                    returnObjects.Add(obj);
                }
            }

            return returnObjects;
        }
    }
}
