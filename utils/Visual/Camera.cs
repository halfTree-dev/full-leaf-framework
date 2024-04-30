/*
Camera.cs
负责处理可绘制的对象并且在屏幕上绘制它们，
建议在游戏主循环中设置之，如果只需要一个摄像机，设置其为静态也行。
使用收发器在【每个】游戏循环中向Camera的绘制列表添加元素，然后它就会相应输出。
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Linq;

namespace full_leaf_framework.Visual;

/// <summary>
/// 摄像机
/// </summary>
public class Camera {

    // 游戏地图内属性
    /// <summary>
    /// 摄像机中心位置坐标
    /// </summary>
    public Vector2 pos;
    /// <summary>
    /// 摄像机的镜头长宽
    /// </summary>
    public Vector2 cameraRange;
    /// <summary>
    /// 相对于屏幕的缩放比
    /// 注：映射的部分长度 = 屏幕长度 / 缩放倍率
    /// </summary>
    public float scale = 1f;

    // 显示屏幕上属性
    /// <summary>
    /// 实际在游戏窗口上的绘制区域，
    /// 请注意：这应该和cameraRange同比例，不得非均匀放缩
    /// </summary>
    public Rectangle drawRange;

    /// <summary>
    /// 在该循环内应当被绘制的对象列表
    /// </summary>
    private List<Drawable> drawTargets;

    public delegate void InsertDrawObject(Drawable drawable);
    /// <summary>
    /// 添加绘制对象的委托
    /// </summary>
    public InsertDrawObject insertObject;

    private SpriteBatch spriteBatch;

    /// <summary>
    /// 创建一个摄像机
    /// </summary>
    /// <param name="drawRange">实际在游戏窗口上的绘制区域，
    /// 请注意：这应该和cameraRange同比例，不得非均匀放缩</param>
    /// <param name="pos">摄像机中心位置坐标</param>
    /// <param name="cameraRange">摄像机的镜头长宽</param>
    public Camera(SpriteBatch spriteBatch, Rectangle drawRange, Vector2 pos, Vector2 cameraRange) {
        this.spriteBatch = spriteBatch;
        this.drawRange = drawRange;
        this.cameraRange = cameraRange;
        this.pos = pos;
        scale = 1f;
        drawTargets = new List<Drawable>();
        insertObject = new InsertDrawObject(InsertDrawable);
    }

    /// <summary>
    /// 返回在指定绝对坐标下在屏幕上物体的绘制位置
    /// </summary>
    /// <param name="target_pos">物体的绝对位置</param>
    /// <returns>物体绘制的位置</returns>
    public Vector2 ReturnScalePos(Vector2 target_pos) {
        Vector2 swift_pos = target_pos - pos;
        // 目标坐标相对于摄像机中心的偏移
        Vector2 halfcamera = cameraRange / 2;
        halfcamera = halfcamera / scale;
        // 半屏幕的大小
        Vector2 swift_ratio = swift_pos / halfcamera;
        // 相对于半屏幕偏移大小的比值
        Vector2 screenDrawCenter = new Vector2(drawRange.X + drawRange.Width / 2,
        drawRange.Y + drawRange.Height / 2);
        // 在游戏窗口上的绘制中心
        return screenDrawCenter +
        new Vector2(drawRange.Width, drawRange.Height) * swift_ratio / 2;
        // 实际应当在屏幕上绘制的位置 = 绘制中心位置 + 偏移位置
        // 此时的放缩大小亦当为根据实际大小和cameraRange调整，再*scale，然后根据cameraRange和drawRange调整倍率
    }

    /// <summary>
    /// 返回在指定游戏窗口坐标上物体的绝对位置
    /// </summary>
    /// <param name="pointer_pos">在游戏窗口上的位置</param>
    /// <returns>物体的实际位置</returns>
    public Vector2 ReturnPointerPos(Vector2 pointer_pos) {
        Vector2 screenDrawCenter = new Vector2(drawRange.X + drawRange.Width / 2,
        drawRange.Y + drawRange.Height / 2);
        // 在游戏窗口上的绘制中心
        Vector2 swift_pos = pointer_pos - screenDrawCenter;
        // 目标相对窗口绘制中心的偏移
        Vector2 halfscreen = new Vector2(drawRange.Width, drawRange.Height) / 2;
        Vector2 swift_ratio = swift_pos / halfscreen;
        // 相对于半绘制区域偏移大小的比值
        return pos + cameraRange / 2 * swift_ratio / scale;
        // 摄像机位置加上偏移除以缩放倍率为真实坐标
    }

    /// <summary>
    /// 更新相机
    /// </summary>
    public void Update(GameTime gameTime) {
        IsRangeAvailable();
    }

    /// <summary>
    /// 检查摄像机的参数是否合法
    /// </summary>
    private void IsRangeAvailable() {
        float camera_ratio = cameraRange.X / cameraRange.Y;
        float draw_ratio = drawRange.Width / (float)drawRange.Height;
        // 屏幕的显示比例应当相等
        if (MathF.Abs(camera_ratio - draw_ratio) > 0.01f) {
            throw new Exception("摄像机的镜头长宽比应当和绘制区域的长宽比等同");
        }
    }

    /// <summary>
    /// 获取从cameraRange转为drawRange的倍率
    /// </summary>
    private float GetRangeRatio() {
        IsRangeAvailable();
        return drawRange.Width / cameraRange.X;
    }

    /// <summary>
    /// 绘制所有对象
    /// </summary>
    public void Draw() {
        DrawList();
    }

    /// <summary>
    /// 添加一个Drawable对象到绘制列表
    /// </summary>
    private void InsertDrawable(Drawable drawable) {
        drawTargets.Add(drawable);
    }

    private void DrawList() {
        // 将绘制物体按照y值和优先级排序
        drawTargets.OrderBy(i => i.layer).ThenBy(i => i.pos.Y);
        // 然后再输出drawObject对象
        foreach (Drawable obj in drawTargets)
        {
            try {
                DrawSprite(obj);
            }
            catch {}
        }
        drawTargets.Clear();
    }

    /// <summary>
    /// 绘制一个Drawable对象
    /// 此时的放缩大小亦当为根据实际大小和cameraRange调整，再*scale，然后根据cameraRange和drawRange调整倍率
    /// </summary>
    private void DrawSprite(Drawable obj) {
        float mixture_scale = 1f;
        mixture_scale *= obj.sizeScale;
        // 实际大小的倍率
        float changed_scale = mixture_scale * scale;
        changed_scale *= GetRangeRatio();
        // 屏幕缩放的倍率
        Vector2 draw_pos = ReturnScalePos(obj.pos);
        // 当强制绘制时更改绘制帧
        int currentFrame = obj.currentAnimation.CurrentFrame;
        if (obj.settledFrame != -1) {
            obj.currentAnimation.CurrentFrame = obj.settledFrame;
        }
        // 绘制的坐标（注：提前处理锚点，根据锚点更改绘制位置）
        obj.currentAnimation.Draw(spriteBatch, draw_pos, obj.anchorPoint,
        obj.effects, obj.angle, changed_scale, obj.drawArea, obj.transparency);
        obj.currentAnimation.CurrentFrame = currentFrame;
    }

}