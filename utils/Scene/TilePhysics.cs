/*
瓦片地图也要有物理效果！
所以我写了一个处理瓦片物理效果的类，
主要是碰撞这一块有很多要处理的，
需要注意的是这个物理系统只管瓦片的物理效果，不管建筑物的
*/

using System.Numerics;
using full_leaf_framework.Physics;

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
                foreach (TilePhysicsInfo tilePhysicsInfo in tileMapInfo.tilePhysics) {
                    if (currentTile == tilePhysicsInfo.tileName) {
                        Polygon collisionArea = tilePhysicsInfo.GetCollisionBox();
                        collisionArea.Translate(shiftPos);
                        // 创建相应的碰撞区域
                        collisionBoxs[i][j] = new CollisionBox(collisionArea, tilePhysicsInfo.collisionLayer);
                        break;
                    }
                }
            }
        }
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

}