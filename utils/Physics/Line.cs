using System;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Physics;

/// <summary>
/// 线段
/// </summary>
public class Line : IShape {

    /// <summary>
    /// 端点1
    /// </summary>
    public Vector2 point1;
    /// <summary>
    /// 端点2
    /// </summary>
    public Vector2 point2;

    /// <summary>
    /// 创建一条线段
    /// </summary>
    /// <param name="point_1">起点</param>
    /// <param name="point_2">终点</param>
    public Line(Vector2 point_1, Vector2 point_2) {
        point1 = point_1;
        point2 = point_2;
    }

    /// <summary>
    /// 平移
    /// </summary>
    /// <param name="swiftPos">偏移坐标</param>
    public void Translate(Vector2 swiftPos) {
        point1 += swiftPos;
        point2 += swiftPos;
    }

    /// <summary>
    /// 围绕自身重心旋转
    /// </summary>
    public void Rotate(float angle) {
        Vector2 center = (point1 + point2) / 2;
        point1 = ShapeManager.RotateAroundPoint(point1, center, angle);
        point2 = ShapeManager.RotateAroundPoint(point1, center, angle);
    }

    /// <summary>
    /// 围绕指定旋转中心旋转
    /// </summary>
    public void Rotate(Vector2 center, float angle) {
        point1 = ShapeManager.RotateAroundPoint(point1, center, angle);
        point2 = ShapeManager.RotateAroundPoint(point1, center, angle);
    }

    /// <summary>
    /// 判断是否是零向量
    /// </summary>
    public bool IsZeroVector() {
        return Vector2.Distance(point1, point2) <= float.Epsilon;
    }

    /// <summary>
    /// 获取该直线最小的AABB外接长方形，
    /// AABB长方形为两对对边分别平行于两个坐标轴的长方形，
    /// 需要注意的是，由于Xna的矩形只能以int作顶点坐标，所以这不是严格的外接，
    /// 不过这个功能本来就用作粗略运算，所以不要在意这么多。
    /// </summary>
    public Rectangle GetSmallestAABBRectangle() {
        var points = new Vector2[2] { point1, point2 };
        float minX = points[0].X;
        float maxX = points[0].X;
        float minY = points[0].Y;
        float maxY = points[0].Y;
        for (int i = 0; i < points.Length; i++) {
            if (points[i].X > maxX) { maxX = points[i].X; }
            else if (points[i].X < minX) { minX = points[i].X; }
            if (points[i].Y > maxY) { maxY = points[i].Y; }
            else if (points[i].Y < minY) { minY = points[i].Y; }
        }
        return new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
    }

    /// <summary>
    /// 获取面积...你是认真的？你和线段谈面积？该方法恒返回0
    /// </summary>
    public float GetArea() {
        return 0;
    }
}

