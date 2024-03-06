/*
HudUnit，平视信息界面中的单个控件，
可以是图像，文字，进度条等诸如此类的显示信息的对象，
由Hud储存、控制，并统一绘制。

HudUnit本身只是一个接口，真正表现功能还得是它的继承类
*/

using System;
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
    public void Update(GameTime gameTime, InputManager input, Camera camera);
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
        switch (handler) {
            case "idle":
                if (idle is null) { return; }
                else {
                    Delegate[] delegates = idle.GetInvocationList();
                    foreach (Delegate @delegate in delegates) {
                        if (((MenuHandleEvent)@delegate.Target).Method.Name == handleEvent.Method.Name) {
                            idle -= (MenuHandleEvent)@delegate;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    public void SetAnimationTime(float time) {
        animationTime = time;
    }

    public void SetAnimationTrack(AnimationTrack animationTrack) {
        this.animationTrack = animationTrack;
    }

    public void SetCollsionBox(Polygon polygon) {
        collisionBox = polygon;
    }

    public void SetDrawObject(Drawable drawable) {
        this.drawable = drawable;
    }

    public virtual void SetEventToHandler(MenuHandleEvent handleEvent, string handler) {
        switch (handler) {
            case "idle":
                if (idle is null) { idle = new MenuHandleEvent(handleEvent);}
                else { idle += handleEvent; }
                break;
            default:
                break;
        }
    }

    public void SetName(string name) {
        this.name = name;
    }

    public virtual void Update(GameTime gameTime, InputManager input, Camera camera) {
        // 简单地赋予其动画
        if (animationTrack is not null)
            animationTrack.AffectDrawable(drawable, animationTime);
        if (idle is not null) { idle(this); }
    }
}

public class Button : Image, IHudUnit {

    /// <summary>
    /// 当鼠标指针不与碰撞箱重合时
    /// </summary>
    public MenuHandleEvent notFocus;
    /// <summary>
    /// 当鼠标指针与碰撞箱重合时
    /// </summary>
    public MenuHandleEvent focus;
    /// <summary>
    /// 按钮被点击
    /// </summary>
    public MenuHandleEvent fired;
    /// <summary>
    /// 按钮被长按
    /// </summary>
    public MenuHandleEvent hold;

    public override void SetEventToHandler(MenuHandleEvent handleEvent, string handler) {
        base.SetEventToHandler(handleEvent, handler);
        switch (handler) {
            case "notFocus":
                if (notFocus is null) { notFocus = new MenuHandleEvent(handleEvent);}
                else { notFocus += handleEvent; }
                break;
            case "focus":
                if (focus is null) { focus = new MenuHandleEvent(handleEvent);}
                else { focus += handleEvent; }
                break;
            case "fired":
                if (fired is null) { fired = new MenuHandleEvent(handleEvent);}
                else { fired += handleEvent; }
                break;
            case "hold":
                if (hold is null) { hold = new MenuHandleEvent(handleEvent);}
                else { hold += handleEvent; }
                break;
            default:
                break;
        }
    }

    public override void RemoveEventFromHandler(MenuHandleEvent handleEvent, string handler) {
        base.RemoveEventFromHandler(handleEvent, handler);
        MenuHandleEvent targetEvent = null;
        Delegate[] delegates = null;
        switch (handler) {
            case "notFocus":
                if (notFocus is null) { return; }
                else { delegates = notFocus.GetInvocationList(); targetEvent = notFocus; }
                break;
            case "focus":
                if (focus is null) { return; }
                else { delegates = focus.GetInvocationList(); targetEvent = focus; }
                break;
            case "fired":
                if (fired is null) { return; }
                else { delegates = fired.GetInvocationList(); targetEvent = fired; }
                break;
            case "hold":
                if (hold is null) { return; }
                else { delegates = hold.GetInvocationList(); targetEvent = hold; }
                break;
            default:
                break;
        }
        foreach (Delegate @delegate in delegates) {
            if (((MenuHandleEvent)@delegate.Target).Method.Name == handleEvent.Method.Name && targetEvent is not null) {
                targetEvent -= (MenuHandleEvent)@delegate;
            }
        }
    }

    public override void Update(GameTime gameTime, InputManager input, Camera camera) {
        base.Update(gameTime, input, camera);
        var mouse = input.GetTrackingMouse();
        Vector2 mousePos = mouse.pos.ToVector2();
        mousePos = camera.ReturnPointerPos(mousePos);
        if (ShapeManager.IsCollision(collisionBox, new Polygon(new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1)))) {
            focus.Invoke(this);
            if (mouse.firedLeft) { if (fired is not null) fired.Invoke(this); }
            if (mouse.pressedLeft) { if (hold is not null ) hold.Invoke(this);  }
        }
        else {
            notFocus.Invoke(this);
        }
    }

}

public class TestPolygon : Image {

    public int showingPolygon = 0;
    public Keys activeKey;

    public override void Update(GameTime gameTime, InputManager input, Camera camera)
    {
        base.Update(gameTime, input, camera);
        if (input.GetTrackingKey(activeKey).pressed) {
            var mouse = input.GetTrackingMouse();
            drawable.pos = mouse.pos.ToVector2();
        }
        drawable.settledFrame = showingPolygon;
    }

    public override void HandleExtArgus(object[] extArgus)
    {
        base.HandleExtArgus(extArgus);
        if ((long)extArgus[0] == 1) {
            activeKey = Keys.D1;
        }
        else if ((long)extArgus[0] == 2) {
            activeKey = Keys.D2;
        }
        else {
            activeKey = Keys.D3;
        }
        showingPolygon = Convert.ToInt32((long)extArgus[1]);
    }

}