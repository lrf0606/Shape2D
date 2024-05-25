using System;
using Shape2D.Vector;
//using UnityEngine; 

namespace Shape2D
{
    public class Ring : Shape2D
    {
        public Vector2 Center;
        public float InnerRadius; // ��Ȧ�뾶
        public float OuterRadius; // ��Ȧ�뾶

        public Circle InnterCircle; // ��Բ
        public Circle OuterCircle; // ��Բ

        public Ring(Vector2 center, float innerRadius, float outerRadius)
        {
            Center = center;
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
           
            InnterCircle = new Circle(center, innerRadius);
            OuterCircle = new Circle(center, outerRadius); 
        }

        public override bool IsIntersect(OBB other)
        {
            if (OuterCircle.IsPointIn(other.Center))
            {
                if (InnterCircle.IsPointIn(other.Center))
                {
                    foreach(var point in other.Points)
                    {
                        if (!InnterCircle.IsPointIn(point))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return OuterCircle.IsIntersect(other);
            }
        }

        public override bool IsIntersect(Circle other)
        {
            if (OuterCircle.IsPointIn(other.Center))
            {
                if (InnterCircle.IsPointIn(other.Center))
                {
                    var sqrMagnitude = (other.Center - Center).sqrMagnitude;
                    if (sqrMagnitude >= (InnerRadius - other.Radius) * (InnerRadius - other.Radius))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return OuterCircle.IsIntersect(other);
            }
        }

        public override bool IsIntersect(Sector other)
        {
            if (OuterCircle.IsPointIn(other.Center))
            {
                if (InnterCircle.IsPointIn(other.Center))
                {
                    if (InnterCircle.IsPointIn(other.LeftPoint) && InnterCircle.IsPointIn(other.RightPoint))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                   
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return OuterCircle.IsIntersect(other);
            }
        }

        public override bool IsIntersect(Ring other)
        {
            // ��Բ���ཻ����Բ�����ཻ
            if (!OuterCircle.IsIntersect(other.OuterCircle))
            {
                return false;
            }
            // ��Բ�ཻ������£�ֻʣ��СԲ���ڴ�Բ������Բ�ڲ���һ��������ཻ
            var biggerRing = other;
            var smallerRing = this;
            if (OuterRadius > other.OuterRadius)
            {
                biggerRing = this;
                smallerRing = other;
            }
            var sqrMagnitude = (smallerRing.Center - biggerRing.Center).sqrMagnitude;
            if (sqrMagnitude >= (biggerRing.InnerRadius - smallerRing.OuterRadius) * (biggerRing.InnerRadius - smallerRing.OuterRadius))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override Tuple<float, float> GetAxisProjectionRagne(Vector2 axis)
        {
            var projectionCenter = Vector2.Dot(Center, axis);
            var minProjection = projectionCenter - OuterRadius;
            var maxProjection = projectionCenter + OuterRadius;
            return new Tuple<float, float>(minProjection, maxProjection);
        }

        public override bool IsPointIn(Vector2 point)
        {
            var sqrMagnitude = (point - Center).sqrMagnitude;
            if (sqrMagnitude > OuterRadius * OuterRadius || sqrMagnitude < InnerRadius * InnerRadius)
            {
                return false;
            }
            return true;
        }
    }
}