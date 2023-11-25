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
    public Particle(AnimatedSprite currentAnimation, Vector2 pos, Vector2 anchorPoint, float sizeScale, float angle = 0, SpriteEffects effects = SpriteEffects.None, int layer = 10) : base(currentAnimation, pos, anchorPoint, sizeScale, angle, effects, layer) {
    }
}