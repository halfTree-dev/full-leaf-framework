using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using full_leaf_framework.Input;
using full_leaf_framework.Visual;
using full_leaf_framework.Physics;
using System;


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
    public Drawable test2;

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
        inputManager.InsertTrackingKeys(new Keys[9] {Keys.A, Keys.D, Keys.W, Keys.S,
        Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.E});
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
        if (inputManager.GetTrackingKey(Keys.Left).pressed) {
            test.angle -= MathF.PI / 24;
        }
        if (inputManager.GetTrackingKey(Keys.Right).pressed) {
            test.angle += MathF.PI / 24;
        }
        if (inputManager.GetTrackingKey(Keys.E).fired) {
            // TestCase，启动！
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            /*
            // 1.直线不相交
            Line line1 = new Line(new Vector2(0, 0), new Vector2(1, 2));
            Line line2 = new Line(new Vector2(1, 1), new Vector2(2, 0));
            Line line3 = new Line(new Vector2(0, 3), new Vector2(2, 3));
            Console.WriteLine(line1.IsCollision(line2)); // f
            Console.WriteLine(line1.IsCollision(line3)); // f
            // 2.直线平行或重合
            line1 = new Line(new Vector2(0, 0), new Vector2(1, 2));
            line2 = new Line(new Vector2(1, 0), new Vector2(2, 2));
            line3 = new Line(new Vector2(0.5f, 1), new Vector2(1.5f, 3));
            Line line4 = new Line(new Vector2(1.5f, 3), new Vector2(2.5f, 5));
            Console.WriteLine(line1.IsCollision(line2)); // f
            Console.WriteLine(line1.IsCollision(line3)); // t
            Console.WriteLine(line1.IsCollision(line4)); // f
            // 3.直线相交
            line1 = new Line(new Vector2(-1, -2), new Vector2(1, 2));
            line2 = new Line(new Vector2(0.5f, 0), new Vector2(0.75f, 3));
            line3 = new Line(new Vector2(-0.5f, -3), new Vector2(-0.5f, 3));
            Console.WriteLine(line1.IsCollision(line2)); // t
            Console.WriteLine(line1.IsCollision(line3)); // t
            */
            /*
            // 4.线段和圆
            Line line1 = new Line(new Vector2(0, 0), new Vector2(2, 2));
            Circle circle1 = new Circle(new Vector2(2, 0), 1.2f);
            Circle circle2 = new Circle(new Vector2(2, 0), 1.5f);
            Console.WriteLine(line1.IsCollision(circle1)); // f
            Console.WriteLine(line1.IsCollision(circle2)); // t
            Line line2 = new Line(new Vector2(3, 1), new Vector2(3, -1));
            circle1 = new Circle(new Vector2(2, 2), 1.2f);
            circle2 = new Circle(new Vector2(2, 2), 1.7f);
            Console.WriteLine(line2.IsCollision(circle1)); // f
            Console.WriteLine(line2.IsCollision(circle2)); // t
            // 5.圆和圆
            circle1 = new Circle(new Vector2(0, 0), 3f);
            circle2 = new Circle(new Vector2(4, 0), 2f);
            Console.WriteLine(circle1.IsCollision(circle2)); // t
            circle2 = new Circle(new Vector2(5, 1), 2f);
            Console.WriteLine(circle2.IsCollision(circle1)); // f
            */
            // 6.多边形和线段
            Polygon polygon1 = new Polygon(new Vector2[4] {new Vector2(1, 1), new Vector2(3, 1),
            new Vector2(4, 3), new Vector2(3, 3) });
            Line line1 = new Line(new Vector2(1, 3), new Vector2(2, 3));
            Line line2 = new Line(new Vector2(0, 1), new Vector2(1, 3));
            Line line3 = new Line(new Vector2(2, 0), new Vector2(5, 2));
            Line line4 = new Line(new Vector2(4, 1), new Vector2(3, 3));
            Console.WriteLine(polygon1.IsCollision(line1)); // f
            Console.WriteLine(polygon1.IsCollision(line2)); // f
            Console.WriteLine(polygon1.IsCollision(line3)); // f
            Console.WriteLine(polygon1.IsCollision(line4)); // t
        }
        // 添加绘制物体
        camera.insertObject(test);
        camera.insertObject(test2);
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
