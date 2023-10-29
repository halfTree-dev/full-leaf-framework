/*
为了从一个包含有TileMap相关数据的Json文件中读取相关数据，
我写了这个类。
*/

using Microsoft.Xna.Framework;
using full_leaf_framework.Visual;
using Newtonsoft.Json;
using System.IO;

namespace full_leaf_framework.Scene;

/// <summary>
/// 瓦片地图信息
/// </summary>
internal class TileMapInfo {

    /// <summary>
    /// 精灵图集的信息
    /// </summary>
    internal SpriteInfo[] spriteInfos;
    /// <summary>
    /// 单个瓦片宽度
    /// </summary>
    internal int tileWidth;
    /// <summary>
    /// 单个瓦片高度
    /// </summary>
    internal int tileHeight;
    /// <summary>
    /// 瓦片地图列数
    /// </summary>
    internal int mapWidth;
    /// <summary>
    /// 瓦片地图行数
    /// </summary>
    internal int mapHeight;
    /// <summary>
    /// 瓦片地图信息
    /// </summary>
    internal string[][] mapInfos;
    /// <summary>
    /// 瓦片信息列表
    /// </summary>
    internal TileInfo[] tileInfos;
    /// <summary>
    /// 建筑信息列表
    /// </summary>
    internal BuildingInfo[] buildingInfos;

    /// <summary>
    /// 创建一个TileMapInfo对象并且从指定路径读取数据
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    internal static TileMapInfo LoadTileMapInfo(string location) {
        string jsonContent = File.ReadAllText(location);
        TileMapInfo tileMapInfo = JsonConvert.DeserializeObject<TileMapInfo>(jsonContent);
        return tileMapInfo;
    }

}

/// <summary>
/// 精灵图集信息
/// </summary>
internal class SpriteInfo {

    /// <summary>
    /// 该图集的名称
    /// </summary>
    internal string unitName;
    /// <summary>
    /// 资源位置
    /// </summary>
    internal string location;
    /// <summary>
    /// 读取的图集资源
    /// </summary>
    internal AnimatedSprite texture;
    /// <summary>
    /// 图集的行数
    /// </summary>
    internal int rows;
    /// <summary>
    /// 图集的列数
    /// </summary>
    internal int column;

}

/// <summary>
/// 瓦片信息
/// </summary>
internal class TileInfo {

    /// <summary>
    /// 瓦片对应要创建的对象名称
    /// </summary>
    internal string tileClass;
    /// <summary>
    /// 瓦片在图表中的符号表示
    /// </summary>
    internal string tileName;
    /// <summary>
    /// 使用的精灵图集名称
    /// </summary>
    internal string usedSprite;
    /// <summary>
    /// 使用帧（开始）
    /// </summary>
    internal int usedFrameL;
    /// <summary>
    /// 使用帧（结尾）
    /// </summary>
    internal int usedFrameR;
    /// <summary>
    /// 帧之间的延迟时间
    /// </summary>
    internal float frameDelay;
    /// <summary>
    /// 开始帧
    /// </summary>
    internal float startFrame;
    /// <summary>
    /// 碰撞箱（可以为null）
    /// </summary>
    internal VectorInfo[] collisionBox;

}

/// <summary>
/// 向量信息
/// </summary>
internal class VectorInfo {

    /// <summary>
    /// X值
    /// </summary>
    internal float X;
    /// <summary>
    /// Y值
    /// </summary>
    internal float Y;

    /// <summary>
    /// 按照X与Y信息返回一个Vector2对象
    /// </summary>
    internal Vector2 GetVector2() {
        return new Vector2(X, Y);
    }

}

/// <summary>
/// 建筑物信息
/// </summary>
internal class BuildingInfo {

    /// <summary>
    /// 建筑物对应要创建的对象名称
    /// </summary>
    internal string buildingClass;
    /// <summary>
    /// 动画信息
    /// </summary>
    internal AnimationInfo spriteInfo;
    /// <summary>
    /// 位置的X值
    /// </summary>
    internal float posX;
    /// <summary>
    /// 位置的Y值
    /// </summary>
    internal float posY;
    /// <summary>
    /// 锚点的X值
    /// </summary>
    internal float anchorPointX;
    /// <summary>
    /// 锚点的Y值
    /// </summary>
    internal float anchorPointY;
    /// <summary>
    /// 放缩大小比值
    /// </summary>
    internal float sizeScale;
    /// <summary>
    /// 绘制图层
    /// </summary>
    internal int layer;
    /// <summary>
    /// 碰撞箱
    /// </summary>
    internal VectorInfo[] collisionBox;

}