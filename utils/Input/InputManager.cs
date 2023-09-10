/*
InputManager（输入管理器）类被置放于游戏主循环中，
它会监测玩家的输入，将玩家的所有输入传递到其它类之中，触发相应的委托。

建议在游戏主循环中设置该控制器，然后设置其为静态
*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace full_leaf_framework.Input;

/// <summary>
/// 输入控制器
/// </summary>
public class InputManager {

    /// <summary>
    /// 追踪中的按键列表
    /// </summary>
    private List<TrackingKey> tracking_keys;
    /// <summary>
    /// 按键情况记录
    /// </summary>
    private List<KeyPressedState> previous_keys;
    /// <summary>
    /// 保留按键记录的数目
    /// </summary>
    private int previous_keys_count = 50;

    /// <summary>
    /// 追踪中的鼠标
    /// </summary>
    private TrackingMouse tracking_mouse;

    // 向记录中增添按键
    internal delegate void InsertKeyLog(TrackingKey trackingKey);
    internal InsertKeyLog log_insert_key;

    /// <summary>
    /// 创建一个输入控制器
    /// </summary>
    public InputManager() {
        tracking_keys = new List<TrackingKey>();
        previous_keys = new List<KeyPressedState>();
        log_insert_key = new InsertKeyLog(InsertNewKey);
        tracking_mouse = new TrackingMouse();
    }

    /// <summary>
    /// 插入追踪按键
    /// </summary>
    /// <param name="keys">按键组</param>
    public void InsertTrackingKeys(Keys[] keys) {
        foreach (Keys key in keys) {
            // 创建追踪按键
            TrackingKey new_key = new TrackingKey(key, this);
            bool is_repeated = false;
            foreach (TrackingKey tracked_key in tracking_keys) {
                if (tracked_key.key_id == key) {
                    is_repeated = true;
                }
            }
            if (!is_repeated) {
                // 不重复时增添按键
                tracking_keys.Add(new_key);
            }
        }
    }

    /// <summary>
    /// 移除追踪按键
    /// </summary>
    /// <param name="keys">按键组</param>
    public void ExcludeTrackingKeys(Keys[] keys) {
        foreach (Keys key in keys) {
            // 检测并移除按键
            foreach (TrackingKey trackingKey in tracking_keys) {
                if (trackingKey.key_id == key) {
                    tracking_keys.Remove(trackingKey);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 更新输入控制器状态
    /// </summary>
    public void Update(GameTime gameTime) {
        KeyboardState kstate = Keyboard.GetState();
        foreach (TrackingKey key in tracking_keys) {
            key.Update(kstate, gameTime);
        }
        // 键盘
        tracking_mouse.Update(gameTime);
        // 鼠标
    }

    /// <summary>
    /// 添加新的按键记录
    /// </summary>
    /// <param name="trackingKey">要添加的按键</param>
    private void InsertNewKey(TrackingKey trackingKey) {
        if (previous_keys.Count <= previous_keys_count) {
            previous_keys.Add(new KeyPressedState(trackingKey.key_id, trackingKey.hold_time));
        }
        else {
            previous_keys.Remove(previous_keys[0]);
            previous_keys.Add(new KeyPressedState(trackingKey.key_id, trackingKey.hold_time));
            // 删除最晚的按键记录
        }
        // 添加新的按键记录
    }

    /// <summary>
    /// 获取追踪中的按键
    /// </summary>
    /// <param name="key">按键id</param>
    /// <returns>按键状态对象</returns>
    public TrackingKey GetTrackingKey(Keys key) {
        foreach (TrackingKey trackingKey in tracking_keys) {
            if (trackingKey.key_id == key) {
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
    public TrackingMouse GetTrackingMouse() {
        return tracking_mouse;
    }

}

/// <summary>
/// 追踪中的按键
/// </summary>
public class TrackingKey {

    /// <summary>
    /// 按键id
    /// </summary>
    public Keys key_id;
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
    public float hold_time;

    // 按键松开时记录
    internal event InputManager.InsertKeyLog insert_released_key;

    /// <summary>
    /// 生成一个追踪按键
    /// </summary>
    /// <param name="key_id">按键id</param>
    public TrackingKey(Keys key_id, InputManager inputManager) {
        this.key_id = key_id;
        fired = pressed = released = false;
        hold_time = 0f;
        // 委托InputManager添加记录
        insert_released_key = new InputManager.InsertKeyLog(inputManager.log_insert_key);
    }

    /// <summary>
    /// 更新该追踪按键
    /// </summary>
    public void Update(KeyboardState kstate, GameTime gameTime) {
        if (kstate.IsKeyDown(key_id)) {
            pressed = true;
            released = false;
            if (hold_time == 0f) {
                fired = true;
                // 刚刚按下
            }
            hold_time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else {
            pressed = false;
            fired = false;
            if (hold_time > 0f) {
                released = true;
                // 刚刚松开
                insert_released_key(this);
            }
            hold_time = 0f;
        }
    }

}

/// <summary>
/// 按键按下情况
/// </summary>
public class KeyPressedState {

    /// <summary>
    /// 按键id
    /// </summary>
    public Keys key;
    /// <summary>
    /// 曾今持续的按键时间
    /// </summary>
    public float pressed_time;

    /// <summary>
    /// 创造一个按键按下情况
    /// </summary>
    public KeyPressedState(Keys key, float pressed_time) {
        this.key = key;
        this.pressed_time = pressed_time;
    }

}

/// <summary>
/// 追踪中的鼠标
/// </summary>
public class TrackingMouse {

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
    public float hold_time_left;
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
    public float hold_time_right;

    /// <summary>
    /// 创建鼠标追踪器
    /// </summary>
    public TrackingMouse() {
        pos = new Point(0, 0);
        firedLeft = pressedLeft = releasedLeft = false;
        firedRight = pressedRight = releasedRight = false;
        hold_time_left = hold_time_right = 0f;
    }

    /// <summary>
    /// 更新该追踪按键
    /// </summary>
    public void Update(GameTime gameTime) {
        MouseState mstate = Mouse.GetState();
        pos = mstate.Position;
        if (mstate.LeftButton == ButtonState.Pressed) {
            pressedLeft = true;
            releasedLeft = false;
            if (hold_time_left == 0f) {
                firedLeft = true;
                // 刚刚按下
            }
            hold_time_left += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else {
            pressedLeft = false;
            firedLeft = false;
            if (hold_time_left > 0f) {
                releasedLeft = true;
                // 刚刚松开
            }
            hold_time_left = 0f;
        }
        // 鼠标左键判定
        if (mstate.RightButton == ButtonState.Pressed) {
            pressedRight = true;
            releasedRight = false;
            if (hold_time_right == 0f) {
                firedRight = true;
                // 刚刚按下
            }
            hold_time_right += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else {
            pressedRight = false;
            firedRight = false;
            if (hold_time_right > 0f) {
                releasedRight = true;
                // 刚刚松开
            }
            hold_time_right = 0f;
        }
        // 鼠标右键判定
    }

}