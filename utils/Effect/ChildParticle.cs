/*
众所周知，Particle类是可以继承的
ChildParticle就是对Particle的进一步实现
*/

using System;
using full_leaf_framework.Effect;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace full_leaf_framework.Effect;

/// <summary>
/// Particle的一个进一步实现
/// </summary>
public class ChildParticle : Particle {

    /// <summary>
    /// 每帧角度改变量
    /// </summary>
    public float angleChanging;

    public override void BeginParticle(ParticleController particleController, AnimatedSprite currentAnimation, Vector2 pos, Vector2 anchorPoint, float sizeScale, Vector2 velocity, float lifeTime, string[] extArugs, float angle = 0, SpriteEffects effects = SpriteEffects.None, int layer = 10)
    {
        base.BeginParticle(particleController, currentAnimation, pos, anchorPoint, sizeScale, velocity, lifeTime, extArugs, angle, effects, layer);
        // 利用额外数据来操作
        double deltaValue = new System.Random().NextDouble();
        angleChanging = float.Parse(extArugs[0]) + (float)deltaValue * (float.Parse(extArugs[1]) - float.Parse(extArugs[0]));
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // 转动
        float speedValue = Vector2.Distance(velocity, Vector2.Zero);
        float currentAngle = VelocityToAngle(velocity);
        currentAngle += angleChanging;
        velocity = new Vector2(MathF.Cos(currentAngle) * speedValue, MathF.Sin(currentAngle) * speedValue);
    }

    /// <summary>
    /// 返回速度方向的辐角
    /// </summary>
    public static float VelocityToAngle(Vector2 velocity) {
        if (velocity.Y == 0) {
            if (velocity.X >= 0) { return 0; } else { return (float)System.Math.PI; }
        }
        if (velocity.X == 0) {
            if (velocity.Y >= 0) { return (float)System.Math.PI / 2; }
            else { return (float)-System.Math.PI / 2; }
        }
        float pre_angle = (float)System.Math.Asin((double)(-velocity.Y /
        System.Math.Sqrt((double)(velocity.X * velocity.X + velocity.Y * velocity.Y))));
        if (pre_angle > 0) {
            if (velocity.X >= 0) { pre_angle = -pre_angle; }
            if (velocity.X < 0) { pre_angle = -(float)System.Math.PI + pre_angle; }
        }
        else {
            if (velocity.X > 0) { pre_angle = -pre_angle; }
            if (velocity.X == 0) { pre_angle = (float)System.Math.PI / 2; }
            if (velocity.X < 0) { pre_angle = (float)System.Math.PI + pre_angle; }
        }
        return pre_angle;
    }

}