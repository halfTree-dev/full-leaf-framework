/*
InputManager（输入管理器）类被置放于游戏主循环中，
它会监测玩家的输入，将玩家的所有输入传递到其它类之中，触发相应的委托。

建议在游戏主循环中设置该控制器，然后设置其为静态
*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace full_leaf_framework.Interact;

/// <summary>
/// 输入控制器
/// </summary>
public class InputManager
{

    /// <summary>
    /// 追踪中的按键列表
    /// </summary>
    private List<TrackingKey> trackingKeys;
    /// <summary>
    /// 按键情况记录
    /// </summary>
    private List<KeyPressedState> previousKeys;
    /// <summary>
    /// 保留按键记录的数目
    /// </summary>
    private int previousKeysCount = 50;

    /// <summary>
    /// 追踪中的鼠标
    /// </summary>
    private TrackingMouse trackingMouse;

    // 向记录中增添按键
    internal delegate void InsertKeyLog(TrackingKey trackingKey);
    internal InsertKeyLog log_insert_key;

    /// <summary>
    /// 创建一个输入控制器
    /// </summary>
    public InputManager()
    {
        trackingKeys = new List<TrackingKey>();
        previousKeys = new List<KeyPressedState>();
        log_insert_key = new InsertKeyLog(InsertNewKey);
        trackingMouse = new TrackingMouse();
    }

    /// <summary>
    /// 插入追踪按键
    /// </summary>
    /// <param name="keys">按键组</param>
    public void InsertTrackingKeys(Keys[] keys)
    {
        foreach (Keys key in keys)
        {
            // 创建追踪按键
            TrackingKey new_key = new TrackingKey(key, this);
            bool is_repeated = false;
            foreach (TrackingKey tracked_key in trackingKeys)
            {
                if (tracked_key.keyId == key)
                {
                    is_repeated = true;
                }
            }
            if (!is_repeated)
            {
                // 不重复时增添按键
                trackingKeys.Add(new_key);
            }
        }
    }

    /// <summary>
    /// 移除追踪按键
    /// </summary>
    /// <param name="keys">按键组</param>
    public void ExcludeTrackingKeys(Keys[] keys)
    {
        foreach (Keys key in keys)
        {
            // 检测并移除按键
            foreach (TrackingKey trackingKey in trackingKeys)
            {
                if (trackingKey.keyId == key)
                {
                    trackingKeys.Remove(trackingKey);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 更新输入控制器状态
    /// </summary>
    public void Update(GameTime gameTime)
    {
        KeyboardState kstate = Keyboard.GetState();
        foreach (TrackingKey key in trackingKeys)
        {
            key.Update(kstate, gameTime);
        }
        // 键盘
        trackingMouse.Update(gameTime);
        // 鼠标
    }

    /// <summary>
    /// 添加新的按键记录
    /// </summary>
    /// <param name="trackingKey">要添加的按键</param>
    private void InsertNewKey(TrackingKey trackingKey)
    {
        if (previousKeys.Count <= previousKeysCount)
        {
            previousKeys.Add(new KeyPressedState(trackingKey.keyId, trackingKey.holdTime));
        }
        else
        {
            previousKeys.Remove(previousKeys[0]);
            previousKeys.Add(new KeyPressedState(trackingKey.keyId, trackingKey.holdTime));
            // 删除最晚的按键记录
        }
        // 添加新的按键记录
    }

    /// <summary>
    /// 获取追踪中的按键
    /// </summary>
    /// <param name="key">按键id</param>
    /// <returns>按键状态对象</returns>
    public TrackingKey GetTrackingKey(Keys key)
    {
        foreach (TrackingKey trackingKey in trackingKeys)
        {
            if (trackingKey.keyId == key)
            {
                return trackingKey;
                // 返回特定的按键对象
            }
        }
        return null;
    }

    /// <summary>
    /// 获取追踪中的鼠标
    /// </summary>
    /// <returns>鼠标状态对象</returns>
    public TrackingMouse GetTrackingMouse()
    {
        return trackingMouse;
    }

}

/// <summary>
/// 追踪中的按键
/// </summary>
public class TrackingKey
{

    /// <summary>
    /// 按键id
    /// </summary>
    public Keys keyId;
    /// <summary>
    /// 是否刚刚激活
    /// </summary>
    public bool fired;
    /// <summary>
    /// 是否被按下
    /// </summary>
    public bool pressed;
    /// <summary>
    /// 是否刚刚放开
    /// </summary>
    public bool released;
    /// <summary>
    /// 按压时间
    /// </summary>
    public float holdTime;

    // 按键松开时记录
    internal event InputManager.InsertKeyLog insert_released_key;

    /// <summary>
    /// 生成一个追踪按键
    /// </summary>
    /// <param name="keyId">按键id</param>
    public TrackingKey(Keys keyId, InputManager inputManager)
    {
        this.keyId = keyId;
        fired = pressed = released = false;
        holdTime = 0f;
        // 委托InputManager添加记录
        insert_released_key = new InputManager.InsertKeyLog(inputManager.log_insert_key);
    }

    /// <summary>
    /// 更新该追踪按键
    /// </summary>
    public void Update(KeyboardState kstate, GameTime gameTime)
    {
        if (kstate.IsKeyDown(keyId))
        {
            pressed = true;
            released = false;
            if (holdTime == 0f)
            {
                fired = true;
                // 刚刚按下
            }
            else {
                fired = false;
            }
            holdTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else
        {
            pressed = false;
            fired = false;
            if (holdTime > 0f)
            {
                released = true;
                // 刚刚松开
                insert_released_key(this);
            }
            else {
                released = false;
            }
            holdTime = 0f;
        }
    }

}

/// <summary>
/// 按键按下情况
/// </summary>
public class KeyPressedState
{

    /// <summary>
    /// 按键id
    /// </summary>
    public Keys key;
    /// <summary>
    /// 曾今持续的按键时间
    /// </summary>
    public float pressedTime;

    /// <summary>
    /// 创造一个按键按下情况
    /// </summary>
    public KeyPressedState(Keys key, float pressedTime)
    {
        this.key = key;
        this.pressedTime = pressedTime;
    }

}

/// <summary>
/// 追踪中的鼠标
/// </summary>
public class TrackingMouse
{

    /// <summary>
    /// 鼠标的位置
    /// </summary>
    public Point pos;
    /// <summary>
    /// 是否刚刚激活（左键）
    /// </summary>
    public bool firedLeft;
    /// <summary>
    /// 是否被按下（左键）
    /// </summary>
    public bool pressedLeft;
    /// <summary>
    /// 是否刚刚放开（左键）
    /// </summary>
    public bool releasedLeft;
    /// <summary>
    /// 按压时间（左键）
    /// </summary>
    public float holdTimeLeft;
    /// <summary>
    /// 是否刚刚激活（右键）
    /// </summary>
    public bool firedRight;
    /// <summary>
    /// 是否被按下（右键）
    /// </summary>
    public bool pressedRight;
    /// <summary>
    /// 是否刚刚放开（右键）
    /// </summary>
    public bool releasedRight;
    /// <summary>
    /// 按压时间（右键）
    /// </summary>
    public float holdTimeRight;

    /// <summary>
    /// 创建鼠标追踪器
    /// </summary>
    public TrackingMouse()
    {
        pos = new Point(0, 0);
        firedLeft = pressedLeft = releasedLeft = false;
        firedRight = pressedRight = releasedRight = false;
        holdTimeLeft = holdTimeRight = 0f;
    }

    /// <summary>
    /// 更新该追踪按键
    /// </summary>
    public void Update(GameTime gameTime)
    {
        MouseState mstate = Mouse.GetState();
        pos = mstate.Position;
        if (mstate.LeftButton == ButtonState.Pressed)
        {
            pressedLeft = true;
            releasedLeft = false;
            if (holdTimeLeft == 0f)
            {
                firedLeft = true;
                // 刚刚按下
            }
            holdTimeLeft += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else
        {
            pressedLeft = false;
            firedLeft = false;
            if (holdTimeLeft > 0f)
            {
                releasedLeft = true;
                // 刚刚松开
            }
            holdTimeLeft = 0f;
        }
        // 鼠标左键判定
        if (mstate.RightButton == ButtonState.Pressed)
        {
            pressedRight = true;
            releasedRight = false;
            if (holdTimeRight == 0f)
            {
                firedRight = true;
                // 刚刚按下
            }
            holdTimeRight += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else
        {
            pressedRight = false;
            firedRight = false;
            if (holdTimeRight > 0f)
            {
                releasedRight = true;
                // 刚刚松开
            }
            holdTimeRight = 0f;
        }
        // 鼠标右键判定
    }

}