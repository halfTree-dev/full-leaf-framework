/*
Shape.cs
描述一个“形状”类型
以下是它们应当具有的基本方法
*/

using System;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Physics;

public interface IShape {
    public void Translate(Vector2 shiftPos);
    public void Rotate(float angle);
    public void Rotate(Vector2 center, float angle);
    public float GetArea();
    public Rectangle GetSmallestAABBRectangle();
}

/// <summary>
/// 形状管理器
/// 管理形状之间的种内/间关系
/// </summary>
public class ShapeManager {

    /// <summary>
    /// 围绕指定点旋转
    /// </summary>
    /// <param name="target">旋转目标点</param>
    /// <param name="center">旋转中心</param>
    /// <param name="angle">旋转角度</param>
    /// <returns></returns>
    internal static Vector2 RotateAroundPoint(Vector2 target, Vector2 center, float angle) {
        Vector2 swiftPos = target - center;
        Vector2 anglePos = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        // 辐角为angle的单位向量，在复平面上
        Vector2 changedPos = new Vector2() {
            X = swiftPos.X * anglePos.X - anglePos.Y * swiftPos.Y,
            Y = anglePos.X * swiftPos.Y + anglePos.Y * swiftPos.X
        };
        // 在复平面内二向量相乘相当于旋转
        return center + changedPos;
    }

    /// <summary>
    /// 获取线段的垂线
    /// </summary>
    /// <param name="target">目标线段</param>
    /// <param name="point">垂线过的点</param>
    internal static Line ReturnVerticleLine(Line target, Vector2 point) {
        if (!target.IsZeroVector()) {
            Vector2 swiftPos = target.point2 - target.point1;
            Vector2 normalVector = new Vector2(-swiftPos.Y, swiftPos.X);
            normalVector.Normalize();
            // 获取线段的法向量
            return new Line(point, point + normalVector);
        }
        return new Line(new Vector2(point.X, point.Y), new Vector2(target.point1.X, target.point1.Y));
    }

    /// <summary>
    /// 获取多边形关于指定投影轴的投影长度取值范围
    /// </summary>
    /// <param name="polygon">被投影多边形</param>
    /// <param name="castOn">投影轴</param>
    private static float[] GetProjectLengthRange(Polygon polygon, Line castOn) {
        float[] result = new float[2] { float.MaxValue, float.MinValue };
        // 接下来，对多边形每个顶点求对投影轴的投影数量
        // 投影数量最大最小值 与 多边形在投影轴上投影线段两顶点的投影数量 相等
        // 以此为依据来判断投影线段的位置，是否重合
        foreach (Vector2 point in polygon.Points) {
            float projectValue = point.X * (castOn.point1.X - castOn.point2.X) +
            point.Y * (castOn.point1.Y - castOn.point2.Y);
            if (projectValue <= result[0]) { result[0] = projectValue; }
            if (projectValue >= result[1]) { result[1] = projectValue; }
        }
        return result;
    }

    /// <summary>
    /// 获取线段关于指定投影轴的投影长度取值范围
    /// </summary>
    /// <param name="line">被投影线段</param>
    /// <param name="castOn">投影轴</param>
    private static float[] GetProjectLengthRange(Line line, Line castOn) {
        float[] result = new float[2] { float.MaxValue, float.MinValue };
        foreach (Vector2 point in new Vector2[2] {line.point1, line.point2}) {
            float projectValue = point.X * (castOn.point1.X - castOn.point2.X) +
            point.Y * (castOn.point1.Y - castOn.point2.Y);
            if (projectValue <= result[0]) { result[0] = projectValue; }
            if (projectValue >= result[1]) { result[1] = projectValue; }
        }
        return result;
    }

    /// <summary>
    /// 获取圆关于指定投影轴的投影长度取值范围
    /// </summary>
    /// <param name="circle">被投影圆</param>
    /// <param name="castOn">投影轴</param>
    private static float[] GetProjectLengthRange(Circle circle, Line castOn) {
        float projectValue = circle.center.X * (castOn.point1.X - castOn.point2.X) +
        circle.center.Y * (castOn.point1.Y - castOn.point2.Y);
        float[] result = new float[2] { projectValue - circle.radius, projectValue + circle.radius };
        return result;
    }

    /// <summary>
    /// 检测两个投影线段是否相交
    /// </summary>
    private static bool IsTwoProjectionIntersect(float[] cast1, float[] cast2) {
		float minAll = MathF.Min(cast1[0], cast2[0]);
        float maxAll = MathF.Max(cast1[1], cast2[1]);
		return cast1[1] - cast1[0] + (cast2[1] - cast2[0]) >= maxAll - minAll;
        // 如果两条线段中间有空隙，总线段长度大于二线段之和，否则，二线段有相交部分
    }

    /// <summary>
    /// 获取点到多边形最近点的投影轴
    /// </summary>
    private static Line GetNearestAxisFromPolygon(Vector2 point, Polygon polygon) {
        float nowDistance = float.MaxValue;
        Line axis = new Line(Vector2.Zero, Vector2.Zero);
        // 对每一条边做判断
        Line[] edges = polygon.GetAllEdges();
        foreach (Line edge in edges) {
            float distance = GetDistanceFromPointToLine(point, edge);
            if (distance < nowDistance) {
                nowDistance = distance;
                axis = ReturnVerticleLine(edge, Vector2.Zero);
                // 发现距离边最近，投影轴为边的法向量
            }
        }
        // 对每一个点做判断
        foreach (Vector2 polygonPoint in polygon.Points) {
            float distance = Vector2.Distance(polygonPoint, point);
            if (distance < nowDistance) {
                nowDistance = distance;
                var pointer = point - polygonPoint;
                pointer.Normalize();
                axis = new Line(Vector2.Zero, pointer);
                // 发现距离点最近，那么点之间的向量为投影轴
            }
        }
        return axis;
    }

    /// <summary>
    /// 获取点到直线距离
    /// </summary>
    private static float GetDistanceFromPointToLine(Vector2 point, Line line) {
        // 获取三角形面积
        Polygon triangle = new Polygon(new Vector2[3] {point, line.point1, line.point2});
        float area = triangle.GetArea();
        // 获取底面长度
        float edgeLength = Vector2.Distance(line.point1, line.point2);
        return 2 * area / edgeLength;
    }

    /// <summary>
    /// 判断两个多边形是否发生碰撞
    /// </summary>
    public static bool IsCollision(Polygon polygon1, Polygon polygon2) {
        var allVerticles = new Line[polygon1.VerticleLines.Length + polygon2.VerticleLines.Length];
        Array.Copy(polygon1.VerticleLines, 0, allVerticles, 0, polygon1.VerticleLines.Length);
        Array.Copy(polygon2.VerticleLines, 0, allVerticles, polygon1.VerticleLines.Length, polygon2.VerticleLines.Length);
        // 合并两个投影轴数组
        foreach (Line verticle in allVerticles) {
            float[] cast1 = GetProjectLengthRange(polygon1, verticle);
            float[] cast2 = GetProjectLengthRange(polygon2, verticle);
            if (!IsTwoProjectionIntersect(cast1, cast2)) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 判断两个圆是否发生碰撞
    /// </summary>
    public static bool IsCollision(Circle circle1, Circle circle2) {
        return Vector2.Distance(circle1.center, circle2.center) <= circle1.radius + circle2.radius;
    }

    /// <summary>
    /// 判断圆与多边形之间是否发生碰撞
    /// </summary>
    public static bool IsCollision(Polygon polygon, Circle circle) {
        var allVerticles = new Line[polygon.VerticleLines.Length + 1];
        Array.Copy(polygon.VerticleLines, 0, allVerticles, 0, polygon.VerticleLines.Length);
        allVerticles[allVerticles.Length - 1] = GetNearestAxisFromPolygon(circle.center, polygon);
        foreach (Line verticle in allVerticles) {
            float[] cast1 = GetProjectLengthRange(polygon, verticle);
            float[] cast2 = GetProjectLengthRange(circle, verticle);
            if (!IsTwoProjectionIntersect(cast1, cast2)) {
                return false;
            }
        }
        return true;
    }

}
