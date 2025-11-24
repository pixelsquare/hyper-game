using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CharacterMotor2D : MonoBehaviour
    {
        public const int MaxColliderResults = 50;
        public const int MaxIterations = 5;
        public const float Padding = 0.01f;
        
        private Collider2D[] _colliderResults = new Collider2D[MaxColliderResults];
        
        [Tooltip("Collidable layers while moving.")]
        [SerializeField] private LayerMask _collisionLayers;
        
        private ContactFilter2D _contactFilter;
        private IMotor2dPhysics _motor2dPhysics;

        private enum CollisionDetectionMethod
        {
            Physics2D,
            QuadtreeAndCustomDetection,
        }

        private CollisionDetectionMethod _collisionDetection = CollisionDetectionMethod.Physics2D;

        /// <summary>
        /// Try to move along a given vector.
        /// </summary>
        /// <param name="moveVector">The vector of movement.</param>
        /// <param name="ignoreMass">If true, will pass through colliders with less mass.</param>
        /// <returns>True if a "static" (collider without a <see cref="IMotor2dPhysics"/>) or heavier collider was hit while trying to move. This can only be true if ignoreMass is false. Otherwise, will be false.</returns>
        public bool Move(Vector2 moveVector, bool ignoreMass = true)
        {
            var thisTransform = transform;
            var startingPosition = (Vector2)thisTransform.position;
            var totalHeavyCollisions = 0;

            var iterations = 0;
            while (iterations < MaxIterations)
            {
                thisTransform.Translate(moveVector);

                var colliderCount = 0;
                var heavyCollisions = 0;
                var adjustmentVector = Vector2.zero;

                switch (_collisionDetection)
                {
                    case CollisionDetectionMethod.Physics2D:
                        // Prevent detecting own collider
                        _motor2dPhysics.CircleCollider2D.enabled = false;
                        colliderCount = Physics2D.OverlapCircle(thisTransform.position,
                            _motor2dPhysics.CircleCollider2D.radius, _contactFilter, _colliderResults);
                        _motor2dPhysics.CircleCollider2D.enabled = true;
                        
                        // Calculate adjustment vector
                        adjustmentVector = AddDistanceVectors(colliderCount, _colliderResults, out heavyCollisions, ignoreMass);
                        break;
                    case CollisionDetectionMethod.QuadtreeAndCustomDetection:
                        QuadTreeInstance.Quadtree.Move(gameObject);
                        var potentials = QuadTreeInstance.Quadtree.Retrieve(gameObject);
                        
                        // TODO: find a way to convert tilemap colliders into normie colliders
                        // foreach (var tilemapCollider in ColliderDirectory.TilemapColliders)
                        // {
                        //     // technically works but on contact the player can't move anymore
                        //     if (tilemapCollider.IsTouching(_motor2dPhysics.CircleCollider2D))
                        //     {
                        //         Debug.LogWarning("is touching");
                        //         collisions.Add(tilemapCollider);
                        //     }    
                        // }

                        var cols = potentials.Select(t => t.GetComponent<Collider2D>()).Where(t => _contactFilter.IsFilteringTrigger(t)).ToList();
                        adjustmentVector = AddDistanceVectors(cols, out heavyCollisions, ignoreMass);
                        break;
                }

                totalHeavyCollisions += heavyCollisions;
                // If space is clear, we can stop...
                // else, apply some adjustments in the next iteration 
                if (adjustmentVector == Vector2.zero)
                {
                    break;
                }
                
                // Prepare adjustment
                moveVector = adjustmentVector.normalized * (adjustmentVector.magnitude + Padding);
                iterations++;
            }

            // Return to start position because we exceeded allowable iterations
            if (iterations >= MaxIterations)
            {
                thisTransform.position = startingPosition;
                if (_collisionDetection == CollisionDetectionMethod.QuadtreeAndCustomDetection)
                {
                    QuadTreeInstance.Quadtree.Move(gameObject);
                }
            }
            
            return totalHeavyCollisions > 0;
        }

        private Vector2 AddDistanceVectors(int colliderCount, IReadOnlyList<Collider2D> colliders, out int heavyCollisionCount, bool ignoreMass = true)
        {
            heavyCollisionCount = 0;
            
            var finalDistanceVector = Vector2.zero;
            for (var i = 0; i < colliderCount; i++)
            {
                var otherCollider = colliders[i];

                if (!ignoreMass)
                {
                    var hasMotor = otherCollider.TryGetComponent<IMotor2dPhysics>(out var characterMotor);
                    if (hasMotor && _motor2dPhysics.Mass >= characterMotor.Mass)
                    {
                        continue;
                    }
                }

                var distance2D = Physics2D.Distance(_motor2dPhysics.CircleCollider2D, otherCollider);

                if (!distance2D.isValid)
                {
                    continue;
                }

                if (!(distance2D.distance < 0f))
                {
                    continue;
                }

                if (!ignoreMass)
                {
                    heavyCollisionCount++;
                }
                
                finalDistanceVector += distance2D.pointB - distance2D.pointA;
            }

            return finalDistanceVector;
        }

        private Vector2 AddDistanceVectors(IEnumerable<Collider2D> colliders, out int heavyCollisionCount, bool ignoreMass = true)
        {
            heavyCollisionCount = 0;
            
            var finalDistanceVector = Vector3.zero;

            foreach (var col in colliders)
            {
                if (!ignoreMass)
                {
                    var hasMotor = col.TryGetComponent<IMotor2dPhysics>(out var characterMotor);
                    if (hasMotor && _motor2dPhysics.Mass >= characterMotor.Mass)
                    {
                        continue;
                    }
                }
                
                if (CollisionDetection.DetectCollision(col, _motor2dPhysics.CircleCollider2D))
                {
                    // TODO: figure out overlap distance
                    finalDistanceVector += (col.transform.position - transform.position);
                }
            }

            return finalDistanceVector;
        }

#region Monobehaviour

        private void Awake()
        {
            _contactFilter = new ContactFilter2D
            {
                layerMask = _collisionLayers,
                useLayerMask = true
            };

            _motor2dPhysics = GetComponent<IMotor2dPhysics>();
            Assert.IsTrue(_motor2dPhysics != null);
        }

#endregion
    }
}
