/*
Menu 继承了 Hud
来做一些方法测试~
*/

using System;

namespace full_leaf_framework.Interact;

public class Menu : Hud {

    private const float gameTick = 1f / 60f;

    public void AnimationTick(IHudUnit hudUnit) {
        float time = hudUnit.GetAnimationTime();
        time += gameTick;
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

}