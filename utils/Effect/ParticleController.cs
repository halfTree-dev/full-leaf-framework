/*
ParticleController 粒子效果控制器
它负责生成、处理粒子，将绘制信息传入摄像机
*/

using full_leaf_framework.Scene;
using full_leaf_framework.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace full_leaf_framework.Effect;

/// <summary>
/// 粒子控制器
/// </summary>
public class ParticleController {

    // 基础信息
    /// <summary>
    /// 精灵图集信息
    /// </summary>
    private SpriteInfo[] spriteInfos;
    /// <summary>
    /// 粒子信息
    /// </summary>
    private ParticleInfo[] particleInfos;
    /// <summary>
    /// 粒子生成信息
    /// </summary>
    private BurstInfo[] burstInfos;

    private ContentManager Content;

    // 控制部分
    /// <summary>
    /// 粒子列表
    /// </summary>
    public List<Particle> particles;

    /// <summary>
    /// 初始化粒子控制器
    /// </summary>
    /// <param name="location">应当填入相对路径，从程序目录开始</param>
    public ParticleController(string location, ContentManager Content) {
        particles = new List<Particle>();
        // 填充基本数据
        var particleControllerInfo = ParticleControllerInfo.LoadParticleControllerInfo(location);
        this.Content = Content;
        burstInfos = particleControllerInfo.burstInfos;
        particleInfos = particleControllerInfo.particleInfos;
        LoadSprites(particleControllerInfo);
    }

    /// <summary>
    /// 读取SpriteInfos
    /// </summary>
    private void LoadSprites(ParticleControllerInfo particleControllerInfo) {
        spriteInfos = particleControllerInfo.spriteInfos;
        foreach (SpriteInfo spriteInfo in spriteInfos) {
            spriteInfo.texture = new AnimatedSprite(Content.Load<Texture2D>(spriteInfo.location),
            spriteInfo.rows, spriteInfo.column);
        }
    }

    /// <summary>
    /// 引发指定的粒子生成事件，可自定义参数
    /// </summary>
    public void Burst(
        string burstName, float[] velocityRange = null, float[] angleRange = null, float[] lifeTimeRange = null,
        int particleCount = -1) {
        BurstInfo burstInfo = null;
        foreach (BurstInfo currentInfo in burstInfos) {
            if (currentInfo.burstName == burstName) {
                burstInfo = currentInfo;
                break;
            }
        }
        if (burstInfo == null) { return; }
        // 找寻指定的生成事件信息
        ParticleInfo particleInfo = null;
        foreach (ParticleInfo currentInfo in particleInfos) {
            if (currentInfo.particleName == burstInfo.burstType) {
                particleInfo = currentInfo;
                break;
            }
        }
        if (particleInfo == null) { return; }
        // 寻找指定粒子信息
        int spawnCount = particleCount == -1 ? burstInfo.particleCount : particleCount;
        for (int i = 1; i <= spawnCount; i++) {
            // 开始生成
        }
    }

    /// <summary>
    /// 更新所有粒子
    /// </summary>
    public void Update(GameTime gameTime) {
        for (int i = 0; i < particles.Count; i++) {
            particles[i].Update(gameTime);
        }
    }

    /// <summary>
    /// 绘制所有粒子
    /// </summary>
    public void Draw(Camera camera) {
        // 由于Particle本身就是Drawable，所以直接添加它们到Camera中是可行的
        // 尽管和Tile一样，他们共用一个AnimatedSprite，但是动画帧数的处理在Update中完成，所以无需在此再操心
        foreach (Particle particle in particles) {
            camera.insertObject(particle);
        }
    }

    /// <summary>
    /// 随机生成一个介于给定范围的数值
    /// </summary>
    private float SpawnRandomValue(float[] range) {
        double deltaValue = new System.Random().NextDouble();
        return range[0] + (float)deltaValue * (range[1] - range[0]);
    }

}