using UnityEngine;

namespace Santelmo.Rinsurv
{
    public static class CollisionDetection
    {
        // TODO: figure out overlap distance between colliders
        
        public static bool DetectCollision(Collider2D col1, Collider2D col2)
        {
            if (col1 is CircleCollider2D circle1)
            {
                switch (col2)
                {
                    case CircleCollider2D circle2:
                        return CircleCircle(circle1, circle2);
                    case BoxCollider2D box2:
                        return CircleRect(circle1, box2);
                    case PolygonCollider2D poly2:
                    {
                        var p2Vertices = poly2.points;

                        for (var i = 0; i < p2Vertices.Length; i++)
                        {
                            p2Vertices[i] += (Vector2)poly2.transform.position;
                        }

                        var position = circle1.transform.position;
                        return PolyCircle(p2Vertices, position.x, position.y, circle1.radius);
                    }
                }
            }
            else if (col1 is BoxCollider2D box1)
            {
                switch (col2)
                {
                    case CircleCollider2D circle2:
                        return CircleRect(circle2, box1);
                    case BoxCollider2D box2:
                        RectRect(box1, box2);
                        break;
                    case PolygonCollider2D poly2:
                    {
                        var p2Vertices = poly2.points;

                        for (var i = 0; i < p2Vertices.Length; i++)
                        {
                            p2Vertices[i] += (Vector2)poly2.transform.position;
                        }
                    
                        return PolyRect(p2Vertices, box1.transform.position.x, box1.transform.position.y, box1.size.x, box1.size.y);
                    }
                }
            }
            else if (col1 is PolygonCollider2D poly1)
            {
                var p1Vertices = poly1.points;

                for (var i = 0; i < p1Vertices.Length; i++)
                {
                    p1Vertices[i] += (Vector2)poly1.transform.position;
                }
                
                switch (col2)
                {
                    case CircleCollider2D circle2:
                        return PolyCircle(p1Vertices, circle2.transform.position.x, circle2.transform.position.y, circle2.radius);
                    case BoxCollider2D box2:
                        return PolyRect(p1Vertices, 
                            box2.transform.position.x - box2.size.x / 2, box2.transform.position.y - box2.size.y / 2, 
                            box2.size.x, box2.size.y);
                    case PolygonCollider2D poly2:
                        return PolyPoly(poly1, poly2);
                }
            }

            return false;  // If the collider combination isn't recognized
        }


        private static bool CircleCircle(CircleCollider2D circle1, CircleCollider2D circle2)
        {
            return CircleCircle(circle1.transform.position, circle1.radius, circle2.transform.position, circle2.radius);
        }

        private static bool CircleCircle(Vector2 c1, float r1, Vector2 c2, float r2)
        {
            var distanceSquared = (c1 - c2).sqrMagnitude;
            var radiiSumSquared = (r1 + r2) * (r1 + r2);

            if (distanceSquared <= radiiSumSquared)
            {
                return true;
            }
            return false;
        }

        private static bool RectRect(BoxCollider2D rect1, BoxCollider2D rect2)
        {
            var r1 = new Rect(rect1.transform.position.x, rect1.transform.position.y, rect1.size.x, rect1.size.y);
            var r2 = new Rect(rect2.transform.position.x, rect2.transform.position.y, rect2.size.x, rect2.size.y);
            return RectRect(r1, r2);
        }

        private static bool RectRect(Rect r1, Rect r2)
        {
            if (r1.xMax >= r2.xMin &&           // r1 right edge past r2 left
                r1.xMin <= r2.xMax &&           // r1 left edge past r2 right
                r1.yMax >= r2.yMin &&           // r1 top edge past r2 bottom
                r1.yMin <= r2.yMax)             // r1 bottom edge past r2 top
            {
                return true;
            }
            return false;
        }

        private static bool CircleRect(CircleCollider2D circle, BoxCollider2D rect)
        {
            var r = new Rect(rect.transform.position.x, rect.transform.position.y, rect.size.x, rect.size.y);
            return CircleRect(circle.transform.position, circle.radius, r);
        }

        private static bool CircleRect(Vector2 circlePos, float radius, Rect rect)
        {
            var testX = circlePos.x;
            var testY = circlePos.y;

            // Determine which edge is closest
            if (circlePos.x < rect.xMin)
            {
                testX = rect.xMin; // test left edge
            }
            else if (circlePos.x > rect.xMax)
            {
                testX = rect.xMax; // test right edge
            }

            if (circlePos.y < rect.yMin)
            {
                testY = rect.yMin; // test top edge
            }
            else if (circlePos.y > rect.yMax)
            {
                testY = rect.yMax; // test bottom edge
            }

            var distX = circlePos.x - testX;
            var distY = circlePos.y - testY;
            var distanceSquared = (distX * distX) + (distY * distY);

            if (distanceSquared <= radius * radius)
            {
                return true;
            }
            return false;
        }

        private static bool PolyPoly(PolygonCollider2D poly1, PolygonCollider2D poly2)
        {
            // Convert to Vector2 arrays for compatibility with the existing function
            var p1Vertices = poly1.points;
            var p2Vertices = poly2.points;

            // Adjust vertices based on the collider's world position
            for (var i = 0; i < p1Vertices.Length; i++)
            {
                p1Vertices[i] += (Vector2)poly1.transform.position;
            }
            for (var i = 0; i < p2Vertices.Length; i++)
            {
                p2Vertices[i] += (Vector2)poly2.transform.position;
            }

            return PolyPoly(p1Vertices, p2Vertices);
        }

        private static bool PolyPoly(Vector2[] p1, Vector2[] p2)
        {
            var next = 0;
            for (var current = 0; current < p1.Length; current++)
            {
                next = current + 1;
                if (next == p1.Length)
                {
                    next = 0;
                }

                var vc = p1[current];
                var vn = p1[next];

                var collision = PolyLine(p2, vc.x, vc.y, vn.x, vn.y);
                if (collision)
                {
                    return true;
                }

                collision = PolyPoint(p1, p2[0].x, p2[0].y);
                if (collision)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool PolyRect(Vector2[] vertices, float rx, float ry, float rw, float rh)
        {
            var next = 0;
            for (var current = 0; current < vertices.Length; current++)
            {
                next = current + 1;
                if (next == vertices.Length)
                {
                    next = 0;
                }

                var vc = vertices[current];    // c for "current"
                var vn = vertices[next];       // n for "next"

                // Check against all four sides of the rectangle
                var collision = LineRect(vc.x, vc.y, vn.x, vn.y, rx, ry, rw, rh);
                if (collision)
                {
                    return true;
                }

                // Optional: test if the rectangle is INSIDE the polygon
                var inside = PolyPoint(vertices, rx, ry);
                if (inside)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool LineRect(float x1, float y1, float x2, float y2, float rx, float ry, float rw, float rh)
        {
            // Check if the line has hit any of the rectangle's sides
            var left = LineLine(x1, y1, x2, y2, rx, ry, rx, ry + rh);
            var right = LineLine(x1, y1, x2, y2, rx + rw, ry, rx + rw, ry + rh);
            var top = LineLine(x1, y1, x2, y2, rx, ry, rx + rw, ry);
            var bottom = LineLine(x1, y1, x2, y2, rx, ry + rh, rx + rw, ry + rh);

            // If ANY of the above are true, the line has hit the rectangle
            if (left || right || top || bottom)
            {
                return true;
            }
            return false;
        }

        private static bool PolyCircle(Vector2[] vertices, float cx, float cy, float r)
        {
            var next = 0;
            for (var current = 0; current < vertices.Length; current++)
            {
                next = current + 1;
                if (next == vertices.Length)
                    next = 0;

                var vc = vertices[current];
                var vn = vertices[next];

                var collision = LineCircle(vc.x, vc.y, vn.x, vn.y, cx, cy, r);
                if (collision)
                    return true;
            }

            // The commented section checks if the circle's center is inside the polygon.
            var centerInside = PolyPoint(vertices, cx, cy);
            if (centerInside)
            {
                return true;
            }

            return false;
        }

        private static bool LineCircle(float x1, float y1, float x2, float y2, float cx, float cy, float r)
        {
            var inside1 = PointCircle(x1, y1, cx, cy, r);
            var inside2 = PointCircle(x2, y2, cx, cy, r);
            if (inside1 || inside2)
            {
                return true;
            }

            var distX = x1 - x2;
            var distY = y1 - y2;
            var len = Mathf.Sqrt(distX * distX + distY * distY);

            var dot = ((cx - x1) * (x2 - x1) + (cy - y1) * (y2 - y1)) / (len * len);

            var closestX = x1 + dot * (x2 - x1);
            var closestY = y1 + dot * (y2 - y1);

            var onSegment = LinePoint(x1, y1, x2, y2, closestX, closestY);
            if (!onSegment)
                return false;

            distX = closestX - cx;
            distY = closestY - cy;
            var distance = Mathf.Sqrt(distX * distX + distY * distY);

            if (distance <= r)
            {
                return true;
            }
            
            return false;
        }

        private static bool LinePoint(float x1, float y1, float x2, float y2, float px, float py)
        {
            var d1 = Vector2.Distance(new Vector2(px, py), new Vector2(x1, y1));
            var d2 = Vector2.Distance(new Vector2(px, py), new Vector2(x2, y2));
            var lineLen = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));

            var buffer = 0.1f;

            if (d1 + d2 >= lineLen - buffer && d1 + d2 <= lineLen + buffer)
            {
                return true;
            }
            return false;
        }

        private static bool PointCircle(float px, float py, float cx, float cy, float r)
        {
            var distX = px - cx;
            var distY = py - cy;
            var distance = Mathf.Sqrt(distX * distX + distY * distY);

            if (distance <= r)
            {
                return true;
            }
            return false;
        }


        private static bool PolyLine(Vector2[] vertices, float x1, float y1, float x2, float y2)
        {
            var next = 0;
            for (var current = 0; current < vertices.Length; current++)
            {
                next = current + 1;
                if (next == vertices.Length)
                {
                    next = 0;
                }

                var x3 = vertices[current].x;
                var y3 = vertices[current].y;
                var x4 = vertices[next].x;
                var y4 = vertices[next].y;

                var hit = LineLine(x1, y1, x2, y2, x3, y3, x4, y4);
                if (hit)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool LineLine(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            var uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            var uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            if (uA is >= 0 and <= 1 && uB is >= 0 and <= 1)
            {
                return true;
            }

            return false;
        }

        private static bool PolyPoint(Vector2[] vertices, float px, float py)
        {
            var collision = false;
            var next = 0;
            for (var current = 0; current < vertices.Length; current++)
            {
                next = current + 1;
                if (next == vertices.Length)
                {
                    next = 0;
                }

                var vc = vertices[current];
                var vn = vertices[next];

                if (((vc.y > py && vn.y < py) || (vc.y < py && vn.y > py)) &&
                    (px < (vn.x - vc.x) * (py - vc.y) / (vn.y - vc.y) + vc.x))
                {
                    collision = !collision;
                }
            }

            return collision;
        }
    }
}
