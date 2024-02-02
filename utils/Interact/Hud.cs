/*
Hud，即平视信息显示，可以在游戏进行过程中显示游戏进行时的信息
从Json中读取一系列有关于一个游戏菜单的信息，然后显示出来，这就是Hud的功能~

实际使用的时候，必须将Hud【继承】出来，因为你自己要写方法对吧。
*/

using System.Collections.Generic;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace full_leaf_framework.Interact;

/// <summary>
/// 平视信息显示
/// </summary>
public class Hud {

    /// <summary>
    /// 控件列表
    /// </summary>
    public List<IHudUnit> hudUnits;
    /// <summary>
    /// 动画轨迹控制器
    /// </summary>
    public AnimationTrackController animationTrackController;

    /// <summary>
    /// 菜单事件的委托
    /// </summary>
    public delegate void MenuHandleEvent(object[] argus);

}