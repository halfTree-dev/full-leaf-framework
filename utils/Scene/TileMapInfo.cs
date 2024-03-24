/*
为了从一个包含有TileMap相关数据的Json文件中读取相关数据，
我写了这个类。
*/

using Microsoft.Xna.Framework;
using full_leaf_framework.Visual;
using full_leaf_framework.Physics;
using Newtonsoft.Json;
using System.IO;

namespace full_leaf_framework.Scene;

/// <summary>
/// 瓦片地图信息
/// </summary>
public class TileMapInfo {

    /// <summary>
    /// 精灵图集的信息
    /// </summary>
    public SpriteInfo[] spriteInfos;
    /// <summary>
    /// 单个瓦片宽度
    /// </summary>
    public int tileWidth;
    /// <summary>
    /// 单个瓦片高度
    /// </summary>
    public int tileHeight;
    /// <summary>
    /// 瓦片地图列数
    /// </summary>
    public int mapWidth;
    /// <summary>
    /// 瓦片地图行数
    /// </summary>
    public int mapHeight;
    /// <summary>
    /// 瓦片地图信息
    /// </summary>
    public string[][] mapInfos;
    /// <summary>
    /// 瓦片将会在的图层
    /// </summary>
    public int layer;
    /// <summary>
    /// 瓦片信息列表
    /// </summary>
    public TileInfo[] tileInfos;
    /// <summary>
    /// 建筑信息列表
    /// </summary>
    public BuildingInfo[] buildingInfos;
    /// <summary>
    /// 瓦片物理信息列表
    /// </summary>
    public TilePhysicsInfo[] tilePhysics;

    /// <summary>
    /// 创建一个TileMapInfo对象并且从指定路径读取数据
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    public static TileMapInfo LoadTileMapInfo(string location) {
        string jsonContent = File.ReadAllText(location);
        TileMapInfo tileMapInfo = JsonConvert.DeserializeObject<TileMapInfo>(jsonContent);
        return tileMapInfo;
    }

}

/// <summary>
/// 精灵图集信息
/// </summary>
public class SpriteInfo {

    /// <summary>
    /// 该图集的名称
    /// </summary>
    public string unitName;
    /// <summary>
    /// 资源位置
    /// </summary>
    public string location;
    /// <summary>
    /// 读取的图集资源
    /// </summary>
    public AnimatedSprite texture;
    /// <summary>
    /// 图集的行数
    /// </summary>
    public int rows;
    /// <summary>
    /// 图集的列数
    /// </summary>
    public int column;

}

/// <summary>
/// 瓦片信息
/// </summary>
public class TileInfo {

    /// <summary>
    /// 瓦片对应要创建的对象名称
    /// </summary>
    public string tileClass;
    /// <summary>
    /// 瓦片在图表中的符号表示
    /// </summary>
    public string tileName;
    /// <summary>
    /// 使用的精灵图集名称
    /// </summary>
    public string usedSprite;
    /// <summary>
    /// 使用帧（开始）
    /// </summary>
    public int usedFrameL;
    /// <summary>
    /// 使用帧（结尾）
    /// </summary>
    public int usedFrameR;
    /// <summary>
    /// 帧之间的延迟时间
    /// </summary>
    public float frameDelay;
    /// <summary>
    /// 开始帧
    /// </summary>
    public int startFrame;
    /// <summary>
    /// 额外瓦片信息
    /// </summary>
    public string[] extArugs;

}

/// <summary>
/// 向量信息
/// </summary>
public class VectorInfo {

    /// <summary>
    /// X值
    /// </summary>
    public float X;
    /// <summary>
    /// Y值
    /// </summary>
    public float Y;

    /// <summary>
    /// 按照X与Y信息返回一个Vector2对象
    /// </summary>
    public Vector2 GetVector2() {
        return new Vector2(X, Y);
    }

}

/// <summary>
/// 建筑物信息
/// </summary>
public class BuildingInfo {

    /// <summary>
    /// 建筑物对应要创建的对象名称
    /// </summary>
    public string buildingClass;
    /// <summary>
    /// 动画信息
    /// </summary>
    public AnimationSpriteInfo spriteInfo;
    /// <summary>
    /// 位置的X值
    /// </summary>
    public float posX;
    /// <summary>
    /// 位置的Y值
    /// </summary>
    public float posY;
    /// <summary>
    /// 锚点的X值
    /// </summary>
    public float anchorPointX;
    /// <summary>
    /// 锚点的Y值
    /// </summary>
    public float anchorPointY;
    /// <summary>
    /// 放缩大小比值
    /// </summary>
    public float sizeScale;
    /// <summary>
    /// 旋转角度
    /// </summary>
    public float angle;
    /// <summary>
    /// 绘制图层
    /// </summary>
    public int layer;

}

/// <summary>
/// 瓦片物理效果信息
/// </summary>
public class TilePhysicsInfo {

    /// <summary>
    /// 瓦片在图表中的符号表示
    /// </summary>
    public string tileName;

    /// <summary>
    /// 碰撞箱向量信息
    /// </summary>
    public VectorInfo[] collisionBox;

    /// <summary>
    /// 碰撞箱所在的图层
    /// </summary>
    public int collisionLayer;

    /// <summary>
    /// 获取多边形类型的碰撞箱区域，是相对坐标
    /// 注意：返回值可以是空值，当没有设置碰撞箱时就是
    /// </summary>
    public Polygon GetCollisionBox() {
        if (collisionBox == null) {
            return null;
        }
        Vector2[] points = new Vector2[collisionBox.Length];
        for (int i = 0; i < collisionBox.Length; i++) {
            points[i] = collisionBox[i].GetVector2();
        }
        return new Polygon(points);
    }

}