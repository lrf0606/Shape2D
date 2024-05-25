using System;
using Shape2D.Vector;
//using UnityEngine; 

namespace Shape2D
{
    public abstract class Shape2D
    {
        // �ཻ��⣨����Ҳ�����ཻ��
        public bool IsIntersect(Shape2D other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("Shape2D.IsIntersect argument other is null");
            }
            if (other is OBB obb)
            {
                return IsIntersect(obb);
            }
            else if(other is Circle circle)
            {
                return IsIntersect(circle);
            }
            else if(other is Sector sector)
            {
                return IsIntersect(sector);
            }
            else if(other is Ring ring)
            {
                return IsIntersect(ring);
            }
            else
            {
                throw new ArgumentException($"Shape2D.IsIntersect argument other:{other} is not supported");
            }
        }

        // ��Ŀ����״��ĳ�����ϵ�ͶӰ��Χ�Ƿ����ص�������ȷ��������
        public bool IsOverlapOnAxis(Shape2D other, Vector2 axis)
        {
            var rangeA = GetAxisProjectionRagne(axis);
            var minA = rangeA.Item1;
            var maxA = rangeA.Item2;
            var rangeB = other.GetAxisProjectionRagne(axis);
            var minB = rangeB.Item1;
            var maxB = rangeB.Item2;
            if (minA < minB)
            {
                return (minB - maxA) <= 0; // ������� = minB - maxA
            }
            else
            {
                return (minA - maxB) <= 0; // ������� = minA - maxB
            }
        }

        public abstract bool IsIntersect(OBB other);
        public abstract bool IsIntersect(Circle other);
        public abstract bool IsIntersect(Sector other);
        public abstract bool IsIntersect(Ring other);

        // ��ȡ��ǰ��״��Ŀ�����ϵ�ͶӰ��Χ  @param:axisΪ��������  @return:��СͶӰ�����ͶӰ
        public abstract Tuple<float, float> GetAxisProjectionRagne(Vector2 axis);

        // ���Ƿ�����״��
        public abstract bool IsPointIn(Vector2 point);
    }
}