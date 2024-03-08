/*
AnimationTrack 动画进行信息
描述一段动画中，目标的行进位置，旋转角度，缩放大小的变化信息
同时也可以输入动画时间，获取指定时间时动画的关键信息
*/

using System;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Visual;

/// <summary>
/// 动画行进信息
/// </summary>
public class AnimationTrack {

    /// <summary>
    /// 动画位移信息
    /// </summary>
    internal AnimationMovement animationMovement;
    /// <summary>
    /// 动画缩放信息
    /// </summary>
    internal AnimationChange animationScale;
    /// <summary>
    /// 动画旋转信息
    /// </summary>
    internal AnimationChange animationSpin;
    /// <summary>
    /// 动画透明度信息
    /// </summary>
    internal AnimationChange animationTransparency;

    /// <summary>
    /// 动画信息的名字
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 返回当前动画的状态
    /// </summary>
    /// <param name="time">动画时间</param>
    public AnimationResult ReturnAnimationResult(float time) {
        return new AnimationResult() {
            pos = animationMovement.ReturnTrackPoint(time),
            scale = animationScale.ReturnCurrentValue(time),
            angle = animationSpin.ReturnCurrentValue(time),
            transparency = animationTransparency.ReturnCurrentValue(time)
        };
    }

    /// <summary>
    /// 用当前的动画参数修改Drawable类型，使其呈现动画效果
    /// </summary>
    /// <param name="drawable">要被赋予动画效果的对象</param>
    public void AffectDrawable(Drawable drawable, float time) {
        AnimationResult result = ReturnAnimationResult(time);
        drawable.pos = result.pos;
        drawable.angle = result.angle;
        drawable.sizeScale = result.scale;
        drawable.transparency = result.transparency;
    }

}

/// <summary>
/// 动画当前状态
/// </summary>
public struct AnimationResult {
    /// <summary>
    /// 动画对象位置
    /// </summary>
    public Vector2 pos;
    /// <summary>
    /// 动画对象旋转角
    /// </summary>
    public float angle;
    /// <summary>
    /// 动画对象缩放值
    /// </summary>
    public float scale;
    /// <summary>
    /// 动画对象透明度
    /// </summary>
    public float transparency;

}

#region 动画位移
/// <summary>
/// 动画位移信息
/// </summary>
public class AnimationMovement {

    /// <summary>
    /// 多段使用贝塞尔曲线决定的轨迹
    /// </summary>
    public TrackCurve[] tracks;
    /// <summary>
    /// 时间的映射方式，控制动画在不同时间的播放速度
    /// 例如：linear(默认),sin
    /// </summary>
    public string timeReflectMode;
    /// <summary>
    /// 动画最大进行时间
    /// </summary>
    public float maxTime;

    /// <summary>
    /// 返回轨迹上指定时间时的位置
    /// </summary>
    /// <param name="time">动画时间</param>
    public Vector2 ReturnTrackPoint(float time) {
        time = (time > maxTime) ? maxTime : time; time = (time < 0) ? 0 : time;
        Vector2 result = new Vector2(0, 0);
        foreach (TrackCurve track in tracks) {
            if (time >= track.durationTime[0] && time <= track.durationTime[1]) {
                // 计算时间映射关系
                if (timeReflectMode == "sin") {
                    time = track.durationTime[0] + MathF.Sin((time - track.durationTime[0]) /
                    (track.durationTime[1] - track.durationTime[0]) * MathF.PI / 2) * (track.durationTime[1] - track.durationTime[0]);
                    // 在本段持续时间之内时，返回本段的轨迹
                }
                if (timeReflectMode == "rsin") {
                    time = track.durationTime[0] + (1 - MathF.Cos((time - track.durationTime[0]) /
                    (track.durationTime[1] - track.durationTime[0]) * MathF.PI / 2)) * (track.durationTime[1] - track.durationTime[0]);
                }
                if (timeReflectMode == "smooth") {
                    float dTime = (time - track.durationTime[0]) / (track.durationTime[1] - track.durationTime[0]);
                    float actualTime = MathF.Pow(dTime, 5) * 6 - MathF.Pow(dTime, 4) * 15 + MathF.Pow(dTime, 3) * 10;
                    time = track.durationTime[0] + actualTime * (track.durationTime[1] - track.durationTime[0]);
                }
                result = track.bezierCurve.ReturnTrackPoint((time - track.durationTime[0])
                / (track.durationTime[1] - track.durationTime[0]));
                break;
            }
        }
        // 计算轨迹
        return result;
    }

}

/// <summary>
/// 分段轨迹曲线
/// </summary>
public class TrackCurve {

    /// <summary>
    /// 本段轨迹
    /// </summary>
    public BezierCurve bezierCurve;
    /// <summary>
    /// 这段轨迹的起止时间
    /// </summary>
    public float[] durationTime;

}

/// <summary>
/// 贝塞尔曲线
/// </summary>
public class BezierCurve {

    /// <summary>
    /// 贝塞尔曲线的控制点
    /// </summary>
    public Vector2[] controllPoints;

    /// <summary>
    /// 返回指定时间时的轨迹点位置
    /// </summary>
    /// <param name="time">[0, 1]之间的相对时间值</param>
    public Vector2 ReturnTrackPoint(float time) {
        return ReturnLinearValue(controllPoints, time);
    }

    /// <summary>
    /// 返回指定时间时的所有控制点的线性插值
    /// </summary>
    internal Vector2 ReturnLinearValue(Vector2[] controllPoints, float time) {
        if (controllPoints.Length == 2) {
            return controllPoints[0] * (1 - time) + controllPoints[1] * time;
            // 当仅有两个数据点，直接返回它们的线性插值
        }
        else if (controllPoints.Length > 2) {
            Vector2[] newControllPoints = new Vector2[controllPoints.Length - 1];
            for (int i = 0; i < controllPoints.Length - 1; i++) {
                // 多于两个数据点，对它们按照顺序一一做线性插值
                Vector2[] pointGroup = new Vector2[2] { controllPoints[i], controllPoints[i + 1] };
                newControllPoints[i] = ReturnLinearValue(pointGroup, time);
            }
            // 对这个已经做完一次插值，阶数减少了1的曲线再求值
            return ReturnLinearValue(newControllPoints, time);
        }
        else {
            throw new Exception("贝塞尔曲线至少应该含有两个数据点");
        }
    }

}
#endregion

#region 浮点数值变化
/// <summary>
/// 动画缩放信息
/// </summary>
public class AnimationChange {

    /// <summary>
    /// 缩放行为列表
    /// </summary>
    public ChangeAction[] changeActions;
    /// <summary>
    /// 时间的映射方式，控制动画在不同时间的播放速度
    /// 例如：linear(默认),sin,rsin(倒转sin，先慢后快),smooth(中间快，两端慢)
    /// </summary>
    public string timeReflectMode;
    /// <summary>
    /// 动画最大进行时间
    /// </summary>
    public float maxTime;

    /// <summary>
    /// 返回指定时间时的数据值
    /// </summary>
    /// <param name="time">动画时间</param>
    public float ReturnCurrentValue(float time) {
        time = (time > maxTime) ? maxTime : time; time = (time < 0) ? 0 : time;
        float result = 1f;
        foreach (ChangeAction changeAction in changeActions) {
            if (time >= changeAction.durationTime[0] && time <= changeAction.durationTime[1]) {
                // 计算时间映射关系
                if (timeReflectMode == "sin") {
                    time = changeAction.durationTime[0] + MathF.Sin((time - changeAction.durationTime[0]) /
                    (changeAction.durationTime[1] - changeAction.durationTime[0]) * MathF.PI / 2) * (changeAction.durationTime[1] - changeAction.durationTime[0]);
                }
                if (timeReflectMode == "rsin") {
                    time = changeAction.durationTime[0] + (1 - MathF.Cos((time - changeAction.durationTime[0]) /
                    (changeAction.durationTime[1] - changeAction.durationTime[0]) * MathF.PI / 2)) * (changeAction.durationTime[1] - changeAction.durationTime[0]);
                }
                if (timeReflectMode == "smooth") {
                    float dTime = (time - changeAction.durationTime[0]) / (changeAction.durationTime[1] - changeAction.durationTime[0]);
                    float actualTime = MathF.Pow(dTime, 5) * 6 - MathF.Pow(dTime, 4) * 15 + MathF.Pow(dTime, 3) * 10;
                    time = changeAction.durationTime[0] + actualTime * (changeAction.durationTime[1] - changeAction.durationTime[0]);
                }
                // 在本段持续时间之内时，返回本段的缩放变化值
                result = changeAction.changeMotion[0] + (time - changeAction.durationTime[0]) /
                    (changeAction.durationTime[1] - changeAction.durationTime[0]) * (changeAction.changeMotion[1] - changeAction.changeMotion[0]);
                break;
            }
        }
        // 计算缩放值
        return result;
    }

}

/// <summary>
/// 单个变化行为
/// </summary>
public class ChangeAction {

    /// <summary>
    /// 变化的起始终止值
    /// </summary>
    public float[] changeMotion;
    /// <summary>
    /// 起止时间
    /// </summary>
    public float[] durationTime;

}
#endregion