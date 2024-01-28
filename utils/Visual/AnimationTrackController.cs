/*
AnimationTrackController 顾名思义，是动画控制器
每个动画轨迹和它们的名称构成键值对，输入动画名称，返回对应的AnimationTrack，
这就是它的功能，是一个集中动画对象的对象~
*/

namespace full_leaf_framework.Visual;

/// <summary>
/// 动画轨迹控制器
/// </summary>
public class AnimationTrackController {

    /// <summary>
    /// 轨迹的列表
    /// </summary>
    public AnimationTrack[] tracks;

    /// <summary>
    /// 返回指定名称的动画路径
    /// </summary>
    /// <param name="name">动画名称</param>
    public AnimationTrack ReturnAnimationTrack(string name) {
        foreach (AnimationTrack track in tracks) {
            if (track.Name == name) {
                return track;
            }
        }
        return null;
    }

}