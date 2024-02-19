/*
HudController 顾名思义，就是管控菜单界面的对象
这里可以通过读取相关Hud的Json信息来创建菜单，还可以做到指定菜单的激活与否，调用指定的方法等
*/

using System.Collections.Generic;
using System.Reflection;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework.Content;

namespace full_leaf_framework.Interact;

/// <summary>
/// 菜单界面控制器
/// </summary>
public class HudController {

    /// <summary>
    /// 菜单界面的集合
    /// </summary>
    public Dictionary<string, Hud> huds;
    /// <summary>
    /// 菜单界面状态集合
    /// </summary>
    public Dictionary<string, HudStatus> hudStatus;

    /// <summary>
    /// 程序集详情（包含Hud对象之类的）
    /// </summary>
    public Assembly assembly;

    /// <summary>
    /// 在控制器中加入一个Hud
    /// </summary>
    /// <param name="location">读取该Hud的Json路径</param>
    /// <param name="isActive">Hud是否被激活</param>
    /// <param name="isVisible">Hud是否可见</param>
    public void AddHud(string location, bool isActive = true, bool isVisible = true) {

    }

    /// <summary>
    /// 按照指定的Json文件生成相应的菜单对象
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    private Hud CreateHud(string location, ContentManager Content) {
        var hudInfo = HudInfo.LoadHudInfo(location);
        assembly = Assembly.GetExecutingAssembly();
        // 获取当前程序集（为了创建指定的Hud）
        dynamic hud = assembly.CreateInstance(hudInfo.hudType);
        // 对应的Hud对象
        hud.name = hudInfo.name;
        hud.hudUnits = hudInfo.ReturnHudUnits(assembly, Content);
        hud.animationTrackController = AnimationInfo.LoadAnimationInfo(hudInfo.animationTracks);
        hud.HandleExtArgus(hudInfo.extArgus);
        hud.hudCommandController = hudInfo.ReturnCommandController();
        foreach (HudCommandInfo commandInfo in hudInfo.initialize) {
            var initCommand = commandInfo.ReturnSequence();
            initCommand.RunCommand(hud);
        }
        return hud;
    }

}

/// <summary>
/// 菜单状态信息
/// </summary>
public class HudStatus {

    /// <summary>
    /// 是否被激活
    /// </summary>
    public bool isActive;
    /// <summary>
    /// 是否可见
    /// </summary>
    public bool isVisible;

}