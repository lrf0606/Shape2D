using System;
using Shape2D.Vector;
//using UnityEngine; 

namespace Shape2D
{
    public class Circle : Shape2D
    {
        public Vector2 Center;
        public float Radius; // °ë¾¶

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public override bool IsIntersect(OBB other)
        {
            return other.IsIntersect(this);
        }

        public override bool IsIntersect(Circle other)
        {
            var length = Radius + other.Radius;
            return (Center - other.Center).sqrMagnitude <= length * length;
        }

        public override bool IsIntersect(Sector other)
        {
            return other.IsIntersect(this);
        }

        public override bool IsIntersect(Ring other)
        {
            return other.IsIntersect(this);
        }

        public override Tuple<float, float> GetAxisProjectionRagne(Vector2 axis)
        {
            var projectionCenter = Vector2.Dot(Center, axis);
            var minProjection = projectionCenter - Radius;
            var maxProjection = projectionCenter + Radius;
            return new Tuple<float, float>(minProjection, maxProjection);
        }

        public override bool IsPointIn(Vector2 point)
        {
            var sqrMagnitude = (point - Center).sqrMagnitude;
            return sqrMagnitude <= Radius * Radius;
        }
    }

}