/*
瓦片地图也要有物理效果！
所以我写了一个处理瓦片物理效果的类，
主要是碰撞这一块有很多要处理的，
需要注意的是这个物理系统只管瓦片的物理效果，不管建筑物的
*/

using full_leaf_framework.Physics;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Scene;

/// <summary>
/// 瓦片地图物理效果
/// </summary>
public class TilePhysics {

    /// <summary>
    /// 目标瓦片地图
    /// </summary>
    private TileMap tileMap;

    /// <summary>
    /// 碰撞箱集合
    /// </summary>
    private CollisionBox[][] collisionBoxs;

    public CollisionBox[][] CollisionBoxs { get => collisionBoxs; }

    /// <summary>
    /// 创建一个瓦片地图的瓦片物理效果
    /// </summary>
    public TilePhysics(TileMap tileMap, TileMapInfo tileMapInfo) {
        this.tileMap = tileMap;
        LoadCollisionBoxs(tileMapInfo);
    }

    /// <summary>
    /// 准备好碰撞箱
    /// </summary>
    private void LoadCollisionBoxs(TileMapInfo tileMapInfo) {
        collisionBoxs = new CollisionBox[tileMap.MapHeight][];
        for (int i = 0; i < tileMap.MapHeight; i++) {
            collisionBoxs[i] = new CollisionBox[tileMap.MapWidth];
            for (int j = 0; j < tileMap.MapWidth; j++) {
                collisionBoxs[i][j] = null;
                // 用tileMap[i][j]表示第i+1行j+1列的瓦片
                // 首先计算瓦片左上角相对于地图(0, 0)的偏移量
                Vector2 shiftPos = new Vector2(j * tileMap.TileWidth, i * tileMap.TileHeight);
                string currentTile = tileMapInfo.mapInfos[i][j];
                // 然后计算每个瓦片的碰撞区域
                try {
                    var tilePhysicsInfo = tileMapInfo.tilePhysicsDic[currentTile];
                    Polygon collisionArea = tilePhysicsInfo.GetCollisionBox();
                    if (collisionArea != null)
                        collisionArea.Translate(shiftPos);
                    // 创建相应的碰撞区域
                    collisionBoxs[i][j] = new CollisionBox(collisionArea, tilePhysicsInfo.collisionLayer);
                    break;
                }
                catch { continue; }
                // 如果读取失败则忽略
            }
        }
    }

    /// <summary>
    /// 判断是否碰撞
    /// </summary>
    public bool IsCollision(Polygon shape, int collisionLayer)
    {
        bool result = false;
        int[] favoriteArea = GetFavouriteArea(shape);
        for (int i = favoriteArea[0]; i <= favoriteArea[2]; i++) {
            for (int j = favoriteArea[1]; j <= favoriteArea[3]; j++) {
                if (collisionBoxs[j][i].collisionLayer == collisionLayer
                && collisionBoxs[j][i].collisionArea != null)
                    result = ShapeManager.IsCollision(collisionBoxs[j][i].collisionArea, shape) || result;
            }
        }
        return result;
    }

    /// <summary>
    /// 判断是否碰撞
    /// </summary>
    public bool IsCollision(Circle shape, int collisionLayer)
    {
        bool result = false;
        int[] favoriteArea = GetFavouriteArea(shape);
        for (int i = favoriteArea[0]; i <= favoriteArea[2]; i++) {
            for (int j = favoriteArea[1]; j <= favoriteArea[3]; j++) {
                if (collisionBoxs[j][i].collisionLayer == collisionLayer
                && collisionBoxs[j][i].collisionArea != null)
                    result = ShapeManager.IsCollision(collisionBoxs[j][i].collisionArea, shape) || result;
            }
        }
        return result;
    }

    /// <summary>
    /// 计算一个图形的感兴趣区域
    /// </summary>
    /// <param name="shape">目标多边形</param>
    /// <returns>感兴趣区域对应的瓦片索引</returns>
    private int[] GetFavouriteArea(IShape shape) {
        Rectangle favouriteArea = shape.GetSmallestAABBRectangle();
        // 瓦片索引对应：左上角i 左上角j 右下角i 右下角j
        float leftEdge = (float)favouriteArea.X / tileMap.TileWidth;
        float rightEdge = (float)(favouriteArea.X + favouriteArea.Width) / tileMap.TileWidth;
        float upEdge = (float)favouriteArea.Y / tileMap.TileHeight;
        float downEdge = (float)(favouriteArea.Y + favouriteArea.Height) / tileMap.TileHeight;
        int[] result = new int[4] {(int)leftEdge, (int)upEdge, (int)rightEdge + 1, (int)downEdge + 1};
        // 算出对应瓦片索引
        result[0] = LimitTheValue(result[0], 0, tileMap.MapWidth - 1);
        result[2] = LimitTheValue(result[2], 0, tileMap.MapWidth - 1);
        result[1] = LimitTheValue(result[1], 0, tileMap.MapHeight - 1);
        result[3] = LimitTheValue(result[3], 0, tileMap.MapHeight - 1);
        // 对指定值做限制
        return result;
    }

    /// <summary>
    /// 限制某个值在一个范围内（闭区间）
    /// </summary>
    private int LimitTheValue(int value, int minValue, int maxValue) {
        int result = value > maxValue ? maxValue : value;
        result = result < minValue ? minValue : result;
        return result;
    }

}

/// <summary>
/// 碰撞箱
/// </summary>
public class CollisionBox {

    /// <summary>
    /// 碰撞区域
    /// </summary>
    public Polygon collisionArea;

    /// <summary>
    /// 碰撞图层
    /// </summary>
    public int collisionLayer;

    /// <summary>
    /// 创建一个碰撞箱
    /// </summary>
    /// <param name="collisionArea">碰撞区域</param>
    /// <param name="collisionLayer">碰撞图层</param>
    public CollisionBox(Polygon collisionArea, int collisionLayer) {
        this.collisionArea = collisionArea;
        this.collisionLayer = collisionLayer;
    }

    /// <summary>
    /// 平移碰撞箱
    /// </summary>
    public virtual void Translate(Vector2 swiftPos) {
        collisionArea?.Translate(swiftPos);
    }

}