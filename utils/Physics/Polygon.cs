/*
凸集：
    凸集是对于集合内的每一对点，连接该对点的直线段上的每个点也在该集合内。
超平面：
    超平面是指n维线性空间中维度为n-1的子空间。 它可以把线性空间分割成不相交的两部分。
    对于三维空间来说，超平面就是一个二维平面，对于二维平面，超平面就是一条直线。
凸集分离定理：
    两个不相交的凸集总可用超平面分离。
    证明太难，我放弃看懂它了，这回就让我们用一用推论吧。

分离轴定理：
    对于两个凸多边形，若存在一条直线将两者分开，则这两个多边形不相交。
    其依据为凸集分离定理。

任意凸多边形的碰撞检测：
    对于两个凸多边形，检测是否存在一条这样的直线t，若存在，则不相交，反之。
    先讨论对于一个具体方向的检测方法：
        作垂直于t的直线l，将两个多边形的所有顶点分别投影到l上，
        分别做出两个多边形的最远的两个投影点的连接线，若两条连接线存在相交部分，
        则不存在该方向上的直线可以满足上述要求。
    遍历所有的方向是不现实的，所以我们要采取更加明智的方法。

    这个推论我也没有证明，但是我还是要用它：
    将两个多边形所有的边所在的直线作为直线t进行依次测试，
    如果都不行，则再不存在其它的直线满足条件。
    其实就是，只需要依次在每条边的垂直线做投影即可。

    于是，检测碰撞的过程就是：
    依次确定多边形的各个投影轴，
    将多边形投射到某条投影轴上，
    检测两段投影是否发生重叠。

性能上的优化：
    可以先使用AABB（轴对齐包围矩形）的方式，创建包含该多边形的两边分别平行于x,y轴的最小矩形，
    先判定这两个大矩形是否有可能相交，再使用上述的精确判定方式。
*/

/*
扯远了，让我们回到Polygon Class，
Polygon仅限储存凸多边形，包含多个点，
这些点在创建之初就会被自动排序，然后可以正确表示一个多边形~
*/

using System;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Physics;

/// <summary>
/// 多边形
/// </summary>
public class Polygon : Shape {


    /// <summary>
    /// 创建一个矩形
    /// </summary>
    /// <param name="rectangle">矩形</param>
    public Polygon(Rectangle rectangle) {
        points = new Vector2[4] {
            new Vector2(rectangle.X, rectangle.Y),
            new Vector2(rectangle.X + rectangle.Width, rectangle.Y),
            new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height),
            new Vector2(rectangle.X, rectangle.Y + rectangle.Height)
        };
    }

    /// <summary>
    /// 创建多边形
    /// </summary>
    /// <param name="points">顶点</param>
    public Polygon(Vector2[] points) {
        this.points = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++) {
            this.points[i] = new Vector2(points[i].X, points[i].Y);
        }
    }

    // 实现抽象类Shape

    public override bool IsCollision(Line line)
    {
        bool result = true;
        Line verticalLine; Line projectLine; Line targetProjectLine;
        // 如果能找到一个分隔轴使得投影点不在投影线段上即可断定点在多边形外部
        for (int i = 0; i < points.Length - 1; i++) {
            verticalLine = new Line(points[i], points[i + 1]).SpawnVerticalLine(Vector2.Zero);
            // 对每一条边创建一个投影轴
            projectLine = GetWidestProjectLine(verticalLine, this);
            // 对指定直线投影
            targetProjectLine = line.Project(verticalLine);
            if (!targetProjectLine.IsCollision(projectLine)) {
                result = false; // 在多边形外部
            }
            if (i == points.Length - 2) {
                // 首尾相接的线段也算
                verticalLine = new Line(points[i + 1], points[0]).SpawnVerticalLine(Vector2.Zero);
                projectLine = GetWidestProjectLine(verticalLine, this);
                targetProjectLine = line.Project(verticalLine);
                if (!targetProjectLine.IsCollision(projectLine)) {
                    result = false; // 在多边形外部
                }
            }
        }
        // 除了对于多边形本身的判断，还有将直线作为分割线的判断
        verticalLine = line.SpawnVerticalLine(Vector2.Zero);
        projectLine = GetWidestProjectLine(verticalLine, this);
        targetProjectLine = line.Project(verticalLine);
        if (!targetProjectLine.IsCollision(projectLine)) {
            result = false; // 在多边形外部
        }
        return result;
    }

    public override bool IsCollision(Circle circle)
    {
        // 大山拔腿就跑
        return circle.IsCollision(this);
    }

    public override bool IsCollision(Polygon polygon)
    {
        bool result = true;
        Line verticalLine; Line projectLine; Line targetProjectLine;
        // 如果能找到一个分隔轴使得投影点不在投影线段上即可断定点在多边形外部
        for (int i = 0; i < points.Length - 1; i++) {
            verticalLine = new Line(points[i], points[i + 1]).SpawnVerticalLine(Vector2.Zero);
            // 对每一条边创建一个投影轴
            projectLine = GetWidestProjectLine(verticalLine, this);
            targetProjectLine = GetWidestProjectLine(verticalLine, polygon);
            // 对两个多边形的直线分别投影
            if (!targetProjectLine.IsCollision(projectLine)) {
                result = false; // 有所分离
            }
            if (i == points.Length - 2) {
                // 首尾相接的线段也算
                verticalLine = new Line(points[i + 1], points[0]).SpawnVerticalLine(Vector2.Zero);
                projectLine = GetWidestProjectLine(verticalLine, this);
                targetProjectLine = GetWidestProjectLine(verticalLine, polygon);
                if (!targetProjectLine.IsCollision(projectLine)) {
                    result = false; // 在多边形外部
                }
            }
        }
        // 对另外一个多边形再来一次
        for (int i = 0; i < polygon.points.Length - 1; i++) {
            verticalLine = new Line(polygon.points[i], polygon.points[i + 1]).SpawnVerticalLine(Vector2.Zero);
            // 对每一条边创建一个投影轴
            projectLine = GetWidestProjectLine(verticalLine, this);
            targetProjectLine = GetWidestProjectLine(verticalLine, polygon);
            // 对两个多边形的直线分别投影
            if (!targetProjectLine.IsCollision(projectLine)) {
                result = false; // 有所分离
            }
            if (i == polygon.points.Length - 2) {
                // 首尾相接的线段也算
                verticalLine = new Line(polygon.points[i + 1], polygon.points[0]).SpawnVerticalLine(Vector2.Zero);
                projectLine = GetWidestProjectLine(verticalLine, this);
                targetProjectLine = GetWidestProjectLine(verticalLine, polygon);
                if (!targetProjectLine.IsCollision(projectLine)) {
                    result = false; // 在多边形外部
                }
            }
        }
        return result;
    }

    public override bool IsPointInside(Vector2 point)
    {
        bool result = true;
        // 如果能找到一个分隔轴使得投影点不在投影线段上即可断定点在多边形外部
        for (int i = 0; i < points.Length - 1; i++) {
            Line verticalLine = new Line(points[i], points[i + 1]).SpawnVerticalLine(Vector2.Zero);
            // 对每一条边创建一个投影轴
            Line projectLine = GetWidestProjectLine(verticalLine, this);
            // 对指定直线投影
            if (!projectLine.IsPointInside(point)) {
                result = false; // 在多边形外部
            }
        }
        return result;
    }

    /// <summary>
    /// 获取多边形的最宽的投影线
    /// </summary>
    /// <param name="verticalLine">投影面</param>
    /// <param name="polygon">多边形</param>
    public static Line GetWidestProjectLine(Line verticalLine, Polygon polygon) {
        // 对每一条边创建一个投影轴
        Vector2 pointL = verticalLine.ProjectPoint(polygon.points[0]);
        Vector2 pointR = verticalLine.ProjectPoint(polygon.points[1]);
        // 由两个point决定最广投影线
        for (int j = 2; j < polygon.points.Length; j++) {
            Vector2 currentPoint = verticalLine.ProjectPoint(polygon.points[j]);
            // 如果这个点带来的投影范围更广，取代之
            if (Vector2.Distance(currentPoint, pointL) > Vector2.Distance(currentPoint, pointR) &&
            Vector2.Distance(currentPoint, pointL) > Vector2.Distance(pointL, pointR)) {
                pointR = currentPoint;
            }
            if (Vector2.Distance(currentPoint, pointR) >= Vector2.Distance(currentPoint, pointL) &&
            Vector2.Distance(currentPoint, pointR) > Vector2.Distance(pointL, pointR)) {
                pointL = currentPoint;
            }
        }
        // 决定最广投影线
        Line projectLine = new Line(pointL, pointR);
        return projectLine;
    }

    public override void Rotate(float angle)
    {
        Vector2 rotateCenter = GetCOG();
        // 获取重心作为旋转中心
        Rotate(angle, rotateCenter);
    }

    /// <summary>
    /// 获取图形面积
    /// </summary>
    public float GetArea() {
        Polygon[] triangles = GetSplitTriangles();
        float result = 0;
        foreach (Polygon triangle in triangles) {
            result += GetTriangleArea(triangle);
            // 将多边形分解为若干三角形，全部求和
        }
        return result;
    }

    /// <summary>
    /// 获取多边形重心
    /// </summary>
    public Vector2 GetCOG() {
        Polygon[] triangles = GetSplitTriangles();
        Vector2 COG = new Vector2(0, 0);
        float areaSum = 0;
        // 三角形重心公式(依据物理原理容易推导)
        // Gx = Sigma(Gx[i] * S[i]) / Sigma(s[i])
        // Gy = Sigma(Gy[i] * S[i]) / Sigma(s[i])
        foreach (Polygon triangle in triangles) {
            float areaThis = GetTriangleArea(triangle);
            areaSum += areaThis;
            // 将多边形分解为若干三角形
            Vector2 COGThis = GetTriangleCOG(triangle);
            COG += COGThis * areaThis;
            // 重心坐标加权面积
        }
        COG /= areaSum;
        return COG;
    }

    /// <summary>
    /// 返回三角形重心
    /// </summary>
    private Vector2 GetTriangleCOG(Polygon triangle) {
        Vector2[] trianglePoints = triangle.Points;
        if (trianglePoints.Length != 3) {
            throw new Exception("三角形必须只具有三个顶点：" + trianglePoints);
        }
        return (trianglePoints[0] + trianglePoints[1] + trianglePoints[2]) / 3;
    }

    /// <summary>
    /// 返回三角形面积
    /// </summary>
    private float GetTriangleArea(Polygon triangle) {
        Vector2[] trianglePoints = triangle.Points;
        if (trianglePoints.Length != 3) {
            throw new Exception("三角形必须只具有三个顶点：" + trianglePoints);
        }
        // 海伦公式
        float p = (trianglePoints[0].Length() +
        trianglePoints[1].Length() + trianglePoints[2].Length()) / 2;
        return MathF.Sqrt(p * (p - trianglePoints[0].Length()) *
        (p - trianglePoints[1].Length()) * (p - trianglePoints[2].Length()));
    }

    /// <summary>
    /// 将这个多边形按照顶点分割为若干个三角形，返回它们
    /// </summary>
    private Polygon[] GetSplitTriangles() {
        Polygon[] result;
        if (points.Length < 3) {
            throw new Exception("不是平面图形，不可分割：" + points);
        }
        if (points.Length == 3) {
            result = new Polygon[1] { this };
        }
        result = new Polygon[points.Length - 2];
        // 轮换遍历顶点
        for (int i = 1; i < points.Length - 1; i++) {
            result[i - 1] = new Polygon(new Vector2[3] { points[0],
            points[i], points[i + 1]});
            // 开始点0和点1,2; 2,3; 3,4...每三点一组
        }
        return result;
    }

}