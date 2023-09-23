/*
Drawable.cs
描述一个可以被Camera类经过处理后绘制的对象
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace full_leaf_framework.Visual;

/// <summary>
/// 可绘制对象
/// </summary>
public class Drawable {

    /// <summary>
    /// 当前的动画
    /// </summary>
    public AnimatedSprite currentAnimation;
    /// <summary>
    /// 动画在游戏内表现的实际大小
    /// </summary>
    public float sizeScale;
    // #warning 不行，这样不能适应不同大小的Camera范围
    // 实际上这个写法没有问题，这是对一个一定大小的精灵做固定比例的放缩，其实也是在本质上修改了图片的基础大小
    // 那我可能会问了：欸，我要是摄像机的囊括范围更改了怎么办？
    // 问题不大，因为游戏内修改使用scale不冲突，并且你要是要改cameraRange的话，那就算预处理，这个也行。
    /// <summary>
    /// 所处位置
    /// </summary>
    public Vector2 pos;
    /// <summary>
    /// 绘制效果
    /// </summary>
    public SpriteEffects effects = SpriteEffects.None;
    /// <summary>
    /// 旋转角度
    /// </summary>
    public float angle = 0;
    /// <summary>
    /// 绘制锚点，
    /// 当绘制时，这个点会在指定的绘制坐标处，
    /// 需要注意的是，图片的中心是(0,0)
    /// </summary>
    public Vector2 anchorPoint;
    /// <summary>
    /// 绘制的优先级，越小越高，越容易绘制在底层
    /// </summary>
    public int layer;

    /// <summary>
    /// 创建Drawable对象
    /// </summary>
    /// <param name="currentAnimation">当前动画</param>
    /// <param name="pos">所处位置</param>
    /// <param name="anchorPoint">锚点</param>
    /// <param name="angle">旋转角度</param>
    /// <param name="effects">绘制效果</param>
    /// <param name="size">动画在游戏内表现的实际大小的倍率</param>
    /// <param name="layer">绘制的优先级，越小越高，越容易绘制在底层</param>
    public Drawable(AnimatedSprite currentAnimation, Vector2 pos, Vector2 anchorPoint, float sizeScale,
    float angle = 0f, SpriteEffects effects = SpriteEffects.None, int layer = 0) {
        this.currentAnimation = currentAnimation;
        this.pos = pos;
        this.anchorPoint = anchorPoint;
        this.sizeScale = sizeScale;
        this.angle = angle;
        this.effects = effects;
        this.layer = layer;
    }

}