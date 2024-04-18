# FullLeafFramework：为MonoGame框架的游戏搭建增添内容

# 前言
[full_leaf_framework](https://gitee.com/half_tree/full-leaf-framework)是一个以[MonoGame](https://monogame.net/articles/index.html)为基础，制作的一个满足游戏开发基本需求的**功能拓展框架**，遵循GPL-2.0开源协议。

这个拓展框架充当了一个场景编辑器，相关的代码允许你比较容易地读取玩家输入、创建TileMap场景、定义摄像机、播放动画、制造粒子效果、进行物理判定、创建菜单栏等等，它们在原版的MonoGame中并没有，这个框架就是对上述这些功能的实现。

# FLFramework可以做什么？

FLFramework是一个MonoGame的功能扩展框架，我们（就我一个人）已经在现阶段实现了以下功能。

- 屏幕摄像机`Camera`，方便完成统一的图像处理；
- 自定义的`Drawable`绘制对象，方便各种视觉效果的设置；
- 动画管理`AnimationTrack`，可自定义各类动画；
- 位图文字管理`BmfontController`，可支持位图文字的绘制；
- 瓦片地图和地物`TileMap`、`Building`、`Tile`，可以方便场景的构建，相关的类还可以继承，以实现更多效果；
- 物理管理系统`ShapeManager`，可以进行任意凸多边形和圆的碰撞判定，搭配`TilePhysics`，可以将其运用到瓦片地图。
- 输入控制`InputManager`，汇总信息，方便读取玩家的输入；
- 游戏菜单`Hud`、`HudInfo`，方便创建，并自定义游戏菜单，还可以通过继承来自定义控件功能和菜单样式；
- 粒子效果`ParticleController`，可以方便自定义粒子效果，并在游戏中实现。

通过引用存在于FLFramework中的代码并编辑一些储存各类游戏信息的Json文件，理论上你可以更方便地构造游戏场景。我也计划在未来优化代码并添加更多功能。

# 开始使用FLFramework

FLFramework实际上由一系列C#脚本组成，这些脚本已经包含了上述具有特别功能的类，可以为你的项目提供这些类的引用，所以，在你的项目中部署它只需要你将相关的文件放在你的项目目录里就行了。

### 第一步：下载项目的release版本

前往 https://gitee.com/half_tree/full-leaf-framework ，直接下载其release版本，然后将压缩包解压到你的项目中即可。

又或者，你也可以直接下载整个仓库，然后将其中/utils文件夹中的文件拷贝到你的项目中，不过我不是很推荐这样做，因为这样会引入一些我在测试过程中使用而实际上并不需要的文件。

### 第二步：安装一些依赖

该脚本使用了`Newtonsoft.Json`用来读取json文件，所以你需要在你的项目中安装它，一个比较推荐的方法是直接在项目的目录中执行安装操作。
```powershell
dotnet add package Newtonsoft.Json
```

### 第三步：在你的项目中引用FLFramework
只要使用`using`语句即可
```csharp
using full_leaf_framework;
```

# 框架使用指南

[点击这里查看框架的介绍文档 (https://half_tree.gitee.io/posts/2024-03-25-full-leaf-framework-introduction.html)](https://half_tree.gitee.io/posts/2024-03-25-full-leaf-framework-introduction.html)

[点击这里查看框架功能文档 (https://half_tree.gitee.io/posts/2024-03-29-full-leaf-framework-document.html)](https://half_tree.gitee.io/posts/2024-03-29-full-leaf-framework-document.html)

[点击这里查看使用框架实现的的样例(https://gitee.com/half_tree/full-leaf-framework-samples)](https://gitee.com/half_tree/full-leaf-framework-samples)

# 引用与感谢

目前，在这个框架的编辑中，我引用了以下的代码：

- BMFontReader  
https://github.com/ironpowertga/BMFont-Reader-Csharp  
由于Bmfont的`.fnt`文件为Xml格式，而我对此没什么了解，所以在读取信息方面我使用了这个项目中的代码帮助我解析Xml

其余的代码均由我个人完成。

# 说明

我作为一名高中生，能力有限。这个框架可能不够完善，甚至很有可能出现问题。所以，如果你正在做一个很正式的项目，而且并不想深究什么底层原理之类的，那么这个框架可能并不是很好的选择。这种情况下，MonoGame.Extended可能很适合你！

不过，如果你尝试了将框架部署到你的项目，并且阅读了文档的话。那么非常感谢，祝您使用愉快！

如果您在使用过程中发现了问题，那么欢迎您发送Issue，提出您的建议。如果您希望对这个项目有所贡献，欢迎您的Pull Request，个人将感激不尽。