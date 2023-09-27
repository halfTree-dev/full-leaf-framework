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
    将两个多边形所有的边所在的直线作为直线l进行依次测试，
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