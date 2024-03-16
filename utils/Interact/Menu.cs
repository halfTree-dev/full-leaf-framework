/*
Menu 继承了 Hud
来做一些方法测试~
*/

using System;
using full_leaf_framework.Physics;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Interact;

public class Menu : Hud {

    public InputManager inputManager;
    public const float gameFrame = 1f / 60f;
    public Camera camera;

    public void FillMenu(InputManager inputManager, Camera camera) {
        this.inputManager = inputManager;
        this.camera = camera;
    }

    public void AnimationTick(IHudUnit hudUnit) {
        float time = hudUnit.GetAnimationTime();
        time += gameFrame;
        hudUnit.SetAnimationTime(time);
    }

    public void ClickOn(IHudUnit hudUnit) {
        Console.WriteLine("Clicked!");
    }

    public void ChangeFocus(IHudUnit hudUnit) {
        var button = (Button)hudUnit;
        button.drawable.settledFrame = 1;
    }

    public void ChangeNotFocus(IHudUnit hudUnit) {
        var button = (Button)hudUnit;
        button.drawable.settledFrame = 0;
    }

    public void ButtonActiveEvent(IHudUnit hudUnit) {
        var button = (Button)hudUnit;
        var mouse = inputManager.GetTrackingMouse();
        Vector2 mousePos = mouse.pos.ToVector2();
        mousePos = camera.ReturnPointerPos(mousePos);
        if (ShapeManager.IsCollision(button.collisionBox, new Circle(mouse.pos.ToVector2(), 1))) {
            button.focus.Invoke(button);
            if (mouse.firedLeft) { if (button.fired is not null) button.fired.Invoke(button); }
            if (mouse.pressedLeft) { if (button.hold is not null ) button.hold.Invoke(button);  }
        }
        else {
            button.notFocus.Invoke(button);
        }
    }

}