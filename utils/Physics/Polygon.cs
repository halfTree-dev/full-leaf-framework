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
public class Polygon : IShape {

    private Vector2[] points;
    /// <summary>
    /// 投影轴列表
    /// </summary>
    private Line[] verticleLines;
    public Vector2[] Points { get => points; }
    public Line[] VerticleLines { get => verticleLines; }

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
        FillVerticleLines();
    }

    /// <summary>
    /// 创建多边形
    /// </summary>
    /// <param name="points">顶点</param>
    public Polygon(Vector2[] points) {
        if (points.Length <= 1) { throw new Exception("多边形的顶点数应当大于1"); }
        this.points = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++) {
            this.points[i] = new Vector2(points[i].X, points[i].Y);
        }
        FillVerticleLines();
    }

    /// <summary>
    /// 创建多边形的所有投影轴
    /// </summary>
    private void FillVerticleLines() {
        verticleLines = new Line[Points.Length];
        for (int i = 0; i < Points.Length - 1; i++) {
            var edge = new Line(Points[i], Points[i + 1]);
            verticleLines[i] = ShapeManager.ReturnVerticleLine(edge, Vector2.Zero);
        }
        verticleLines[Points.Length - 1] = ShapeManager.ReturnVerticleLine
        (new Line(Points[Points.Length - 1], Points[0]), Vector2.Zero);
        // 所有的投影轴都过原点，且与某一条边垂直
    }

    /// <summary>
    /// 获取多边形的所有边
    /// </summary>
    /// <returns></returns>
    public Line[] GetAllEdges() {
        Line[] edges = new Line[Points.Length];
        for (int i = 0; i < Points.Length - 1; i++) {
            var edge = new Line(Points[i], Points[i + 1]);
            edges[i] = edge;
        }
        edges[Points.Length - 1] = new Line(Points[Points.Length - 1], Points[0]);
        return edges;
    }

    /// <summary>
    /// 围绕重心旋转
    /// </summary>
    public void Rotate(float angle)
    {
        Vector2 rotateCenter = GetCenterOfGravity();
        for (int i = 0; i < Points.Length; i++) {
            Points[i] = ShapeManager.RotateAroundPoint(Points[i], rotateCenter, angle);
        }
        FillVerticleLines();
    }

    /// <summary>
    /// 围绕旋转中心旋转
    /// </summary>
    public void Rotate(Vector2 center, float angle)
    {
        for (int i = 0; i < Points.Length; i++) {
            Points[i] = ShapeManager.RotateAroundPoint(Points[i], center, angle);
        }
        FillVerticleLines();
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
    public Vector2 GetCenterOfGravity() {
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
            Vector2 COGThis = GetTriangleCenterOfGravity(triangle);
            COG += COGThis * areaThis;
            // 重心坐标加权面积
        }
        COG /= areaSum;
        return COG;
    }

    /// <summary>
    /// 返回三角形重心
    /// </summary>
    private Vector2 GetTriangleCenterOfGravity(Polygon triangle) {
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
        if (Points.Length < 3) {
            throw new Exception("不是平面图形，不可分割：" + Points);
        }
        if (Points.Length == 3) {
            result = new Polygon[1] { this };
        }
        result = new Polygon[Points.Length - 2];
        // 轮换遍历顶点
        for (int i = 1; i < Points.Length - 1; i++) {
            result[i - 1] = new Polygon(new Vector2[3] { Points[0],
            Points[i], Points[i + 1]});
            // 开始点0和点1,2; 2,3; 3,4...每三点一组
        }
        return result;
    }

    /// <summary>
    /// 获取该多边形最小的AABB外接长方形，
    /// AABB长方形为两对对边分别平行于两个坐标轴的长方形，
    /// 需要注意的是，由于Xna的矩形只能以int作顶点坐标，所以这不是严格的外接，
    /// 不过这个功能本来就用作粗略运算，所以不要在意这么多。
    /// </summary>
    public Rectangle GetSmallestAABBRectangle() {
        float minX = Points[0].X;
        float maxX = Points[0].X;
        float minY = Points[0].Y;
        float maxY = Points[0].Y;
        for (int i = 0; i < Points.Length; i++) {
            if (Points[i].X > maxX) { maxX = Points[i].X; }
            else if (Points[i].X < minX) { minX = Points[i].X; }
            if (Points[i].Y > maxY) { maxY = Points[i].Y; }
            else if (Points[i].Y < minY) { minY = Points[i].Y; }
        }
        return new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
    }

    /// <summary>
    /// 平移图形
    /// </summary>
    /// <param name="shiftPos">偏移量</param>
    public void Translate(Vector2 shiftPos)
    {
        for (int i = 0; i < points.Length; i++) {
            points[i] += shiftPos;
        }
    }
}