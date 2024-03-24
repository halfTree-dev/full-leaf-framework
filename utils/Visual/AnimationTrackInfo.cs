/*
AnimationTrackInfo 为了读取动画信息而存在的类
*/

using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace full_leaf_framework.Visual;

/// <summary>
/// 动画Json信息
/// </summary>
public class AnimationInfo {

    /// <summary>
    /// 动画信息集合
    /// </summary>
    public AnimationTrackInfo[] trackInfos;

    /// <summary>
    /// 从指定路径读取动画信息数据并返回动画信息控制器(AnimationTrackController)
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    public static AnimationTrackController LoadAnimationInfo(string location) {
        string jsonContent = File.ReadAllText(location);
        AnimationInfo animationInfo = JsonConvert.DeserializeObject<AnimationInfo>(jsonContent);
        // 读取Json信息
        AnimationTrackController controller = new AnimationTrackController {
            tracks = new AnimationTrack[animationInfo.trackInfos.Length]
        };
        // 实例化Conroller对象
        for (int i = 0; i < animationInfo.trackInfos.Length; i++) {
            controller.tracks[i] = new AnimationTrack() {
                Name = animationInfo.trackInfos[i].name,
                animationMovement = animationInfo.trackInfos[i].
                    animationMovement.ReturnAnimationMovement(),
                animationScale = animationInfo.trackInfos[i].animationScale,
                animationSpin = animationInfo.trackInfos[i].animationSpin,
                animationTransparency = animationInfo.trackInfos[i].animationTransparency
            };
            // 填充动画信息
        }
        return controller;
    }

}

/// <summary>
/// 动画行进信息的Json信息
/// </summary>
public class AnimationTrackInfo {

    public string name;
    public AnimationMovementInfo animationMovement;
    public AnimationChange animationScale;
    public AnimationChange animationSpin;
    public AnimationChange animationTransparency;

}

/// <summary>
/// 动画位移信息
/// </summary>
public class AnimationMovementInfo {

    /// <summary>
    /// 多段使用贝塞尔曲线决定的轨迹
    /// </summary>
    public TrackCurveInfo[] tracks;
    /// <summary>
    /// 时间的映射方式，控制动画在不同时间的播放速度
    /// 例如：linear(默认),sin
    /// </summary>
    public string timeReflectMode;
    /// <summary>
    /// 动画最大进行时间
    /// </summary>
    public float maxTime;

    public AnimationMovement ReturnAnimationMovement() {
        TrackCurve[] trackCurves = new TrackCurve[tracks.Length];
        for (int i = 0; i < trackCurves.Length; i++) {
            trackCurves[i] = new TrackCurve();
            Vector2[] newControllPoints = new Vector2[tracks[i].bezierCurve.controllPoints.Length];
            for (int j = 0; j < tracks[i].bezierCurve.controllPoints.Length; j++) {
                newControllPoints[j] = tracks[i].bezierCurve.controllPoints[j].ReturnVector2();
            }
            trackCurves[i].bezierCurve = new BezierCurve() {
                controllPoints = newControllPoints
            };
            trackCurves[i].durationTime = tracks[i].durationTime;
        }
        // 根据轨迹信息创建TrackCurve对象
        return new AnimationMovement() {
            tracks = trackCurves,
            timeReflectMode = timeReflectMode,
            maxTime = maxTime
        };
    }

}

/// <summary>
/// 分段轨迹曲线
/// </summary>
public class TrackCurveInfo {

    /// <summary>
    /// 本段轨迹
    /// </summary>
    public BezierCurveInfo bezierCurve;
    /// <summary>
    /// 这段轨迹的起止时间
    /// </summary>
    public float[] durationTime;

}

/// <summary>
/// 贝塞尔曲线
/// </summary>
public class BezierCurveInfo {

    /// <summary>
    /// 贝塞尔曲线的控制点
    /// </summary>
    public VectorInfo[] controllPoints;

}

/// <summary>
/// 向量信息
/// </summary>
public class VectorInfo {

    public float X;
    public float Y;

    public Vector2 ReturnVector2() {
        return new Vector2(X, Y);
    }

}

// 另行说明：浮点数变化直接使用AnimationChange对象即可