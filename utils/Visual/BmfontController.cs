/*
BmfontString
有很多游戏都会使用BMFont来管理、输出文字，
这个BmfontController会记录一个BMFont字体，并且利用字体的信息和输入的文字
生成一个BmfontString，内含包含文字图片的Drawable对象数组，将它们绘制，可以达到输出文字的目的。
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace full_leaf_framework.Visual;

/// <summary>
/// Bmfont控制器
/// </summary>
public class BmfontController {

    /// <summary>
    /// 存放字体文件的根目录位置（不包含最初的Content文件夹）
    /// </summary>
    private string root;

    /// <summary>
    /// Bmfont字体的名称
    /// </summary>
    private string name;
    public string Name { get => name; }

    /// <summary>
    /// 该字体文件的信息
    /// </summary>
    private BmfontInfo fontInfo;

    /// <summary>
    /// id和它所对应的纹理的映射关系
    /// </summary>
    private Dictionary<int, AnimatedSprite> pagesDic;
    /// <summary>
    /// id和它所对应字符的信息
    /// </summary>
    private Dictionary<int, FontChar> charsDic;
    /// <summary>
    /// 两个字符id和他们对应的字偶矩关系
    /// </summary>
    private Dictionary<Point, int> kerningsDic;

    private ContentManager Content;

    /// <summary>
    /// 构建一个BmFont管理器
    /// </summary>
    /// <param name="root">存放字体文件的根目录位置（不包含最初的Content文件夹）</param>
    /// <param name="fontName">.fnt字体文件的名称，不带后缀</param>
    public BmfontController(string root, string fontName, ContentManager Content) {
        // 纹理应当和.fnt文件在同一个位置
        this.root = root;
        name = fontName;
        this.Content = Content;
        try { InitializeFontInfo(root, fontName); }
        catch { throw new System.Exception(@"没有找到目标字体文件：Content\" + root + @"\" + fontName + ".fnt"); }
        InitializeDictionary();
    }

    private void InitializeFontInfo(string root, string fontName) {
        XmlSerializer serializer = new XmlSerializer(typeof(BmfontInfo));
        FileStream stream = new FileStream(@"Content\" + root + @"\" + fontName + ".fnt", FileMode.Open);
        fontInfo = (BmfontInfo)serializer.Deserialize(stream);
    }

    private void InitializeDictionary() {
        // 先填充pages
        pagesDic = new Dictionary<int, AnimatedSprite>();
        foreach (FontPage page in fontInfo.Pages) {
            // 这里，由于page.File携带.png的后缀名，故仅仅截取第一个'.'前面的字符串，请注意文件名本身不带'.'
            Texture2D texture = Content.Load<Texture2D>(root + @"\" + page.File.Split('.')[0]);
            var animation = new AnimatedSprite(texture);
            pagesDic.Add(page.Id, animation);
        }
        // 再填充chars
        charsDic = new Dictionary<int, FontChar>();
        foreach (FontChar charInfo in fontInfo.Chars) {
            charsDic.Add(charInfo.Id, charInfo);
        }
        // 再填充kerningsDic（用Point(x,y)表示映射关系）
        kerningsDic = new Dictionary<Point, int>();
        if (fontInfo.Kernings != null)
            foreach (FontKerning kerning in fontInfo.Kernings) {
                var charKerning = new Point(kerning.First, kerning.Second);
                kerningsDic.Add(charKerning, kerning.Amount);
            }
    }

    /// <summary>
    /// 获得由指定字符串转化而成的Drawable集合
    /// </summary>
    private BmfontDrawable GetDrawableString(string drawString, float sizeScale, bool noOffSet = true,
    SpriteEffects effects = SpriteEffects.None, int layer = 10, float transparency = 1) {
        List<Drawable> drawChars = new List<Drawable>();
        int xSwift = 0;
        char[] charArray = drawString.ToCharArray();
        for (int i = 0; i < charArray.Length; i++) {
            char eachChar = charArray[i];
            int charAscii = eachChar; // 将每个字符转化为其对应ASCII
            AnimatedSprite sprite = pagesDic[charsDic[charAscii].Page];
            FontChar fontChar = charsDic[charAscii];
            var drawable = new Drawable(sprite, new Vector2(xSwift, 0),
            new Vector2(fontChar.Width / 2, fontChar.Height / 2), sizeScale,
            drawArea : new Rectangle(fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height),
            effects : effects, layer : layer, transparency : transparency);
            if (!noOffSet) { drawable.pos += new Vector2(fontChar.XOffset, fontChar.YOffset); }
            drawChars.Add(drawable);
            // 生成Drawable对象，并指定绘制区域
            if (i < charArray.Length - 1) {
                Point kerning = new Point(charArray[i], charArray[i+1]);
                if (kerningsDic.ContainsKey(kerning)) {
                    xSwift += (int)(kerningsDic[kerning] * sizeScale);
                }
                else {
                    xSwift += (int)(fontChar.XAdvance * sizeScale);
                }
            }
            else {
                xSwift += (int)(fontChar.XAdvance * sizeScale);
            }
            // 推进到下一个绘制位点，如果有字偶矩则使用字偶矩
        }
        var drawables = new Drawable[drawChars.Count];
        drawChars.CopyTo(drawables);
        // 创建BmfontDrawable对象
        int length = xSwift;
        int lineSpace = (int)(fontInfo.Common.LineHeight * sizeScale);
        var result = new BmfontDrawable(drawString, drawables, length, lineSpace);
        return result;
    }

    /// <summary>
    /// 将字符串输出到摄像机上
    /// </summary>
    /// <param name="camera">摄像机</param>
    /// <param name="drawString">字符串</param>
    /// <param name="pos">位置</param>
    /// <param name="method">对齐方式</param>
    /// <param name="noOffSet">是否使用字符的offset（偏移量）</param>
    /// <param name="sizeScale">缩放大小</param>
    /// <param name="effects">绘制效果</param>
    /// <param name="layer">绘制的优先级，越小越高，越容易绘制在底层</param>
    /// <param name="transparency">透明度（0为完全透明，1为完全不透明）</param>
    public void InsertDrawObjects(Camera camera, string drawString,
    Vector2 pos, BmfontDrawable.TranslateMethod method, float sizeScale = 1, bool noOffSet = true,
    SpriteEffects effects = SpriteEffects.None, int layer = 10, float transparency = 1) {
        try { var drawableString = GetDrawableString(drawString, sizeScale, noOffSet, effects, layer, transparency);
            drawableString.InsertDrawObjects(camera, pos, method);
        }
        catch (Exception e) { throw new Exception("出现异常字符：" + e); }
    }

    /// <summary>
    /// 将多行字符串输出到摄像机上
    /// </summary>
    /// <param name="camera">摄像机</param>
    /// <param name="drawString">字符串（每个元素占据一行）</param>
    /// <param name="pos">位置</param>
    /// <param name="method">对齐方式</param>
    /// <param name="noOffSet">是否使用字符的offset（偏移量）</param>
    /// <param name="sizeScale">缩放大小</param>
    /// <param name="effects">绘制效果</param>
    /// <param name="layer">绘制的优先级，越小越高，越容易绘制在底层</param>
    /// <param name="transparency">透明度（0为完全透明，1为完全不透明）</param>
    public void InsertDrawObjects(Camera camera, string[] drawString,
    Vector2 pos, BmfontDrawable.TranslateMethod method, float sizeScale = 1, bool noOffSet = true,
    SpriteEffects effects = SpriteEffects.None, int layer = 10, float transparency = 1) {
        foreach (string singleString in drawString) {
            try {
                var drawableString = GetDrawableString(singleString, sizeScale, noOffSet, effects, layer, transparency);
                drawableString.InsertDrawObjects(camera, pos, method);
                pos.Y += drawableString.LineSpace;
            }
            catch (Exception e) { throw new Exception("出现异常字符：" + e); }
        }
    }

}

/// <summary>
/// 由Bmfont将即将输出的字符转变的Drawable对象集合
/// </summary>
public class BmfontDrawable {

    /// <summary>
    /// 要绘制的字符串
    /// </summary>
    public string drawString;
    /// <summary>
    /// 可绘制对象集合
    /// </summary>
    public Drawable[] drawables;

    private int length;
    /// <summary>
    /// 所有可绘制对象长度之和
    /// </summary>
    public int Length { get => length; }

    private int lineSpace;
    /// <summary>
    /// 行间距
    /// </summary>
    public int LineSpace { get => lineSpace; }

    /// <summary>
    /// 对齐方式
    /// </summary>
    public enum TranslateMethod {
        Left = 0,
        Middle = 1,
        Right = 2
    }

    internal void SetLength(int length) {
        this.length = length;
    }

    internal void SetLineSpace(int lineSpace) {
        this.lineSpace = lineSpace;
    }

    public BmfontDrawable(string drawString, Drawable[] drawables, int length, int lineSpace) {
        this.drawables = drawables;
        this.drawString = drawString;
        this.length = length;
        this.lineSpace = lineSpace;
    }

    /// <summary>
    /// 将可绘制对象上传到摄像机
    /// </summary>
    internal void InsertDrawObjects(Camera camera, Vector2 pos, TranslateMethod method) {
        switch (method) {
            case TranslateMethod.Left:
                pos += new Vector2(0, -lineSpace / 2);
                break;
            case TranslateMethod.Middle:
                pos += new Vector2(-length / 2, -lineSpace / 2);
                break;
            case TranslateMethod.Right:
                pos += new Vector2(-length, lineSpace / 2);
                break;
        }
        // 根据对齐方式调整坐标
        foreach (Drawable drawable in drawables) {
            if (drawable != null) {
                drawable.pos += pos;
                camera.insertObject(drawable);
            }
        }
    }

}