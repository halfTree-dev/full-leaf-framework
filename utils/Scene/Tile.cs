/*
Tile 就是瓦片地图中的瓦片
实际上就是单个的地块
现在提供一个Tile基类，可以储存一个地块的一些基本信息
可以写继承类，然后创建具有专门功能的地块，在地图中发挥作用
*/

using full_leaf_framework.Physics;
using full_leaf_framework.Visual;

namespace full_leaf_framework.Scene;

/// <summary>
/// 瓦片
/// </summary>
public class Tile {

    /// <summary>
    /// 瓦片的绘制效果
    /// </summary>
    public Drawable drawedTile;

    /// <summary>
    /// 使用的图集
    /// </summary>
    protected AnimatedSprite usedSprite;
    /// <summary>
    /// 使用帧（开始）
    /// </summary>
    protected int usedFrameL;
    /// <summary>
    /// 使用帧（结尾）
    /// </summary>
    protected int usedFrameR;
    /// <summary>
    /// 当前帧
    /// </summary>
    protected int currentFrame;
    /// <summary>
    /// 帧之间的延迟时间
    /// </summary>
    protected float frameDelay;
    /// <summary>
    /// 当前延迟时间计数器
    /// </summary>
    protected float currentDelay;
    protected Polygon collisionBox;
    /// <summary>
    /// 碰撞箱
    /// </summary>
    public Polygon CollisionBox { get => collisionBox; }

}
