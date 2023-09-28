using System;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Physics;

/// <summary>
/// 圆形
/// </summary>
public class Circle : Shape {

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
        points = new Vector2[1] { new Vector2(point.X, point.Y) };
        this.radius = radius;
    }

    // 实现抽象类Shape

    public override bool IsCollision(Line line)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsCollision(Circle circle)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsCollision(Polygon polygon)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsPointInside(Vector2 point)
    {
        throw new System.NotImplementedException();
    }

    public override void Rotate(float angle)
    {
        // 圆形绕着它的圆心旋转不会出现任何变化
    }

    /// <summary>
    /// 获取图形面积
    /// </summary>
    public float GetArea() {
        return MathF.PI * radius * radius;
    }
}