/*
瓦片类可以被继承
这样可以编辑继承类来让瓦片达到不一样的效果~
*/

using System;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;

namespace full_leaf_framework.Scene;

public class ChildTile : Tile {

    public override void BeginTile(AnimatedSprite usedSprite, int usedFrameL, int usedFrameR, int currentFrame, float frameDelay, string[] extArugs)
    {
        base.BeginTile(usedSprite, usedFrameL, usedFrameR, currentFrame, frameDelay, extArugs);
        Console.WriteLine("我是一个特殊的瓦片类！");
        Console.WriteLine("我读取了额外信息: " + extArugs);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        frameDelay = 0.02f;
    }

}