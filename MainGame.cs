using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using full_leaf_framework.Interact;
using full_leaf_framework.Visual;
using full_leaf_framework.Physics;
using full_leaf_framework.Scene;
using System;
using full_leaf_framework.Effect;

namespace full_leaf_framework;

public class MainGame : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    // 窗口尺寸
    public static int SCREEN_WIDTH = 800;
    public static int SCREEN_HEIGHT = 600;

    public static InputManager inputManager;
    public static Camera camera;

    public TileMap tileMap;

    public Drawable test;
    public Drawable test2;

    public HudController hudController;

    public static AnimationTrackController trackController;

    public ParticleController particleController;

    public MainGame()
    {
        graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = SCREEN_WIDTH,
            PreferredBackBufferHeight = SCREEN_HEIGHT
        };
        graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        inputManager = new InputManager();
        inputManager.InsertTrackingKeys(new Keys[12] {Keys.A, Keys.D, Keys.W, Keys.S,
        Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.E, Keys.O, Keys.P, Keys.R});
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        // 测试camera
        camera = new Camera(spriteBatch, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
        new Vector2(0, 0), new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT));
        // 测试物体
        test = new Drawable(new AnimatedSprite(Content.Load<Texture2D>("Characters/test")), new Vector2(0, 0),
        new Vector2(0, 50), 1);
        test2 = new Drawable(new AnimatedSprite(Content.Load<Texture2D>("Characters/test")), new Vector2(100, 0),
        new Vector2(0, -50), 1);
        trackController = AnimationInfo.LoadAnimationInfo("utils/Visual/sample_animation.json");
        tileMap = new TileMap("utils/Scene/test_map.json", Content);
        particleController = new ParticleController("utils/Effect/test_particles.json", Content);
        // TODO: use this.Content to load your game content here
        // 读取菜单
        Camera hudCamera = new Camera(spriteBatch, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
        new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT));
        hudController = new HudController(Content, hudCamera);
        hudController.AddHud("utils/Interact/test_hud.json", true, true);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        inputManager.Update(gameTime);
        // 操控摄像机
        if (inputManager.GetTrackingKey(Keys.A).fired) {
            camera.pos.X -= 20;
        }
        if (inputManager.GetTrackingKey(Keys.D).fired) {
            camera.pos.X += 20;
        }
        if (inputManager.GetTrackingKey(Keys.S).fired) {
            camera.pos.Y += 20;
        }
        if (inputManager.GetTrackingKey(Keys.W).fired) {
            camera.pos.Y -= 20;
        }
        if (inputManager.GetTrackingKey(Keys.Up).fired) {
            camera.scale += 0.1f;
        }
        if (inputManager.GetTrackingKey(Keys.Down).fired) {
            camera.scale -= 0.1f;
        }
        if (inputManager.GetTrackingKey(Keys.Left).pressed) {
            test.angle -= MathF.PI / 24;
        }
        if (inputManager.GetTrackingKey(Keys.Right).pressed) {
            test.angle += MathF.PI / 24;
        }
        if (inputManager.GetTrackingKey(Keys.E).fired) {
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            tileMap = new TileMap("utils/Scene/test_map.json", Content);
            Polygon polygon1 = new Polygon(new Vector2[4] { new Vector2(32,32),
            new Vector2(32,70), new Vector2(70,70), new Vector2(70,32) });
            Polygon polygon2 = new Polygon(new Vector2[4] { new Vector2(-16,-48),
            new Vector2(16,70), new Vector2(50,80), new Vector2(50,-32) });
            Polygon polygon3 = new Polygon(new Vector2[3] { new Vector2(32,80),
            new Vector2(232,280), new Vector2(32,280) });
            Console.WriteLine(tileMap.IsCollision(polygon1, 2) + " - " + tileMap.IsCollision(polygon2, 2)
            + " - " + tileMap.IsCollision(polygon3, 2) + " - " + tileMap.IsCollision(polygon1, 1));
            Console.WriteLine((float)gameTime.TotalGameTime.TotalSeconds - time);
            // Reload Hud
            Camera hudCamera = new Camera(spriteBatch, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
            new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT));
            hudController = new HudController(Content, hudCamera);
            hudController.AddHud("utils/Interact/test_hud.json", true, true);
            Console.WriteLine("Reload Hud");
        }
        if (inputManager.GetTrackingKey(Keys.O).fired) {
            particleController.Burst("testBurst1", new Vector2(0, 0));
            hudController.RunHudCommandSequence("testHud", "no_animation");
        }
        if (inputManager.GetTrackingKey(Keys.P).fired) {
            particleController.Burst("testBurst2", new Vector2(0, 0));
            hudController.RunHudCommandSequence("testHud", "pause_animation");
        }
        // 添加绘制物体
        // camera.insertObject(test);
        // camera.insertObject(test2);
        // TODO: Add your update logic here
        tileMap.Update(gameTime);
        particleController.Update(gameTime);
        tileMap.Draw(camera);
        particleController.Draw(camera);
        hudController.Update(gameTime, inputManager);
        hudController.InsertDrawObjects();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.Begin();
        camera.Draw();
        hudController.Draw();
        spriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
