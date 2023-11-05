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
    /// 向外展现的可绘制对象
    /// </summary>
    public Drawable drawable;

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

    public AnimatedSprite UsedSprite { get => usedSprite; set => usedSprite = value; }
    public int UsedFrameL { get => usedFrameL; set => usedFrameL = value; }
    public int UsedFrameR { get => usedFrameR; set => usedFrameR = value; }
    public int CurrentFrame { get => currentFrame; set => currentFrame = value; }
    public float FrameDelay { get => frameDelay; set => frameDelay = value; }
    public float CurrentDelay { get => currentDelay; set => currentDelay = value; }

    /// <summary>
    /// 填充瓦片基本内容
    /// </summary>
    /// <param name="usedSprite">使用的图集</param>
    /// <param name="usedFrameL">使用帧（开始）</param>
    /// <param name="usedFrameR">使用帧（结尾）</param>
    /// <param name="currentFrame">当前帧</param>
    /// <param name="frameDelay">帧之间的延迟时间</param>
    public void BeginTile(AnimatedSprite usedSprite, int usedFrameL, int usedFrameR, int currentFrame, float frameDelay) {
        this.UsedSprite = usedSprite;
        this.UsedFrameL = usedFrameL;
        this.UsedFrameR = usedFrameR;
        this.CurrentFrame = currentFrame;
        this.FrameDelay = frameDelay;
        CurrentDelay = frameDelay;
    }

    /// <summary>
    /// 更新瓦片，基类的更新仅包含动画
    /// </summary>
    public virtual void Update(GameTime gameTime) {
        CurrentDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (CurrentDelay <= 0f) {
            // 更新帧
            CurrentFrame++;
            if (CurrentFrame > UsedFrameR) {
                CurrentFrame = UsedFrameL;
            }
            CurrentDelay = FrameDelay;
        }
    }

}
