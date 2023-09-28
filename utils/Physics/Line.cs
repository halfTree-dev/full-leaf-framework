using Microsoft.Xna.Framework;

namespace full_leaf_framework.Physics;

/// <summary>
/// 线段
/// </summary>
public class Line : Shape {

    /// <summary>
    /// 创建一条线段
    /// </summary>
    /// <param name="point_1">起点</param>
    /// <param name="point_2">终点</param>
    public Line(Vector2 point_1, Vector2 point_2) {
        points = new Vector2[2] {
            new Vector2(point_1.X, point_1.Y),
            new Vector2(point_2.X, point_2.Y)
        };
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
        if (IsZeroVector(this)) {
            // 零向量则判定点的重合
            return Vector2.Distance(point, points[0]) <= 0.001f;
        }
        // 从线段一点指向另一点的向量
        Vector2 beVector = points[1] - points[0];
        // 从同一点指向目标点的向量
        Vector2 bpVector = point - points[0];
        // 判断是否共线(x1y2=x2y1)(包容些许误差)
        if (beVector.X * bpVector.Y - beVector.Y * bpVector.X <= 0.01f) {
            // 判断向量的比值，0<=a<=1，即向量模be更大且同向
            if (bpVector.Length() <= beVector.Length() &&
            bpVector.X * beVector.X >= 0f) {
                return true;
            }
        }
        return false;
    }

    public override void Rotate(float angle)
    {
        Vector2 rotateCenter = (points[0] + points[1]) / 2;
        Rotate(angle, rotateCenter);
    }

    /// <summary>
    /// 判断是否是零向量
    /// </summary>
    private bool IsZeroVector(Line line) {
        return Vector2.Distance(line.points[1], line.points[0]) <= 0.001f;
    }
}