/*
Particle 即粒子效果，
这里描述一个单独的粒子，
结合粒子控制器，可以做到对粒子效果的很好控制
*/

using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace full_leaf_framework.Effect;

/// <summary>
/// 单个粒子
/// </summary>
public class Particle : Drawable
{

    /// <summary>
    /// 上级控制器
    /// </summary>
    protected ParticleController controller;

    // 动画信息
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

    // 粒子属性
    /// <summary>
    /// 粒子运行速度（每秒位移）
    /// </summary>
    protected Vector2 velocity;
    /// <summary>
    /// 剩余存在时间
    /// </summary>
    protected float lifeTime;
    /// <summary>
    /// 总存在时间
    /// </summary>
    protected float maxLifeTime;

    public Vector2 Velocity { get => velocity; }
    public float LifeTime { get => lifeTime; }
    public float MaxLifeTime { get => maxLifeTime; }
    public int UsedFrameL { get => usedFrameL; set => usedFrameL = value; }
    public int UsedFrameR { get => usedFrameR; set => usedFrameR = value; }
    public int CurrentFrame { get => currentFrame; set => currentFrame = value; }
    public float FrameDelay { get => frameDelay; set => frameDelay = value; }
    public float CurrentDelay { get => currentDelay; set => currentDelay = value; }

    public Particle(ParticleController particleController, AnimatedSprite currentAnimation, Vector2 pos, Vector2 anchorPoint, float sizeScale, Vector2 velocity,
                    float lifeTime, string[] extArugs, float angle = 0, SpriteEffects effects = SpriteEffects.None, int layer = 10) : base(currentAnimation, pos, anchorPoint, sizeScale, angle, effects, layer) {
        controller = particleController;
        this.velocity = velocity;
        this.lifeTime = lifeTime;
        maxLifeTime = lifeTime;
        // 在设置BeginAnimation前锁定动画
        settledFrame = 0;
        CurrentDelay = float.MaxValue;
        // extArugs用于给继承类额外传值
    }

    /// <summary>
    /// 为Particle设置其动画
    /// </summary>
    /// <param name="usedFrameL">使用帧（开始）</param>
    /// <param name="usedFrameR">使用帧（结尾）</param>
    /// <param name="currentFrame">当前帧</param>
    /// <param name="frameDelay">帧之间的延迟时间</param>
    public void BeginAnimation(int usedFrameL, int usedFrameR, int currentFrame, float frameDelay) {
        UsedFrameL = usedFrameL;
        UsedFrameR = usedFrameR;
        CurrentFrame = currentFrame;
        FrameDelay = frameDelay;
        CurrentDelay = frameDelay;
    }

    /// <summary>
    /// 更新粒子
    /// </summary>
    public virtual void Update(GameTime gameTime) {
        // 更新位置
        pos += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        // 检查生命周期
        lifeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (lifeTime <= 0f) { controller.particles.Remove(this); return; }
        // 切换动画
        CurrentDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (CurrentDelay <= 0f) {
            // 更新帧
            CurrentFrame++;
            if (CurrentFrame > UsedFrameR) {
                CurrentFrame = UsedFrameL;
            }
            CurrentDelay = FrameDelay;
        }
        settledFrame = CurrentFrame;
    }

}