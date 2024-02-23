/*
Menu 继承了 Hud
来做一些方法测试~
*/

namespace full_leaf_framework.Interact;

public class Menu : Hud {

    private const float gameTick = 1f / 60f;

    public void AnimationTick(IHudUnit hudUnit) {
        float time = hudUnit.GetAnimationTime();
        time += gameTick;
        hudUnit.SetAnimationTime(time);
    }

}