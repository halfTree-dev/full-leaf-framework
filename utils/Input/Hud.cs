/*
Hud，即平视信息显示，可以在游戏进行过程中显示游戏进行时的信息
从Json中读取一系列有关于一个游戏菜单的信息，然后显示出来，这就是Hud的功能~
*/

using System.Collections.Generic;
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

}