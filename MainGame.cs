using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using full_leaf_framework.Input;
using full_leaf_framework.Visual;

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

    public Drawable test;

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
        inputManager.InsertTrackingKeys(new Keys[6] {Keys.A, Keys.D, Keys.W, Keys.S, Keys.Up, Keys.Down});
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
        new Vector2(0, 0), 1);

        // TODO: use this.Content to load your game content here
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
        // 添加绘制物体
        camera.insertObject(test);
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        spriteBatch.Begin();
        camera.Draw();
        spriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
