/*
Hud，即平视信息显示，可以在游戏进行过程中显示游戏进行时的信息
从Json中读取一系列有关于一个游戏菜单的信息，然后显示出来，这就是Hud的功能~

实际使用的时候，必须将Hud【继承】出来，因为你自己要写方法对吧。
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using static full_leaf_framework.Interact.Hud;

namespace full_leaf_framework.Interact;

/// <summary>
/// 平视信息显示
/// </summary>
public class Hud {

    /// <summary>
    /// 菜单的名字
    /// </summary>
    public string name;

    /// <summary>
    /// 控件列表
    /// </summary>
    public List<IHudUnit> hudUnits;
    // #warning 应当允许在过程中自己添加菜单项目
    /// <summary>
    /// 动画轨迹控制器
    /// </summary>
    public AnimationTrackController animationTrackController;
    /// <summary>
    /// 菜单命令控制器
    /// </summary>
    public HudCommandController hudCommandController;
    /// <summary>
    /// 控件预设模板
    /// </summary>
    public HudUnitInfo[] preSets;

    /// <summary>
    /// 菜单事件的委托
    /// </summary>
    public delegate void MenuHandleEvent(IHudUnit unit);

    /*
    特别注意：
    不要为Hud对象编写有参数的构造函数！
    Hud的创建使用CreateInstance()方法，默认不传入参数，
    有参数的构造函数的菜单会因为找不到相应的构造函数而没法创建。
    */


    /// <summary>
    /// 提供额外参数后的处理
    /// </summary>
    public virtual void HandleExtArgus(object[] extArgus) {
    }

    /// <summary>
    /// 运行命令
    /// </summary>
    internal void RunCommandSequence(string commandName) {
        try {
            hudCommandController.RunCommandSequence(commandName, this);
        }
        catch (Exception e) { Console.WriteLine(e); }
    }

    /// <summary>
    /// 传入指定参数并执行命令
    /// </summary>
    public void RunCommandSequence(string type, string target, string content, string handler) {
        try {
            hudCommandController.RunCommandSequence(type, target, content, handler, this);
        }
        catch (Exception e) { Console.WriteLine(e); }
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    internal void Update(GameTime gameTime) {
        foreach (IHudUnit hudUnit in hudUnits) {
            hudUnit.Update(gameTime);
        }
    }

    /// <summary>
    /// 绘制菜单
    /// </summary>
    internal void Draw(Camera camera) {
        foreach (IHudUnit hudUnit in hudUnits) {
            hudUnit.Draw(camera);
        }
    }

}

/// <summary>
/// 菜单命令控制器
/// </summary>
public class HudCommandController {

    /// <summary>
    /// 命令列表
    /// </summary>
    private Dictionary<string, HudCommandSequence[]> commands;

    public HudCommandController() {
        commands = new Dictionary<string, HudCommandSequence[]>();
    }

    /// <summary>
    /// 添加菜单命令
    /// </summary>
    /// <param name="name">命令名称</param>
    /// <param name="sequences">命令行</param>
    internal void AddCommandSequence(string name, HudCommandSequence[] sequences) {
        commands.Add(name, sequences);
    }

    /// <summary>
    /// 移除菜单命令
    /// </summary>
    /// <param name="name">命令名称</param>
    internal void RemoveCommandSequence(string name) {
        commands.Remove(name);
    }

    /// <summary>
    /// 运行菜单命令
    /// </summary>
    /// <param name="name">命令名称</param>
    internal void RunCommandSequence(string name, Hud hud) {
        var sequences = commands[name];
        foreach (HudCommandSequence sequence in sequences) {
            sequence.RunCommand(hud);
        }
    }

    internal void RunCommandSequence(string type, string target, string content, string handler, Hud hud) {
        HudCommandSequence sequence = new HudCommandSequence() {
            type = type,
            content = content,
            handler = handler,
            target = target
        };
        sequence.RunCommand(hud);
    }

}

/// <summary>
/// 菜单命令行
/// </summary>
public class HudCommandSequence {

    /// <summary>
    /// 命令类别
    /// 目前的命令只有add_event, add_animation, rmv_event, rmv_animation
    /// </summary>
    public string type;
    /// <summary>
    /// 作用对象
    /// </summary>
    public string target;
    /// <summary>
    /// 作用内容
    /// </summary>
    public string content;
    /// <summary>
    /// 作用对象的处理方式
    /// </summary>
    public string handler;

    /// <summary>
    /// 运行这个菜单命令
    /// </summary>
    /// <param name="hud">菜单对象</param>
    public void RunCommand(Hud hud) {
        MethodInfo method = hud.GetType().GetMethod(content);
        // 反射指定的方法
        IHudUnit targetUnit = null;
        foreach (IHudUnit hudUnit in hud.hudUnits) {
            if (target == hudUnit.GetName()) {
                targetUnit = hudUnit;
            }
        }
        // 获取对应的控件
        if (targetUnit == null) { return; }
        switch (type) {
            case "add_event":
                var handleEvent = method.CreateDelegate(typeof(MenuHandleEvent), hud);
                targetUnit.SetEventToHandler((MenuHandleEvent)handleEvent, handler);
                break;
            case "rmv_event":
                var delEvent = method.CreateDelegate(typeof(MenuHandleEvent), hud);
                targetUnit.RemoveEventFromHandler((MenuHandleEvent)delEvent, handler);
                break;
            case "add_animation":
                var animationTrack = hud.animationTrackController.ReturnAnimationTrack(content);
                targetUnit.SetAnimationTrack(animationTrack);
                targetUnit.SetAnimationTime(0);
                break;
            case "rmv_animation":
                targetUnit.SetAnimationTrack(null);
                targetUnit.SetAnimationTime(0);
                break;
        }
        // 执行相关的操作
    }

}