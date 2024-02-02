/*
HudUnit，平视信息界面中的单个控件，
可以是图像，文字，进度条等诸如此类的显示信息的对象，
由Hud储存、控制，并统一绘制。

HudUnit本身只是一个接口，真正表现功能还得是它的继承类
*/

using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace full_leaf_framework.Interact;

/// <summary>
/// 平视信息显示的控件
/// </summary>
public interface IHudUnit {

    /// <summary>
    /// 获取控件名称
    /// </summary>
    public string GetName();
    /// <summary>
    /// 将指定委托绑定到控件的事件上
    /// </summary>
    /// <param name="handleEvent">委托</param>
    /// <param name="handler">控件事件名称</param>
    public void SetEventToHandler(Hud.MenuHandleEvent handleEvent, string handler);
    /// <summary>
    /// 为控件设置动画轨迹
    /// </summary>
    public void SetAnimationTrack(AnimationTrack animationTrack);
    /// <summary>
    /// 获取动画进行时间
    /// </summary>
    public float GetAnimationTime();
    /// <summary>
    /// 设置动画进行时间
    /// </summary>
    public void SetAnimationTime(float time);
    /// <summary>
    /// 更新控件
    /// </summary>
    public void Update(GameTime gameTime);
    /// <summary>
    /// 绘制控件
    /// </summary>
    public void Draw(Camera camera);

}

/// <summary>
/// 图像
/// </summary>
public class Image : IHudUnit {

    public string name;
    /// <summary>
    /// 要显示的图像
    /// </summary>
    public Drawable drawable;
    /// <summary>
    /// 动画信息
    /// </summary>
    public AnimationTrack animationTrack;
    /// <summary>
    /// 动画时间
    /// </summary>
    public float animationTime = 0f;

    public void Draw(Camera camera) {
        camera.insertObject(drawable);
    }

    public float GetAnimationTime() {
        return animationTime;
    }

    public string GetName() {
        return name;
    }

    public void SetAnimationTime(float time) {
        animationTime = time;
    }

    public void SetAnimationTrack(AnimationTrack animationTrack)
    {
        this.animationTrack = animationTrack;
    }

    public void SetEventToHandler(Hud.MenuHandleEvent handleEvent, string handler)
    {
        throw new System.NotImplementedException();
    }

    public void Update(GameTime gameTime) {
        // 简单地赋予其动画
        animationTrack.AffectDrawable(drawable, animationTime);
    }
}