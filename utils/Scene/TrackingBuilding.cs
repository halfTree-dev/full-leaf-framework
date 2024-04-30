/*
TrackingBuilding 测试动画轨迹用
*/

using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace full_leaf_framework.Scene;

/// <summary>
/// 沿轨迹运动的建筑物
/// </summary>
public class TrackingBuilding : Building
{

    /// <summary>
    /// 动画轨迹
    /// </summary>
    private AnimationTrack track;
    public float existTime = 0f;

    /// <summary>
    /// 填充一个建筑物
    /// </summary>
    public override void StartBuilding(AnimatedSprite currentAnimation,
        Vector2 pos,
        Vector2 anchorPoint,
        float sizeScale,
        float angle = 0,
        SpriteEffects effects = SpriteEffects.None,
        int layer = 0, object[] extArgus = null) {
            base.StartBuilding(currentAnimation, pos, anchorPoint, sizeScale,
            angle, effects, layer);
            track = MainGame.trackController.ReturnAnimationTrack("SampleAnimation1");
    }

    /// <summary>
    /// 更新建筑物，基类的更新仅包含动画
    /// </summary>
    public override void Update(GameTime gameTime) {
        base.Update(gameTime);
        existTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        // 处理动画效果
        AnimationResult result = track.ReturnAnimationResult(existTime);
        drawable.pos = result.pos;
        drawable.angle = result.angle;
        drawable.sizeScale = result.scale;
        drawable.transparency = result.transparency;
        if (existTime > track.animationMovement.maxTime){
            existTime = 0f;
        }
    }

}