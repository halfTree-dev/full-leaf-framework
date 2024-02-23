/*
HudInfo 就是为了读取Hud信息而创建的类
*/

using System.IO;
using System.Reflection;
using full_leaf_framework.Physics;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace full_leaf_framework.Interact;

/// <summary>
/// Hud的Json信息
/// </summary>
public class HudInfo {

    /// <summary>
    /// 菜单的名字
    /// </summary>
    public string name;
    /// <summary>
    /// 菜单类的完全限定名
    /// </summary>
    public string hudType;
    /// <summary>
    /// 菜单的控件列表
    /// </summary>
    public HudUnitInfo[] hudUnits;
    /// <summary>
    /// 动画轨迹列表
    /// </summary>
    public string animationTracks;
    /// <summary>
    /// 菜单初始化时执行的命令
    /// </summary>
    public HudCommandInfo[] initialize;
    /// <summary>
    /// 菜单命令
    /// </summary>
    public HudCommandSequenceInfo[] commands;
    /// <summary>
    /// 额外参数
    /// </summary>
    public object[] extArgus;

    /// <summary>
    /// 创建一个HudInfo对象并且从指定路径读取数据
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    public static HudInfo LoadHudInfo(string location) {
        string jsonContent = File.ReadAllText(location);
        HudInfo hudInfo = JsonConvert.DeserializeObject<HudInfo>(jsonContent);
        return hudInfo;
    }

    public IHudUnit[] ReturnHudUnits(Assembly assembly, ContentManager Content) {
        IHudUnit[] result = new IHudUnit[hudUnits.Length];
        for (int i = 0; i < result.Length; i++) {
            result[i] = hudUnits[i].ReturnHudUnit(assembly, Content);
        }
        return result;
    }

    public HudCommandController ReturnCommandController() {
        HudCommandController result = new HudCommandController();
        foreach (HudCommandSequenceInfo command in commands) {
            result.AddCommandSequence(command.name, command.ReturnCommandSequence());
        }
        return result;
    }

}

/// <summary>
/// 菜单控件信息
/// </summary>
public class HudUnitInfo {

    /// </summary>
    /// 名字
    /// </summary>
    public string name;
    /// <summary>
    /// 控件类的完全限定名
    /// </summary>
    public string hudClass;
    /// <summary>
    /// 绘制对象的信息
    /// </summary>
    public HudDrawableInfo drawableInfo;
    /// <summary>
    /// 碰撞箱
    /// </summary>
    public VectorInfo[] collisionBox;
    /// <summary>
    /// 额外参数
    /// </summary>
    public object[] extArgus;

    public IHudUnit ReturnHudUnit(Assembly assembly, ContentManager Content) {
        dynamic hudUnit = assembly.CreateInstance(hudClass);
        hudUnit.SetName(name);
        Drawable hudDrawable = new Drawable(drawableInfo.spriteInfo.ReturnAnimation(Content),
        new Vector2(drawableInfo.posX, drawableInfo.posY), new Vector2(drawableInfo.anchorPointX, drawableInfo.anchorPointY),
        drawableInfo.sizeScale, drawableInfo.angle, SpriteEffects.None, drawableInfo.layer);
        hudUnit.SetCollsionBox(ReturnCollisionBox());
        hudUnit.SetDrawObject(hudDrawable);
        hudUnit.HandleExtArgus(extArgus);
        return hudUnit;
    }

    private Polygon ReturnCollisionBox() {
        var points = new Vector2[collisionBox.Length];
        for (int i = 0; i < points.Length; i++) {
            points[i] = collisionBox[i].ReturnVector2();
        }
        Polygon result = new Polygon(points);
        return result;
    }

}

/// <summary>
/// 建筑物信息
/// </summary>
public class HudDrawableInfo {

    /// <summary>
    /// 动画信息
    /// </summary>
    public AnimationSpriteInfo spriteInfo;
    /// <summary>
    /// 位置的X值
    /// </summary>
    public float posX;
    /// <summary>
    /// 位置的Y值
    /// </summary>
    public float posY;
    /// <summary>
    /// 锚点的X值
    /// </summary>
    public float anchorPointX;
    /// <summary>
    /// 锚点的Y值
    /// </summary>
    public float anchorPointY;
    /// <summary>
    /// 放缩大小比值
    /// </summary>
    public float sizeScale;
    /// <summary>
    /// 旋转角度
    /// </summary>
    public float angle;
    /// <summary>
    /// 绘制图层
    /// </summary>
    public int layer;

}

/// <summary>
/// 菜单命令信息
/// </summary>
public class HudCommandInfo {

    /// <summary>
    /// 命令类别
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
    /// 处理者
    /// </summary>
    public string handler;

    public HudCommandSequence ReturnSequence() {
        HudCommandSequence result = new HudCommandSequence() {
            type = type,
            target = target,
            content = content,
            handler = handler
        };
        return result;
    }

}

/// <summary>
/// 菜单命令块信息
/// </summary>
public class HudCommandSequenceInfo {

    /// <summary>
    /// 名字
    /// </summary>
    public string name;
    /// <summary>
    /// 命令块
    /// </summary>
    public HudCommandInfo[] sequence;

    public HudCommandSequence[] ReturnCommandSequence() {
        HudCommandSequence[] result = new HudCommandSequence[sequence.Length];
        for (int i = 0; i < result.Length; i++) {
            result[i] = sequence[i].ReturnSequence();
        }
        return result;
    }

}