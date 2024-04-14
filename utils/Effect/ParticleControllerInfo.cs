/*
为了从一个包含有粒子效果信息的Json文件中读取相关信息，我写了这个类。
*/

using System.IO;
using Newtonsoft.Json;

namespace full_leaf_framework.Effect;

/// <summary>
/// 粒子控制器的信息
/// </summary>
public class ParticleControllerInfo {

    /// <summary>
    /// 精灵图集信息
    /// </summary>
    public Scene.SpriteInfo[] spriteInfos;
    /// <summary>
    /// 粒子信息的集合
    /// </summary>
    public ParticleInfo[] particleInfos;
    /// <summary>
    /// 粒子生成事件信息的集合
    /// </summary>
    public BurstInfo[] burstInfos;

    /// <summary>
    /// 创建一个ParticleControllerInfo对象并且从指定路径读取数据
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    public static ParticleControllerInfo LoadParticleControllerInfo(string location) {
        string jsonContent = File.ReadAllText(location);
        var particleControllerInfo = JsonConvert.DeserializeObject<ParticleControllerInfo>(jsonContent);
        return particleControllerInfo;
    }

}

/// <summary>
/// 粒子信息
/// </summary>
public class ParticleInfo {

    /// <summary>
    /// 要对应创建的粒子的类名
    /// </summary>
    public string particleClass;
    /// <summary>
    /// 粒子名字
    /// </summary>
    public string particleName;
    /// <summary>
    /// 使用的图集名称
    /// </summary>
    public string usedSprite;
    /// <summary>
    /// 帧数左边界
    /// </summary>
    public int usedFrameL;
    /// <summary>
    /// 帧数右边界
    /// </summary>
    public int usedFrameR;
    /// <summary>
    /// 帧间隔
    /// </summary>
    public float frameDelay;
    /// <summary>
    /// 起始帧
    /// </summary>
    public int startFrame;
    /// <summary>
    /// 锚点坐标X
    /// </summary>
    public int anchorPointX;
    /// <summary>
    /// 锚点坐标Y
    /// </summary>
    public int anchorPointY;
    /// <summary>
    /// 起始缩放值
    /// </summary>
    public float sizeScale;
    /// <summary>
    /// 旋转角度
    /// </summary>
    public int angle;
    /// <summary>
    /// 所在图层
    /// </summary>
    public int layer;
    /// <summary>
    /// 额外参数（将会被传入类中）
    /// </summary>
    public string[] extArugs;

}

/// <summary>
/// 粒子生成事件的信息
/// </summary>
public class BurstInfo {

    /// <summary>
    /// 生成事件的名称
    /// </summary>
    public string burstName;
    /// <summary>
    /// 生成粒子的名称
    /// </summary>
    public string burstType;
    /// <summary>
    /// 生成粒子的数目
    /// </summary>
    public int particleCount;
    /// <summary>
    /// 速度取值范围
    /// </summary>
    public float[] velocityRange;
    /// <summary>
    /// 角度取值范围
    /// </summary>
    public float[] angleRange;
    /// <summary>
    /// 生命周期取值范围
    /// </summary>
    public float[] lifeTimeRange;

}