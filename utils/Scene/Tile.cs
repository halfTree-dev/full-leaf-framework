/*
Tile 就是瓦片地图中的瓦片
实际上就是单个的地块
现在提供一个Tile基类，可以储存一个地块的一些基本信息
可以写继承类，然后创建具有专门功能的地块，在地图中发挥作用
*/

using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Scene;

/// <summary>
/// 瓦片
/// </summary>
public class Tile {

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

    /// <summary>
    /// 填充瓦片基本内容
    /// </summary>
    /// <param name="usedSprite">使用的图集</param>
    /// <param name="usedFrameL">使用帧（开始）</param>
    /// <param name="usedFrameR">使用帧（结尾）</param>
    /// <param name="currentFrame">当前帧</param>
    /// <param name="frameDelay">帧之间的延迟时间</param>
    public void BeginTile(AnimatedSprite usedSprite, int usedFrameL, int usedFrameR, int currentFrame, float frameDelay) {
        this.usedSprite = usedSprite;
        this.usedFrameL = usedFrameL;
        this.usedFrameR = usedFrameR;
        this.currentFrame = currentFrame;
        this.frameDelay = frameDelay;
        currentDelay = frameDelay;
    }

    /// <summary>
    /// 更新瓦片，基类的更新仅包含动画
    /// </summary>
    public virtual void Update(GameTime gameTime) {
        currentDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (currentDelay <= 0f) {
            // 更新帧
            currentFrame++;
            if (currentFrame > usedFrameR) {
                currentFrame = usedFrameL;
            }
            currentDelay = frameDelay;
        }
    }

}
