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
        // 先检查圆心到直线距离
        float distance = line.GetDistanceFromPoint(points[0]);
        if (distance < radius) {
            Vector2 projectPoint = line.ProjectPoint(points[0]);
            // 圆心对直线垂点
            float halfCrossLine = MathF.Sqrt(radius * radius - distance * distance);
            // 相交弦长的一半
            float[] lineEquation = line.GetLineEquation();
            Vector2 directionVector = new Vector2(lineEquation[1], -lineEquation[0]);
            directionVector.Normalize();
            Line crossLine = new Line(projectPoint + directionVector * halfCrossLine,
            projectPoint - directionVector * halfCrossLine);
            // 求出相交线段
            // 是否相交？
            return crossLine.IsCollision(line);
        }
        return false;
    }

    public override bool IsCollision(Circle circle)
    {
        // 圆心距小于半径和
        return Vector2.Distance(circle.Points[0], points[0]) <= radius + circle.radius;
    }

    public override bool IsCollision(Polygon polygon)
    {
        // 只需要检查分离轴：圆心距离多边形上距离最短的一点即可
        // 这个点不在顶点上就在线段上~
        bool result = true;
        // 检查所有的边
        for (int i = 0; i < polygon.Points.Length - 1; i++) {
            Line verticalLine = new Line(polygon.Points[i], polygon.Points[i + 1]).SpawnVerticalLine(Vector2.Zero);
            // 对每一条边创建一个投影轴
            Line projectLine = Polygon.GetWidestProjectLine(verticalLine, polygon);
            Line targetProjectLine = Project(verticalLine);
            // 对两个多边形的直线分别投影
            if (!targetProjectLine.IsCollision(projectLine)) {
                result = false; // 有所分离
            }
            if (i == polygon.Points.Length - 2) {
                // 首尾相接的线段也算
                verticalLine = new Line(polygon.Points[i + 1], polygon.Points[0]).SpawnVerticalLine(Vector2.Zero);
                projectLine = Polygon.GetWidestProjectLine(verticalLine, polygon);
                targetProjectLine = Project(verticalLine);
                if (!targetProjectLine.IsCollision(projectLine)) {
                    result = false; // 在多边形外部
                }
            }
        }
        // 检查所有的顶点
        for (int i = 0; i < polygon.Points.Length; i++) {
            Vector2 currentPoint = polygon.Points[i];
            Line verticalLine = new Line(currentPoint, points[0]).SpawnVerticalLine(Vector2.Zero);
            Line projectLine = Polygon.GetWidestProjectLine(verticalLine, polygon);
            Line targetProjectLine = Project(verticalLine);
            if (!targetProjectLine.IsCollision(projectLine)) {
                result = false; // 有所分离
            }
        }
        return result;
    }

    public override bool IsPointInside(Vector2 point)
    {
        // 点到圆心距离在半径内
        return Vector2.Distance(point, points[0]) <= radius;
    }

    public override void Rotate(float angle)
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
    /// 将自身投射到指定直线上
    /// </summary>
    /// <param name="line">将本条直线作为投射方向</param>
    public Line Project(Line line) {
        Vector2 projectPoint = line.ProjectPoint(points[0]);
        // 圆心对直线垂点
        float halfCrossLine = radius;
        // 投影线段一半长（从哪边投影一半都是半径）
        float[] lineEquation = line.GetLineEquation();
        Vector2 directionVector = new Vector2(lineEquation[1], -lineEquation[0]);
        directionVector.Normalize();
        Line crossLine = new Line(projectPoint + directionVector * halfCrossLine,
        projectPoint - directionVector * halfCrossLine);
        // 求出相交线段
        return crossLine;
    }

}