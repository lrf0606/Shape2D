using System;
using Shape2D.Vector;
//using UnityEngine; 

namespace Shape2D
{
    public class Sector : Shape2D
    {
        public Vector2 Center;
        public Vector2 Direction; // 中心点到圆弧中心的方向，必须为单位向量
        public float Radius; // 半径
        public float Angle; // 扇形角度

        public float SinTheta; // 半角对应的sin值
        public float CosTheta; // 半角对应的cos值
        public Vector2 LeftDirection; // Direction逆时针旋转HalfAngle
        public Vector2 RightDirection; // Direction瞬时针旋转HalfAngle
        public Vector2 LeftPoint; // LeftDirection方向的点
        public Vector2 RightPoint; // RightDirection方向的点


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
            // 1.扇形所在圆和OBB不相交，必然不相交
            var circle = new Circle(Center, Radius);
            if (!other.IsIntersect(circle))
            {
                return false;
            }
            // 2.OBB两个边和扇形两个直边寻找分离轴
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
            // 1.扇形所在圆和目标圆是否相交
            var center2Center = other.Center - Center;
            if (center2Center.sqrMagnitude > Radius * Radius)
            {
                return false;
            }
            // 2.目标圆心在扇形角度内
            var projectionX = Vector2.Dot(center2Center, Direction); // 圆转为扇形局部坐标
            var projectionY = Math.Abs(Vector2.Dot(center2Center, new Vector2(-Direction.y, Direction.x))); // 对称到一二象限
            if (projectionX >= center2Center.magnitude * CosTheta)
            {
                return true;
            }
            //3. 扇形左线段是否和圆相交，原理见 https://zhuanlan.zhihu.com/p/23903445
            var circleLocalCenter = new Vector2(projectionX, projectionY);
            var lineLocalEnd = LeftPoint - Center;
            var t = Vector2.Dot(circleLocalCenter, lineLocalEnd) / lineLocalEnd.sqrMagnitude;
            var sqrMagnitude = (circleLocalCenter - Math.Clamp(t, 0, 1) * lineLocalEnd).sqrMagnitude;
            return sqrMagnitude <= other.Radius * other.Radius;
        }
        
        public override bool IsIntersect(Sector other)
        {
            // 1.扇形对应的两个圆不相交，则两扇形不相交
            var center2Center = other.Center - Center;
            var length = Radius + other.Radius;
            if (center2Center.sqrMagnitude > length * length)
            {
                return false;
            }
            // 2.两个扇形的四条直边寻找分离轴
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
            // 3.两个扇形分别检验三个点（圆心和弧的两个端点）是否在另一个扇形内
            if (IsPointIn(other.Center) || IsPointIn(other.LeftPoint) || IsPointIn(other.RightPoint) 
                || other.IsPointIn(Center) || other.IsPointIn(LeftPoint) || other.IsPointIn(RightPoint))
            {
                return true;
            }
            // 4.两中心点连线的垂线能否分离（处理两扇形所在圆相切、两扇形仅有两个弧线相交）
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
            // 若分离轴与扇形方向的夹角大于HalfAngle，只需考虑三点的投影，否则还需考虑Radius   
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
            // 点在圆形内且点在扇形的角度内
            var center2Center = point - Center;
            var sqrMagnitude = center2Center.sqrMagnitude;
            if (sqrMagnitude > Radius * Radius)
            {
                return false;
            }
            //var length = MathF.Sqrt(sqrMagnitude);
            //return Vector2.Dot(p, Direction) >= length * MathF.Cos(HalfAngle);
            // 对Sqrt优化，Angle限定小于180°，所以Cos(HalfAngle)必定>=0
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