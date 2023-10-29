/*
Building 地图中的建筑物
这里只提供一个建筑物基类，包含场景中建筑物的一些关键信息
可以写继承类，让其发挥一些其他效果
*/

using full_leaf_framework.Physics;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace full_leaf_framework.Scene;

/// <summary>
/// 建筑物
/// </summary>
public class Building : Drawable
{

    /// <summary>
    /// 碰撞箱
    /// </summary>
    protected Polygon collisionBox;
    public Polygon CollisionBox { get => collisionBox; }

    public Building(AnimatedSprite currentAnimation,
        Vector2 pos,
        Vector2 anchorPoint,
        float sizeScale,
        float angle = 0,
        SpriteEffects effects = SpriteEffects.None,
        int layer = 0) : base(currentAnimation, pos, anchorPoint, sizeScale, angle, effects, layer) {
    }

}