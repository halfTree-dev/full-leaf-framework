/*
Shape.cs
描述一个“形状”类型
以下是它们应当具有的基本方法
*/

using System;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Physics;

/// <summary>
/// 形状
/// </summary>
public abstract class Shape {

    /// <summary>
    /// 顶点（或者端点，圆心）
    /// </summary>
    protected Vector2[] points;
    public Vector2[] Points { get => points; }

    /// <summary>
    /// 依据旋转角旋转一个向量
    /// </summary>
    private Vector2 RotateVector(Vector2 vector, float angle) {
        if (Vector2.Distance(vector, Vector2.Zero) <= 0f) {
            return new Vector2(0, 0);
        }
        // 现在陈述一下我旋转向量的思路：先将总的向量拆分成两个坐标轴上的向量，
        // 然后分别对它们按照目标角度旋转，最后将它们合成
        float newX = vector.X * MathF.Cos(angle) + vector.Y * MathF.Sin(angle);
        float newY = vector.X * MathF.Sin(angle) + vector.Y * MathF.Cos(angle);
        return new Vector2(newX, newY);
    }

    /// <summary>
    /// 对图形平移
    /// </summary>
    /// <param name="direction">方向</param>
    public void Translate(Vector2 direction)
    {
        for (int i = 0; i < points.Length; i++) {
            points[i] += direction;
            // 所有顶点向同一个方向移动
        }
    }
    /// <summary>
    /// 对图形绕其重心旋转
    /// </summary>
    public abstract void Rotate(float angle);
    /// <summary>
    /// 对图形绕指定旋转中心旋转
    /// </summary>
    public void Rotate(float angle, Vector2 centerPoint) {
        // 按照中心点旋转
        for (int i = 0; i < points.Length; i++) {
            // 对所有顶点按照规则旋转
            Vector2 shiftVector = points[i] - centerPoint;
            Vector2 rotatedVector = RotateVector(shiftVector, angle);
            points[i] = centerPoint + rotatedVector;
        }
    }
    /// <summary>
    /// 点是否在图形内（上）
    /// </summary>
    public abstract bool IsPointInside(Vector2 point);
    /// <summary>
    /// 判断是否碰撞
    /// </summary>
    public abstract bool IsCollision(Line line);
    /// <summary>
    /// 判断是否碰撞
    /// </summary>
    public abstract bool IsCollision(Circle circle);
    /// <summary>
    /// 判断是否碰撞
    /// </summary>
    public abstract bool IsCollision(Polygon polygon);
    /// <summary>
    /// 获取该多直线最小的AABB外接长方形
    /// </summary>
    public abstract Rectangle GetSmallestAABBRectangle();

}