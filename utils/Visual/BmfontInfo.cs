// 在下面这一段中，我引用了了https://github.com/ironpowertga/BMFont-Reader-Csharp 项目中的代码，
// 这段代码提供了一个可以得到Xml反序列化信息的类：BmfontInfo（如下）
// 在此表示感谢 Copyright (C) 2019 Antoine Guilbaud (IronPowerTga)

using System.Xml.Serialization;

[XmlRoot("font")]
public class BmfontInfo
{
    /// <summary>
    /// 字体生成信息
    /// </summary>
    [XmlElement("info")]
    public FontInfo Info { get; set; }

    /// <summary>
    /// 所有字符的共用信息
    /// </summary>
    [XmlElement("common")]
    public FontCommon Common { get; set; }

    /// <summary>
    /// 纹理文件的名称列表
    /// </summary>
    [XmlArray("pages")]
    [XmlArrayItem("page")]
    public FontPage[] Pages { get; set; }

    /// <summary>
    /// 所有字符的列表
    /// </summary>
    [XmlArray("chars")]
    [XmlArrayItem("char")]
    public FontChar[] Chars { get; set; }

    /// <summary>
    /// 字偶距的列表
    /// 用来调整一些特殊字符之间的间隔
    /// </summary>
    [XmlArray("kernings")]
    [XmlArrayItem("kerning")]
    public FontKerning[] Kernings { get; set; }

}

/// <summary>
/// 字体生成信息
/// </summary>
public class FontInfo
{
    /// <summary>
    /// 使用的字体名称
    /// </summary>
    [XmlAttribute("face")]
    public string Face { get; set; }

    /// <summary>
    /// 字体的大小
    /// </summary>
    [XmlAttribute("size")]
    public int Size { get; set; }

    /// <summary>
    /// 是否是粗体（0/1）
    /// </summary>
    [XmlAttribute("bold")]
    public int Bold { get; set; }

    /// <summary>
    /// 是否是斜体（0/1）
    /// </summary>
    [XmlAttribute("italic")]
    public int Italic { get; set; }

    /// <summary>
    /// 当不使用unicode时，字符集的名称（为空时使用unicode）
    /// </summary>
    [XmlAttribute("charset")]
    public string CharSet { get; set; }

    /// <summary>
    /// 是否使用unicode（0/1）
    /// </summary>
    public int unicode;

    /// <summary>
    /// 字符高度延申比例
    /// </summary>
    [XmlAttribute("stretchH")]
    public int StretchH { get; set; }

    /// <summary>
    /// 是否使用平滑处理
    /// </summary>
    [XmlAttribute("smooth")]
    public int Smooth { get; set; }

    /// <summary>
    /// 是否使用超采样
    /// </summary>
    [XmlAttribute("aa")]
    public int SuperSampling { get; set; }

    /// <summary>
    /// 字符内间距（四个方向）
    /// </summary>
    [XmlAttribute("padding")]
    public string Padding { get; set; }

    /// <summary>
    /// 字符之间的间距
    /// </summary>
    [XmlAttribute("spacing")]
    public string Spacing { get; set; }

    /// <summary>
    /// 字符的轮廓宽度
    /// </summary>
    [XmlAttribute("outline")]
    public int Outline { get; set; }
}

/// <summary>
/// 所有字符的共用信息
/// </summary>
public class FontCommon
{
    /// <summary>
    /// 换行时文本的上下间距
    /// </summary>
    [XmlAttribute("lineHeight")]
    public int LineHeight { get; set; }

    /// <summary>
    /// 从一行的顶部到其基部的距离
    /// </summary>
    [XmlAttribute("base")]
    public int Base { get; set; }

    /// <summary>
    /// 纹理文件的宽度
    /// </summary>
    [XmlAttribute("scaleW")]
    public int ScaleW { get; set; }

    /// <summary>
    /// 纹理文件的高度
    /// </summary>
    [XmlAttribute("scaleH")]
    public int ScaleH { get; set; }

    /// <summary>
    /// 纹理集的页数
    /// </summary>
    [XmlAttribute("pages")]
    public int Pages { get; set; }

    // 以下属性描述纹理的一些颜色通道等，我还没有考虑使用
    [XmlAttribute("packed")]
    public int Packed { get; set; }

    [XmlAttribute("alphaChnl")]
    public int AlphaChnl { get; set; }

    [XmlAttribute("redChnl")]
    public int RedChnl { get; set; }

    [XmlAttribute("blueChnl")]
    public int BlueChnl { get; set; }
}

/// <summary>
/// 纹理文件
/// </summary>
public class FontPage
{

    /// <summary>
    /// 纹理文件的id
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// 纹理文件的文件名
    /// </summary>
    [XmlAttribute("file")]
    public string File { get; set; }
}

/// <summary>
/// 文字信息
/// </summary>
public class FontChar
{
    /// <summary>
    /// 文字id（对应其ASCII）
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// 文字在纹理中的X位置
    /// </summary>
    [XmlAttribute("x")]
    public int X { get; set; }

    /// <summary>
    /// 文字在纹理中的Y位置
    /// </summary>
    [XmlAttribute("y")]
    public int Y { get; set; }

    /// <summary>
    /// 文字的宽度
    /// </summary>
    [XmlAttribute("width")]
    public int Width { get; set; }

    /// <summary>
    /// 文字的高度
    /// </summary>
    [XmlAttribute("height")]
    public int Height { get; set; }

    /// <summary>
    /// 文字被绘制后的X方向偏移量
    /// </summary>
    [XmlAttribute("xoffset")]
    public int XOffset { get; set; }

    /// <summary>
    /// 文字被绘制后的Y方向偏移量
    /// </summary>
    [XmlAttribute("yoffset")]
    public int YOffset { get; set; }

    /// <summary>
    /// 文字绘制完成后下一个文字的横向偏移大小
    /// </summary>
    [XmlAttribute("xadvance")]
    public int XAdvance { get; set; }

    /// <summary>
    /// 文字所在的纹理图集id
    /// </summary>
    [XmlAttribute("page")]
    public int Page { get; set; }

    /// <summary>
    /// 颜色通道的使用状态
    /// </summary>
    [XmlAttribute("chnl")]
    public int Channel { get; set; }
}

/// <summary>
/// 子偶距信息
/// </summary>
public class FontKerning
{
    /// <summary>
    /// 第一个字符
    /// </summary>
    [XmlAttribute("first")]
    public int First { get; set; }

    /// <summary>
    /// 第二个字符
    /// </summary>
    [XmlAttribute("second")]
    public int Second { get; set; }

    /// <summary>
    /// 它们之间的偏移量
    /// </summary>
    [XmlAttribute("amount")]
    public int Amount { get; set; }
}
