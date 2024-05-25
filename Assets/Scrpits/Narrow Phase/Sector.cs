using System;
using Shape2D.Vector;
//using UnityEngine; 

namespace Shape2D
{
    public class Sector : Shape2D
    {
        public Vector2 Center;
        public Vector2 Direction; // ���ĵ㵽Բ�����ĵķ��򣬱���Ϊ��λ����
        public float Radius; // �뾶
        public float Angle; // ���νǶ�

        public float SinTheta; // ��Ƕ�Ӧ��sinֵ
        public float CosTheta; // ��Ƕ�Ӧ��cosֵ
        public Vector2 LeftDirection; // Direction��ʱ����תHalfAngle
        public Vector2 RightDirection; // Direction˲ʱ����תHalfAngle
        public Vector2 LeftPoint; // LeftDirection����ĵ�
        public Vector2 RightPoint; // RightDirection����ĵ�


        public Sector(Vector2 center, Vector2 direction, float radius, float angle)
        {
            Center = center;
            Radius = radius;
            Direction = direction;
            Angle = angle;

            SinTheta = MathF.Sin(angle / 2);
            CosTheta = MathF.Cos(angle / 2);
            LeftDirection = new Vector2(CosTheta * Direction.x - SinTheta * Direction.y, SinTheta * Direction.x + CosTheta * Direction.y);
            RightDirection = new Vector2(CosTheta * Direction.x + SinTheta * Direction.y, -SinTheta * Direction.x + CosTheta * Direction.y);
            LeftPoint = Center + Radius * LeftDirection;
            RightPoint = Center + Radius * RightDirection;
        }

        public override bool IsIntersect(OBB other)
        {
            // 1.��������Բ��OBB���ཻ����Ȼ���ཻ
            var circle = new Circle(Center, Radius);
            if (!other.IsIntersect(circle))
            {
                return false;
            }
            // 2.OBB�����ߺ���������ֱ��Ѱ�ҷ�����
            if (!IsOverlapOnAxis(other, other.XDirection))
            {
                return false;
            }
            if (!IsOverlapOnAxis(other, other.YDirection))
            {
                return false;
            }
            if (!IsOverlapOnAxis(other, new Vector2(-LeftDirection.y, LeftDirection.x)))
            {
                return false;
            }
            if (!IsOverlapOnAxis(other, new Vector2(-RightDirection.y, RightDirection.x)))
            {
                return false;
            }
            return true;
        }

        public override bool IsIntersect(Circle other)
        {
            // 1.��������Բ��Ŀ��Բ�Ƿ��ཻ
            var center2Center = other.Center - Center;
            if (center2Center.sqrMagnitude > Radius * Radius)
            {
                return false;
            }
            // 2.Ŀ��Բ�������νǶ���
            var projectionX = Vector2.Dot(center2Center, Direction); // ԲתΪ���ξֲ�����
            var projectionY = Math.Abs(Vector2.Dot(center2Center, new Vector2(-Direction.y, Direction.x))); // �ԳƵ�һ������
            if (projectionX >= center2Center.magnitude * CosTheta)
            {
                return true;
            }
            //3. �������߶��Ƿ��Բ�ཻ��ԭ��� https://zhuanlan.zhihu.com/p/23903445
            var circleLocalCenter = new Vector2(projectionX, projectionY);
            var lineLocalEnd = LeftPoint - Center;
            var t = Vector2.Dot(circleLocalCenter, lineLocalEnd) / lineLocalEnd.sqrMagnitude;
            var sqrMagnitude = (circleLocalCenter - Math.Clamp(t, 0, 1) * lineLocalEnd).sqrMagnitude;
            return sqrMagnitude <= other.Radius * other.Radius;
        }
        
        public override bool IsIntersect(Sector other)
        {
            // 1.���ζ�Ӧ������Բ���ཻ���������β��ཻ
            var center2Center = other.Center - Center;
            var length = Radius + other.Radius;
            if (center2Center.sqrMagnitude > length * length)
            {
                return false;
            }
            // 2.�������ε�����ֱ��Ѱ�ҷ�����
            if (!IsOverlapOnAxis(other, new Vector2(-LeftDirection.y, LeftDirection.x)))
            {
                return false;
            }
            if (!IsOverlapOnAxis(other, new Vector2(-RightDirection.y, RightDirection.x)))
            {
                return false;
            }
            if (!other.IsOverlapOnAxis(this, new Vector2(-other.LeftDirection.y, other.LeftDirection.x)))
            {
                return false;
            }
            if (!other.IsOverlapOnAxis(this, new Vector2(-other.RightDirection.y, other.RightDirection.x)))
            {
                return false;
            }
            // 3.�������ηֱ���������㣨Բ�ĺͻ��������˵㣩�Ƿ�����һ��������
            if (IsPointIn(other.Center) || IsPointIn(other.LeftPoint) || IsPointIn(other.RightPoint) 
                || other.IsPointIn(Center) || other.IsPointIn(LeftPoint) || other.IsPointIn(RightPoint))
            {
                return true;
            }
            // 4.�����ĵ����ߵĴ����ܷ���루��������������Բ���С������ν������������ཻ��
            if (!IsOverlapOnAxis(other, new Vector2(-center2Center.y, center2Center.x).normalized))
            {
                return false;
            }
            return true;
        }

        public override bool IsIntersect(Ring other)
        {
            return other.IsIntersect(this);
        }

        public override Tuple<float, float> GetAxisProjectionRagne(Vector2 axis)
        {
            float minProjection, maxProjection;
            var projectionCeter = Vector2.Dot(Center, axis);
            var projectionLeft = Vector2.Dot(LeftPoint, axis);
            var projectionRight = Vector2.Dot(RightPoint, axis);
            // �������������η���ļнǴ���HalfAngle��ֻ�迼�������ͶӰ�������迼��Radius   
            var projectionAxis = Vector2.Dot(Radius * axis, Direction);
            if (projectionAxis * projectionAxis <= (Radius * axis).sqrMagnitude * CosTheta * CosTheta)
            {
                minProjection = MathF.Min(projectionCeter, MathF.Min(projectionLeft, projectionRight));
                maxProjection = MathF.Max(projectionCeter, MathF.Max(projectionLeft, projectionRight));
            }
            else
            {
                if (projectionAxis > 0)
                {
                    minProjection = projectionCeter;
                    maxProjection = projectionCeter + Radius;
                }
                else
                {
                    minProjection = projectionCeter - Radius;
                    maxProjection = projectionCeter;
                }
            }
            return new Tuple<float, float>(minProjection, maxProjection);
        }

        public override bool IsPointIn(Vector2 point)
        {
            // ����Բ�����ҵ������εĽǶ���
            var center2Center = point - Center;
            var sqrMagnitude = center2Center.sqrMagnitude;
            if (sqrMagnitude > Radius * Radius)
            {
                return false;
            }
            //var length = MathF.Sqrt(sqrMagnitude);
            //return Vector2.Dot(p, Direction) >= length * MathF.Cos(HalfAngle);
            // ��Sqrt�Ż���Angle�޶�С��180�㣬����Cos(HalfAngle)�ض�>=0
            var projection = Vector2.Dot(center2Center, Direction);
            if (projection >= 0)
            {
                return projection * projection >= sqrMagnitude * CosTheta * CosTheta;
            }
            else
            {
                return false;
            }
        }
    }
}