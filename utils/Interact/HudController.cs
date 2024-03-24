/*
HudController 顾名思义，就是管控菜单界面的对象
这里可以通过读取相关Hud的Json信息来创建菜单，还可以做到指定菜单的激活与否，调用指定的方法等
*/

using System.Collections.Generic;
using System.Reflection;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace full_leaf_framework.Interact;

/// <summary>
/// 菜单界面控制器
/// </summary>
public class HudController {

    /// <summary>
    /// 显示Hud的摄像机
    /// </summary>
    private Camera camera;

    /// <summary>
    /// 菜单界面的集合
    /// </summary>
    private Dictionary<string, Hud> huds;
    /// <summary>
    /// 菜单界面状态集合
    /// </summary>
    private Dictionary<string, HudStatus> hudStatus;

    /// <summary>
    /// 程序集详情（包含Hud对象之类的）
    /// </summary>
    private Assembly assembly;

    private ContentManager Content;

    /// <summary>
    /// 创建菜单控制器
    /// </summary>
    /// <param name="Content">资源管理器</param>
    /// <param name="camera">显示Hud的摄像机</param>
    public HudController(ContentManager Content, Camera camera) {
        assembly = Assembly.GetExecutingAssembly();
        // 获取当前程序集（为了创建指定的Hud）
        huds = new Dictionary<string, Hud>();
        hudStatus = new Dictionary<string, HudStatus>();
        // 初始化列表
        this.Content = Content;
        this.camera = camera;
    }

    /// <summary>
    /// 获取指定名称的Hud
    /// </summary>
    public Hud this[string hudName] {
        get {
            if (huds.ContainsKey(hudName) && hudStatus.ContainsKey(hudName)) {
                return huds[hudName];
            }
            return null;
        }
    }

    /// <summary>
    /// 在控制器中加入一个Hud
    /// </summary>
    /// <param name="location">读取该Hud的Json路径</param>
    /// <param name="isActive">Hud是否被激活</param>
    /// <param name="isVisible">Hud是否可见</param>
    /// <returns>返回刚刚生成的菜单对象，请自行为其填充参数</returns>
    public Hud AddHud(string location, bool isActive = true, bool isVisible = true) {
        HudStatus status = new HudStatus() {
            isActive = isActive,
            isVisible = isVisible
        };
        Hud hud = CreateHud(location, Content);
        huds.Add(hud.name, hud);
        hudStatus.Add(hud.name, status);
        return hud;
    }

    /// <summary>
    /// 在控制器中移除一个Hud
    /// </summary>
    /// <param name="name">Hud名称</param>
    public void RemoveHud(string name) {
        if (huds.ContainsKey(name)) { huds.Remove(name); }
        if (hudStatus.ContainsKey(name)) { hudStatus.Remove(name); }
    }

    public void ChangeHudStatus(string name, bool isActive, bool isVisible) {
        if (huds.ContainsKey(name) && hudStatus.ContainsKey(name)) {
            hudStatus[name].isActive = isActive;
            hudStatus[name].isVisible = isVisible;
        }
    }

    /// <summary>
    /// 获取菜单信息
    /// </summary>
    public HudStatus GetHudStatus(string name) {
        if (huds.ContainsKey(name) && hudStatus.ContainsKey(name)) {
            return hudStatus[name];
        }
        return null;
    }

    /// <summary>
    /// 按照指定的Json文件生成相应的菜单对象
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    private Hud CreateHud(string location, ContentManager Content) {
        var hudInfo = HudInfo.LoadHudInfo(location);
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
        hud.preSets = hudInfo.preSets;
        return hud;
    }

    /// <summary>
    /// 运行Hud的命令
    /// </summary>
    /// <param name="hudName">对应Hud的名称</param>
    /// <param name="commandName">命令的名称</param>
    public void RunHudCommandSequence(string hudName, string commandName) {
        if (hudStatus.ContainsKey(hudName) && huds.ContainsKey(hudName)) {
            huds[hudName].RunCommandSequence(commandName);
        }
    }

    /// <summary>
    /// 使用预设模板为菜单添加新控件
    /// </summary>
    /// <param name="hudName">对应菜单的名称</param>
    /// <param name="originName">控件本来的名字</param>
    /// <param name="changedName">更改过后的名字</param>
    /// <param name="extArgus">额外参数</param>
    public void InsertHudObject(string hudName, string originName, string changedName, object[] extArgus = null) {
        if (huds.ContainsKey(hudName)) {
            var hud = huds[hudName];
            foreach (HudUnitInfo info in hud.preSets) {
                if (info.name == originName) {
                    var newHudUnit = info.ReturnHudUnit(assembly, Content, extArgus);
                    newHudUnit.SetName(changedName);
                    hud.hudUnits.Add(newHudUnit);
                }
            }
        }
    }

    /// <summary>
    /// 从菜单中移除指定控件
    /// </summary>
    /// <param name="hudName">对应菜单的名称</param>
    /// <param name="unitName">控件的名字</param>
    public void RemoveHudObject(string hudName, string unitName) {
        if (huds.ContainsKey(hudName)) {
            var hud = huds[hudName];
            foreach (var unit in hud.hudUnits) {
                if (unit.GetName() == hudName) {
                    hud.hudUnits.Remove(unit);
                }
            }
        }
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    public void Update(GameTime gameTime) {
        foreach (string key in huds.Keys) {
            if (hudStatus.ContainsKey(key)) {
                if (hudStatus[key].isActive) {
                    huds[key].Update(gameTime);
                }
            }
        }
    }

    /// <summary>
    /// 添加控件到绘制菜单
    /// </summary>
    public void InsertDrawObjects() {
        foreach (string key in huds.Keys) {
            if (hudStatus.ContainsKey(key)) {
                if (hudStatus[key].isVisible) {
                    huds[key].Draw(camera);
                }
            }
        }
    }

    /// <summary>
    /// 绘制菜单
    /// </summary>
    public void Draw() {
        camera.Draw();
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