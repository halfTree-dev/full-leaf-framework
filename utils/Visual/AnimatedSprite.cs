/*
AnimatedSprite.cs
通过精灵图集来构建动画对象。
辅助Drawable类型来使用。
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace full_leaf_framework.Visual;

/// <summary>
/// 动画对象
/// </summary>
public class AnimatedSprite {

    /// <summary>
    /// 图集材质
    /// </summary>
    private Texture2D texture;
    /// <summary>
    /// 图集的行数
    /// </summary>
    private int rows;
    /// <summary>
    /// 图集的列数
    /// </summary>
    private int columns;
    /// <summary>
    /// 当前对应帧
    /// </summary>
    private int currentFrame;
    public int CurrentFrame { get => currentFrame;
    set => currentFrame = value; }
    /// <summary>
    /// 总帧数
    /// </summary>
    private int totalFrames;
    /// <summary>
    /// 帧之间的间隔时间
    /// </summary>
    private float frameDelay;
    /// <summary>
    /// 帧切换计时器
    /// </summary>
    private float currentFrameDelay;

    private int width;
    /// <summary>
    /// 单个精灵的宽度
    /// </summary>
    public int Width { get => width; }
    private int height;
    /// <summary>
    /// 单个精灵的高度
    /// </summary>
    public int Height { get => height; }

    /// <summary>
    /// 创建一个动画对象
    /// </summary>
    /// <param name="texture">图集材质</param>
    /// <param name="rows">行数</param>
    /// <param name="columns">列数</param>
    /// <param name="frameDelay">帧之间的间隔时间</param>
    /// <param name="startFrame">开始的帧数</param>
    public AnimatedSprite(Texture2D texture, int rows = 1, int columns = 1,
    float frameDelay = 0f, int startFrame = 0) {
        this.texture = texture;
        this.rows = rows;
        this.columns = columns;
        currentFrame = startFrame;
        totalFrames = rows * columns;
        this.frameDelay = frameDelay;
        currentFrameDelay = 0f;
        this.width = texture.Width / columns;
        this.height = texture.Height / rows;
    }

    /// <summary>
    /// 返回动画的单帧大小
    /// </summary>
    public Vector2 ReturnFrameSize() {
        return new Vector2(width, height);
    }

    /// <summary>
    /// 刷新该动画
    /// </summary>
    public void Update(GameTime gameTime) {
        currentFrameDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (currentFrameDelay > frameDelay) {
            currentFrame++;
            // currentFrame从0开始，一直到(totalFrames-1)帧
            if (currentFrame >= totalFrames) {
                currentFrame = 0;
            }
            currentFrameDelay = 0f;
        }
    }

    /// <summary>
    /// 在一个上下界范围中刷新该动画
    /// </summary>
    /// <param name="down">帧数下界</param>
    /// <param name="up">帧数上界</param>
    public void Update(GameTime gameTime, int down, int up) {
        if (currentFrame < down) { currentFrame = down; }
        if (currentFrame > up) { currentFrame = up; }
        currentFrameDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (currentFrameDelay > frameDelay) {
            currentFrame++;
            if (currentFrame > up) {
                currentFrame = down;
            }
            currentFrameDelay = 0f;
        }
    }

    /// <summary>
    /// 绘制动画，
    /// 当使用Draw方法时，动画会被居中绘制在指定坐标，
    /// </summary>
    /// <param name="location">位置</param>
    /// <param name="anchorPoint">锚点</param>
    /// <param name="effects">绘制附加效果</param>
    /// <param name="angle">旋转角度</param>
    /// <param name="scale">缩放比值</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 location, Vector2 anchorPoint,
    SpriteEffects effects = SpriteEffects.None, float angle = 0, float scale = 1, Rectangle? drawArea = null) {
        int currentRow = currentFrame / columns;
        int currentColumn = currentFrame % columns;
        // 截取对应图集中的单个精灵
        Rectangle sourceRectangle;
        if (drawArea == null) {
            sourceRectangle = new Rectangle(width * currentColumn,
            height * currentRow, width, height);
        }
        else {
            // 改变截取区域
            Rectangle interceptArea = (Rectangle)drawArea;
            sourceRectangle = new Rectangle(width * currentColumn + interceptArea.X,
            height * currentRow + interceptArea.Y, interceptArea.Width, interceptArea.Height);
        }
        // 绘制它（锚点引起的坐标改变被提前处理）
        spriteBatch.Draw(texture, location,
        sourceRectangle, Color.White, angle, new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2) + anchorPoint,
        scale, effects, 1);
        // 破案了，spriteBatch默认是居中绘制，好吧，我输了，居然花了许久去考虑这一点
        // 又破案了，原来origin居然还会决定绘制的中心位置，我去，我居然还花了这么久调坐标
        /*
        最后再来总结一下spriteBatch的绘制流程：
        通过origin决定图片的中心位置，然后根据location将图片绘制，接着再以这个中心位置进行scale
        所以Camera只需要给出转化后的pos即可，无需其他处理
        */
    }

}

/// <summary>
/// 动画信息对象
/// </summary>
public class AnimationSpriteInfo {

    /// <summary>
    /// 资源位置
    /// </summary>
    public string location;
    /// <summary>
    /// 行数
    /// </summary>
    public int rows;
    /// <summary>
    /// 列数
    /// </summary>
    public int column;
    /// <summary>
    /// 帧间隔时间
    /// </summary>
    public float frameDelay;
    /// <summary>
    /// 起始帧
    /// </summary>
    public int startFrame;

    /// <summary>
    /// 返回动画对象
    /// </summary>
    public AnimatedSprite ReturnAnimation(ContentManager Content) {
        return new AnimatedSprite(Content.Load<Texture2D>(location), rows,
        column, frameDelay, startFrame);
    }

}