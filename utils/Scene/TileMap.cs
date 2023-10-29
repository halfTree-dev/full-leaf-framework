/*
TileMap 瓦片地图
一种常用的地图构建方法，将地图分割为整齐的地块，
并对每个地块的数据分别编辑。
对于地图中重复元素较多的情况下，这种方式能够提升编辑效率
*/

using Microsoft.Xna.Framework;

namespace full_leaf_framework.Scene;

/// <summary>
/// 瓦片地图主体
/// </summary>
public class TileMap {

    /// <summary>
    /// 精灵图集信息
    /// </summary>
    private SpriteInfo[] spriteInfos;
    private int tileWidth;
    /// <summary>
    /// 瓦片的宽度
    /// </summary>
    public int TileWidth { get => tileWidth; }
    private int tileHeight;
    /// <summary>
    /// 瓦片的高度
    /// </summary>
    public int TileHeight { get => tileHeight; }
    private int mapWidth;
    /// <summary>
    /// 瓦片地图列数
    /// </summary>
    public int MapWidth { get => mapWidth; }
    private int mapHeight;
    /// <summary>
    /// 瓦片地图行数
    /// </summary>
    public int MapHeight { get => mapHeight; }

    /// <summary>
    /// 瓦片集合
    /// </summary>
    public Tile[][] tiles;
    // 注：认为瓦片地图的最左上角是(0, 0)
    // 每个瓦片的左上角是其相对坐标的(0, 0)
    // 瓦片的各项初始化要在读取数据后就完成

    /// <summary>
    /// 建筑物集合
    /// </summary>
    public Building[] buildings;
    // 注：建筑物本身就是一个Drawable对象

    /// <summary>
    /// 初始化瓦片地图
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    public TileMap(string location) {
        TileMapInfo tileMapInfo = TileMapInfo.LoadTileMapInfo(location);
        // 以下是对数据的各项处理
    }

}