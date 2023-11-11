/*
TileMap 瓦片地图
一种常用的地图构建方法，将地图分割为整齐的地块，
并对每个地块的数据分别编辑。
对于地图中重复元素较多的情况下，这种方式能够提升编辑效率
*/

using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using System.Collections.Generic;
using full_leaf_framework.Physics;

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
    /// 瓦片将会在的图层
    /// </summary>
    private int layer;
    public int Layer { get => layer; }

    /// <summary>
    /// 瓦片集合
    /// </summary>
    public Tile[][] tiles;
    // 注：认为瓦片地图的最左上角是(0, 0)
    // 每个瓦片的左上角是其相对坐标的(0, 0)
    // 瓦片的各项初始化要在读取数据后就完成

    /// <summary>
    /// 建筑物集合
    /// 这是动态列表，所以你随时可以加入新的建筑物
    /// </summary>
    public List<Building> buildings;

    /// <summary>
    /// 瓦片效果集合
    /// 可以给瓦片规定碰撞体积，然后由它来判定
    /// </summary>
    public TilePhysics tilePhysics;

    private ContentManager Content;

    /// <summary>
    /// 初始化瓦片地图
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    public TileMap(string location, ContentManager Content) {
        TileMapInfo tileMapInfo = TileMapInfo.LoadTileMapInfo(location);
        this.Content = Content;
        // 以下是对数据的各项处理
        tileWidth = tileMapInfo.tileWidth;
        tileHeight = tileMapInfo.tileHeight;
        mapWidth = tileMapInfo.mapWidth;
        mapHeight = tileMapInfo.mapHeight;
        layer = tileMapInfo.layer;
        // 填充基本数据
        LoadSprites(tileMapInfo);
        LoadTiles(tileMapInfo);
        LoadBuildings(tileMapInfo);
        LoadPhysics(tileMapInfo);
        // 读取相关资源
    }

    #region 读取资源

    /// <summary>
    /// 读取SpriteInfos
    /// </summary>
    private void LoadSprites(TileMapInfo tileMapInfo) {
        spriteInfos = tileMapInfo.spriteInfos;
        foreach (SpriteInfo spriteInfo in spriteInfos) {
            spriteInfo.texture = new AnimatedSprite(Content.Load<Texture2D>(spriteInfo.location),
            spriteInfo.rows, spriteInfo.column);
        }
    }

    /// <summary>
    /// 读取瓦片
    /// </summary>
    private void LoadTiles(TileMapInfo tileMapInfo) {
        // 用来参考的tile信息
        tiles = new Tile[mapHeight][];
        // 生成地图
        Assembly assembly = Assembly.GetExecutingAssembly();
        // 获取当前程序集（为了创建瓦片）
        for (int i = 0; i < tileMapInfo.mapInfos.Length; i++) {
            tiles[i] = new Tile[mapWidth];
            for (int j = 0; j < tileMapInfo.mapInfos[i].Length; j++) {
                // 用tileMapInfo[i][j]表示第i+1行j+1列的瓦片
                // 从tileInfos里查找与字符相匹配的瓦片类型
                foreach (TileInfo tileInfo in tileMapInfo.tileInfos) {
                    if (tileInfo.tileName == tileMapInfo.mapInfos[i][j]) {
                        // 找到了，然后将对应的Tile按照类名初始化
                        // 必须填写类的完全限定名，这点应当在Json中体现
                        dynamic obj = assembly.CreateInstance(tileInfo.tileClass);
                        Tile tile = (Tile)obj;
                        foreach (SpriteInfo spriteInfo in spriteInfos) {
                            if (tileInfo.usedSprite == spriteInfo.unitName) {
                                // 找到图集，那么创建瓦片
                                tile.BeginTile(spriteInfo.texture, tileInfo.usedFrameL,
                                tileInfo.usedFrameR, tileInfo.startFrame, tileInfo.frameDelay, tileInfo.extArugs);
                                // 对每个Tile对象生成Drawable
                                tile.drawable = new Drawable(tile.UsedSprite, new Vector2(j * tileWidth, i * tileHeight),
                                new Vector2(-tile.UsedSprite.Width / 2, -tile.UsedSprite.Height / 2),
                                tileWidth / tile.UsedSprite.Width, 0, SpriteEffects.None, layer);
                            }
                        }
                        tiles[i][j] = tile;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 读取建筑物
    /// </summary>
    private void LoadBuildings(TileMapInfo tileMapInfo) {
        buildings = new List<Building>();
        // 创建建筑列表
        Assembly assembly = Assembly.GetExecutingAssembly();
        // 获取当前程序集（为了创建建筑物）
        for (int i = 0; i < tileMapInfo.buildingInfos.Length; i++) {
            // 按照名称创建对应建筑
            dynamic obj = assembly.CreateInstance(tileMapInfo.buildingInfos[i].buildingClass);
            Building building = (Building)obj;
            BuildingInfo currentInfo = tileMapInfo.buildingInfos[i];
            building.StartBuilding(currentInfo.spriteInfo.ReturnAnimation(Content),
            new Vector2(currentInfo.posX, currentInfo.posY), new Vector2(currentInfo.anchorPointX, currentInfo.anchorPointY),
            currentInfo.sizeScale, currentInfo.angle, SpriteEffects.None, currentInfo.layer);
            buildings.Add(building);
        }
    }

    /// <summary>
    /// 读取瓦片地图的物理信息
    /// </summary>
    private void LoadPhysics(TileMapInfo tileMapInfo) {
        tilePhysics = new TilePhysics(this, tileMapInfo);
    }

    #endregion

    /// <summary>
    /// 更新瓦片地图
    /// </summary>
    public void Update(GameTime gameTime) {
        for (int i = 0; i < tiles.Length; i++) {
            for (int j = 0; j < tiles[0].Length; j++) {
                tiles[i][j].Update(gameTime);
            }
        }
        foreach (Building building in buildings) {
            building.Update(gameTime);
        }
    }

    /// <summary>
    /// 检测任意图形和瓦片地图的瓦片是否存在碰撞
    /// </summary>
    /// <param name="shape">任意图形</param>
    /// <param name="collisionLayer">碰撞图层</param>
    public bool IsCollision(Shape shape, int collisionLayer) {
        return tilePhysics.IsCollision(shape, collisionLayer);
    }

    /// <summary>
    /// 绘制瓦片地图
    /// </summary>
    public void Draw(Camera camera) {
        // 向camera中添加相应的对象
        // 但是注意，对于每个Tile，它们共用一个AnimatedSprite，所以务必做好区分
        for (int i = 0; i < tiles.Length; i++) {
            for (int j = 0; j < tiles[0].Length; j++) {
                // 利用瓦片的Drawable对象
                tiles[i][j].drawable.settledFrame = tiles[i][j].CurrentFrame;
                camera.insertObject(tiles[i][j].drawable);
            }
        }
        foreach (Building building in buildings) {
            camera.insertObject(building.drawable);
        }
    }

}