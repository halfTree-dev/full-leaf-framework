using System;
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
        Vector2? intersectionPoint = GetIntersectionPoint(line);
        if (intersectionPoint == null) {
            // 平行或者重合
            // 仅当重合以及存在重合区间时相交，若存在重合区间，则必然有一方端点位于另一方上
            return IsPointInside(line.points[0]) || IsPointInside(line.points[1])
            || line.IsPointInside(points[0]) || line.IsPointInside(points[1]);
        }
        else {
            // 相交，当交点同时在两条线段上(交集)则有碰撞
            Vector2 crossPoint = (Vector2)intersectionPoint;
            return IsPointInside(crossPoint) && line.IsPointInside(crossPoint);
        }
    }

    public override bool IsCollision(Circle circle)
    {
        // 我不想走向大山，可以让大山向我走来~
        return circle.IsCollision(this);
    }

    public override bool IsCollision(Polygon polygon)
    {
        // 我不想走向大山，可以让大山向我走来~
        return polygon.IsCollision(this);
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
        if (MathF.Abs(beVector.X * bpVector.Y - beVector.Y * bpVector.X) <= 0.001f) {
            // 判断向量的比值，0<=a<=1，即向量模be更大且同向
            if (bpVector.Length() <= beVector.Length() &&
            bpVector.X * beVector.X + bpVector.Y * beVector.Y >= 0f) {
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

    /// <summary>
    /// 返回直线方程的三个参数A,B,C
    /// </summary>
    public float[] GetLineEquation() {
        if (IsZeroVector(this)) {
            throw new System.Exception("不可以对一个零向量求其所在直线的方程" + this);
        }
        // Ax1 + By1 + C = 0, Ax2 + By2 + C = 0
        // A(x2 - x1) + B(y2 - y1) = 0
        float[] result = new float[3];
        result[0] = points[0].Y - points[1].Y; //y1 - y2
        result[1] = points[1].X - points[0].X; //x2 - x1
        result[2] = -(result[0] * points[0].X + result[1] * points[0].Y); //带入方程
        return result;
    }

    /// <summary>
    /// 返回直线交点（平行或者重合时返回空值）
    /// </summary>
    public Vector2? GetIntersectionPoint(Line line) {
        // A1x + B1y + C1 = 0, A2x + B2y + C2 = 0
        // A1A2x + B1A2y + C1A2 = 0, A2A1x + B2A1y + C2A1 = 0
        // 两式相减得到y,x同理
        float[] line1 = GetLineEquation();
        float[] line2 = line.GetLineEquation();
        if (MathF.Abs(line1[0] * line2[1] - line2[0] * line1[1]) <= 0.001f) {
            return null;
        }
        float result_y = (line1[2] * line2[0] - line2[2] * line1[0]) /
        (line1[0] * line2[1] - line2[0] * line1[1]);
        //(c1 * a2 - c2 * a1) / (a1 * b2 - a2 * b1)
        float result_x = (line2[2] * line1[1] - line1[2] * line2[1]) /
        (line1[0] * line2[1] - line2[0] * line1[1]);
        //(c2 * b1 - c1 * b2) / (a1 * b2 - a2 * b1)
        return new Vector2(result_x, result_y);
    }

    /// <summary>
    /// 根据直线参数(A,B,C)生成直线
    /// </summary>
    public static Line SpawnLineFromEquation(float[] lineParams) {
        // Ax + By + C = 0, 代入x=0, 得到直线上一点(0, -C / B)
        // 代入y=0，得到直线上一点(-C / A, 0)
        // 戳啦！不能这样做，直线过原点或与坐标轴平行怎么办？
        // 还是分类讨论吧
        if (lineParams[1] != 0) {
            // B != 0 直线不平行于y轴
            // 带入x=0
            Vector2 point1 = new Vector2(0, -lineParams[2] / lineParams[1]);
            // 根据斜率向量(B, -A)平移
            Vector2 point2 = point1 + new Vector2(lineParams[1], -lineParams[0]);
            return new Line(point1, point2);
        }
        else {
            // B = 0 直线平行于y轴
            // 算出x的值
            Vector2 point1 = new Vector2(-lineParams[2] / lineParams[0], 0);
            Vector2 point2 = point1 + new Vector2(0, 1);
            return new Line(point1, point2);
        }
    }

    /// <summary>
    /// 获取点到直线距离
    /// </summary>
    public float GetDistanceFromPoint(Vector2 point) {
        float[] targetLine = GetLineEquation();
        // d = |Ax + By + C|/Sqrt(A^2 + B^2)
        float distance = MathF.Abs(targetLine[0] * point.X + targetLine[1] * point.Y + targetLine[2])
        / MathF.Sqrt(targetLine[0] * targetLine[0] + targetLine[1] * targetLine[1]);
        return distance;
    }

    /// <summary>
    /// 生成自身的垂线
    /// </summary>
    /// <param name="point">垂线过的点</param>
    public Line SpawnVerticalLine(Vector2 point) {
        float[] directionLine = GetLineEquation();
        float c = directionLine[1] * point.X - directionLine[0] * point.Y;
        Line verticalLine = SpawnLineFromEquation(new float[3] {-directionLine[1], directionLine[0], c});
        return verticalLine;
    }

    /// <summary>
    /// 将自身投射到指定直线上
    /// </summary>
    /// <param name="line">将本条直线作为投射方向</param>
    public Line Project(Line line) {
        float[] directionLine = line.GetLineEquation();
        // 投影目标方向，新直线的A，B应当是本直线的-B与A
        float c1 = directionLine[1] * points[0].X - directionLine[0] * points[0].Y;
        float c2 = directionLine[1] * points[1].X - directionLine[0] * points[1].Y;
        // Ax + By + C = 0，将端点处的坐标带入，得到C = Bx - Ay
        Line verticalLine1 = SpawnLineFromEquation(new float[3] {-directionLine[1], directionLine[0], c1});
        Line verticalLine2 = SpawnLineFromEquation(new float[3] {-directionLine[1], directionLine[0], c2});
        // 生成垂直直线
        Vector2 crossPoint1 = (Vector2)verticalLine1.GetIntersectionPoint(line);
        Vector2 crossPoint2 = (Vector2)verticalLine2.GetIntersectionPoint(line);
        // 算出相交交点
        return new Line(crossPoint1, crossPoint2);
        // 生成投影
    }

    /// <summary>
    /// 将点投射到自身上
    /// </summary>
    /// <param name="line">直线外一点</param>
    public Vector2 ProjectPoint(Vector2 point) {
        float[] directionLine = GetLineEquation();
        // 投影目标方向，新直线的A，B应当是本直线的-B与A
        float c = directionLine[1] * point.X - directionLine[0] * point.Y;
        // Ax + By + C = 0，将端点处的坐标带入，得到C = Bx - Ay
        Line verticalLine = SpawnLineFromEquation(new float[3] {-directionLine[1], directionLine[0], c});
        // 生成垂直直线
        Vector2 crossPoint = (Vector2)GetIntersectionPoint(verticalLine);
        // 算出相交交点
        return crossPoint;
        // 生成投影
    }

}

