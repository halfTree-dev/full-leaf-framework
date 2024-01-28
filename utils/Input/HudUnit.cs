/*
HudUnit，平视信息界面中的单个控件，
可以是图像，文字，进度条等诸如此类的显示信息的对象，
由Hud储存、控制，并统一绘制。

HudUnit本身只是一个接口，真正表现功能还得是它的继承类
*/

using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace full_leaf_framework.Interact;

/// <summary>
/// 平视信息显示的控件
/// </summary>
public interface IHudUnit {

    public void Update(GameTime gameTime);
    public void Draw(Camera camera);

}

/// <summary>
/// 图像
/// </summary>
public class Image {

    /// <summary>
    /// 要显示的图像
    /// </summary>
    public Drawable drawable;
    /// <summary>
    /// 动画信息
    /// </summary>
    public AnimationTrack animationTrack;

}