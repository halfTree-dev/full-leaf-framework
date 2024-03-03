using System;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Physics;

/// <summary>
/// 圆形
/// </summary>
public class Circle {

    /// <summary>
    /// 圆心
    /// </summary>
    public Vector2 center;
    /// <summary>
    /// 半径
    /// </summary>
    public float radius;

    /// <summary>
    /// 创建一个圆形
    /// </summary>
    /// <param name="point">圆心</param>
    /// <param name="radius">半径</param>
    public Circle(Vector2 point, float radius) {
        center = point;
        this.radius = radius;
    }

    public bool IsPointInside(Vector2 point)
    {
        // 点到圆心距离在半径内
        return Vector2.Distance(point, center) <= radius;
    }

    /// <summary>
    /// 围绕重心旋转（你在开玩笑吗，圆绕着它的圆心旋转不会出现任何变化）
    /// </summary>
    public void Rotate(float angle)
    {
        // 圆绕着它的圆心旋转不会出现任何变化
    }

    /// <summary>
    /// 获取图形面积
    /// </summary>
    public float GetArea() {
        return MathF.PI * radius * radius;
    }

    /// <summary>
    /// 获取该圆最小的AABB外接长方形，
    /// AABB长方形为两对对边分别平行于两个坐标轴的长方形，
    /// 需要注意的是，由于Xna的矩形只能以int作顶点坐标，所以这不是严格的外接，
    /// 不过这个功能本来就用作粗略运算，所以不要在意这么多。
    /// </summary>
    public Rectangle GetSmallestAABBRectangle() {
        float minX = center.X - radius;
        float maxX = center.X + radius;
        float minY = center.Y - radius;
        float maxY = center.Y + radius;
        return new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
    }

}