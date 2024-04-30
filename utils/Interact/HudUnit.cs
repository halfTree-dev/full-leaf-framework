/*
HudUnit，平视信息界面中的单个控件，
可以是图像，文字，进度条等诸如此类的显示信息的对象，
由Hud储存、控制，并统一绘制。

HudUnit本身只是一个接口，真正表现功能还得是它的继承类
*/

using System;
using System.Reflection;
using full_leaf_framework.Physics;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static full_leaf_framework.Interact.Hud;

namespace full_leaf_framework.Interact;

/// <summary>
/// 平视信息显示的控件
/// </summary>
public interface IHudUnit {

    /// <summary>
    /// 设置控件名称
    /// </summary>
    public void SetName(string name);
    /// <summary>
    /// 获取控件名称
    /// </summary>
    public string GetName();
    /// <summary>
    /// 设置控件的碰撞箱
    /// </summary>
    public void SetCollsionBox(Polygon polygon);
    /// <summary>
    /// 获取控件的碰撞箱
    /// </summary>
    public Polygon GetCollisionBox();
    /// <summary>
    /// 设置可绘制对象
    /// </summary>
    public void SetDrawObject(Drawable drawable);
    /// <summary>
    /// 将指定委托绑定到控件的事件上
    /// </summary>
    /// <param name="handleEvent">委托</param>
    /// <param name="handler">控件事件名称</param>
    public void SetEventToHandler(MenuHandleEvent handleEvent, string handler);
    /// <summary>
    /// 将控件的事件从委托上移除
    /// </summary>
    /// <param name="handleEvent">委托</param>
    /// <param name="handler">控件事件名称</param>
    public void RemoveEventFromHandler(MenuHandleEvent handleEvent, string handler);
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
    /// <summary>
    /// 处理额外参数
    /// </summary>
    public void HandleExtArgus(object[] extArgus);

}

/// <summary>
/// 基本控件
/// </summary>
public class BasicUnit : IHudUnit {

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
    /// <summary>
    /// 控件的碰撞箱
    /// </summary>
    public Polygon collisionBox;

    // 这些是菜单控件的委托，会被控件在指定的条件下执行的
    public MenuHandleEvent idle;

    public void Draw(Camera camera) {
        camera.insertObject(drawable);
    }

    public float GetAnimationTime() {
        return animationTime;
    }

    public Polygon GetCollisionBox() {
        return collisionBox;
    }

    public string GetName() {
        return name;
    }

    public virtual void HandleExtArgus(object[] extArgus) {
    }

    public virtual void RemoveEventFromHandler(MenuHandleEvent handleEvent, string handler) {
        try {
            FieldInfo eventField = GetType().GetField(handler);
            object unitEvents = eventField.GetValue(this);
            // 获取handler所对应的事件
            if (unitEvents is null) { return; }
            else {
                var unitEventsM = (MenuHandleEvent)unitEvents;
                Delegate[] delegates = unitEventsM.GetInvocationList();
                foreach (Delegate @delegate in delegates) {
                    if (((MenuHandleEvent)@delegate.Target).Method.Name == handleEvent.Method.Name) {
                        unitEventsM -= (MenuHandleEvent)@delegate;
                        eventField.SetValue(this, unitEventsM);
                    }
                }
            }
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return;
        }
    }

    public void SetAnimationTime(float time) {
        animationTime = time;
    }

    public void SetAnimationTrack(AnimationTrack animationTrack) {
        this.animationTrack = animationTrack.TranslateTrack(drawable.pos);
    }

    public void SetCollsionBox(Polygon polygon) {
        collisionBox = polygon;
    }

    public void SetDrawObject(Drawable drawable) {
        this.drawable = drawable;
    }

    public virtual void SetEventToHandler(MenuHandleEvent handleEvent, string handler) {
        try {
            FieldInfo eventField = GetType().GetField(handler);
            object unitEvents = eventField.GetValue(this);
            // 获取handler所对应的事件
            if (unitEvents is not null) {
                var unitEventsM = (MenuHandleEvent)unitEvents;
                unitEventsM += handleEvent;
                eventField.SetValue(this, unitEventsM);
            }
            else {
                eventField.SetValue(this, handleEvent);
            }
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return;
        }
    }

    public void SetName(string name) {
        this.name = name;
    }

    public virtual void Update(GameTime gameTime) {
        // 简单地赋予其动画
        if (animationTrack is not null)
            animationTrack.AffectDrawable(drawable, animationTime);
        if (idle is not null) { idle(this); }
    }
}