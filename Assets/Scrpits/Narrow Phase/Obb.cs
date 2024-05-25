using System;
using Shape2D.Vector;
//using UnityEngine; 

namespace Shape2D
{
    public class OBB : Shape2D
    {
        public Vector2 Center; // ���ĵ�
        public Vector2 Size; // �ߴ� {����}
        public Vector2 YDirection; // Y�������򣬱���Ϊ��λ����

        public Vector2 XDirection; // X�������򣬱���Ϊ��λ����
        public Vector2[] Points; // �ĸ��������

        public OBB(Vector2 center, Vector2 size, Vector2 yDirection)
        {
            Center = center;
            Size = size;
            YDirection = yDirection;

            XDirection = new Vector2(yDirection.y, -yDirection.x);
            Points = new Vector2[4];
            var x = Size.x / 2 * XDirection;
            var y = Size.y / 2 * YDirection;
            Points[0] = Center - x - y; // ����
            Points[1] = Center - x + y; // ����
            Points[2] = Center + x + y; // ����
            Points[3] = Center + x - y;  // ����
        }

        public override bool IsIntersect(OBB other)
        {
            // SAT�����ᶨ�ɣ�����OBB��4����Ѱ�ҷ�����
            if (!IsOverlapOnAxis(other, XDirection))
            {
                return false;
            }
            if (!IsOverlapOnAxis(other, YDirection))
            {
                return false;
            }
            if (!IsOverlapOnAxis(other, other.XDirection))
            {
                return false;
            }
            if (!IsOverlapOnAxis(other, other.XDirection))
            {
                return false;
            }
            return true;
        }

        public override bool IsIntersect(Circle other)
        {
            // 1. Բ����ת��ΪOBB����ϵ���������
            var center2Center = other.Center - Center;
            var projectionX = Vector2.Dot(center2Center, XDirection);
            var projectionY = Vector2.Dot(center2Center, YDirection);
            var newCircleCenter = new Vector2(projectionX, projectionY);
            // 2.Բ�ԳƵ�OBB����ĵ�һ����
            newCircleCenter.x = MathF.Abs(newCircleCenter.x);
            newCircleCenter.y = MathF.Abs(newCircleCenter.y);
            // 3.Ѱ��OBB��Բ�ĵľ����������, ԭ��� https://www.zhihu.com/question/24251545/answer/27184960
            var nearest = newCircleCenter - new Vector2(Size.x / 2, Size.y / 2);
            if (nearest.x < 0)
            {
                nearest.x = 0;
            }
            if (nearest.y < 0)
            {
                nearest.y = 0;
            }
            // 4.�ȽϾ���
            return nearest.sqrMagnitude <= other.Radius * other.Radius;
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
            float minProjection = 0, maxProjection = 0;
            for (int i = 0; i < 4; i++)
            {
                var projectionA = Vector2.Dot(Points[i], axis);
                if (i == 0)
                {
                    minProjection = projectionA;
                    maxProjection = projectionA;
                }
                else
                {
                    minProjection = MathF.Min(minProjection, projectionA);
                    maxProjection = MathF.Max(maxProjection, projectionA);
                }
            }
            return new Tuple<float, float>(minProjection, maxProjection);
        }

        public override bool IsPointIn(Vector2 point)
        {
            // ��תOBB�ֲ��ռ�
            var p = point - Center;
            var x = Vector2.Dot(p, XDirection);
            var y = Vector2.Dot(p, YDirection);
            return (x >= -Size.x / 2 && x <= Size.x / 2 && y >= -Size.y / 2 && y <= Size.y / 2);
        }
    }
}