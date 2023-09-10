/*
DebugManager（调试管理器）类被置放于游戏主循环中，
通过为目标打上相关的特性标签，调试器会予行处理，将其的值显示在游戏中，或生成日志。
值需要在每个循环都被添加才会生效

建议在游戏主循环中设置该控制器，然后设置其为静态
*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace full_leaf_framework.Debug;

/// <summary>
/// 调试管理器
/// </summary>
public class DebugManager {

    /// <summary>
    /// 调试信息列表
    /// </summary>
    private List<DebugInfo> debugInfos;

    /// <summary>
    /// 绘制字体
    /// </summary>
    private SpriteFont spriteFont;
    /// <summary>
    /// 绘制颜色
    /// </summary>
    private Color color = Color.White;

    /// <summary>
    /// 创建调试管理器
    /// </summary>
    public DebugManager(SpriteFont spriteFont) {
        debugInfos = new List<DebugInfo>();
        // 在外部读取spriteFont并且导入
        this.spriteFont = spriteFont;
    }

    /// <summary>
    /// 添加追踪的浮点数
    /// </summary>
    public void AddTrackedFloat(string label, float content) {
        debugInfos.Add(new DebugInfo(label, content.ToString()));
    }

    /// <summary>
    /// 添加追踪的字符串
    /// </summary>
    public void AddTrackedString(string label, string content) {
        debugInfos.Add(new DebugInfo(label, content));
    }

    /// <summary>
    /// 切换调试字体的颜色
    /// </summary>
    public void ChangeColor(Color new_color) {
        color = new_color;
    }

    /// <summary>
    /// 绘制调试信息
    /// </summary>
    public void DrawInfos(SpriteBatch spriteBatch) {
        // 将debugInfos的所有列表绘制
        // 然后清空列表
        int draw_y = 0;
        foreach (DebugInfo debugInfo in debugInfos) {
            spriteBatch.DrawString(spriteFont,
            debugInfo.ReturnInfoString(), new Vector2(0, draw_y), color);
            draw_y += spriteFont.LineSpacing;
        }
        debugInfos.Clear();
    }

}

/// <summary>
/// 调试信息
/// </summary>
public class DebugInfo {

    /// <summary>
    /// 标签
    /// </summary>
    public string label;
    /// <summary>
    /// 信息
    /// </summary>
    public string content;

    /// <summary>
    /// 创建调试信息
    /// </summary>
    public DebugInfo(string label, string content) {
        this.label = label;
        this.content = content;
    }

    /// <summary>
    /// 返回调试信息字符串
    /// </summary>
    public string ReturnInfoString() {
        return label + " : " + content;
    }

}