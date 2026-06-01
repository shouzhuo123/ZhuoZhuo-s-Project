# Unity 游戏开发框架文档

## 概述

本框架是一套轻量级 Unity 游戏开发工具集，提供单例模式、事件中心、UI管理、资源加载、对象池、场景管理、音乐音效、输入管理、延时执行等常用模块。框架遵循"约定优于配置"原则，API 简洁，适合中小型 Unity 项目快速开发。

---

## 架构图

```
┌─────────────────────────────────────────────────────────┐
│                      游戏业务层                          │
├─────────────────────────────────────────────────────────┤
│  UIMgr    │ InputMgr/SteamVRInputMgr │ SceneMgr │ MusicMgr│
├───────────┴──────────────────────────┴──────────┴────────┤
│  EventCenter  │  PoolMgr  │  ResMgr  │  DelayManager    │
├──────────────────────────────────────────────────────────┤
│  MonoMgr / MonoController  (帧更新 + 协程载体)           │
├──────────────────────────────────────────────────────────┤
│  BaseManager<T>  │  SingletonMono<T>  │  SingletonAutoMono│
│  (单例基类)        │  (Mono单例)         │  (自动Mono单例)  │
└──────────────────────────────────────────────────────────┘
```

---

## 模块详解

### 1. BaseManager<T> — 单例基类

所有 Manager 的基类，提供普通 C# 类的单例模式。

```csharp
// 使用方式：所有 Manager 直接继承即可
public class MyManager : BaseManager<MyManager> { }

// 获取实例
MyManager.GetInstance().DoSomething();
```

**特点：** 懒加载，首次调用 `GetInstance()` 时创建实例。

---

### 2. SingletonMono<T> / SingletonAutoMono<T> — MonoBehaviour 单例

用于需要挂载到 GameObject 上的单例组件。

| 类型 | 创建方式 | 适用场景 |
|------|---------|---------|
| `SingletonMono<T>` | 手动拖到场景物体上 | 需要预先配置的单例 |
| `SingletonAutoMono<T>` | 代码自动创建 | 无需手动放置的单例 |

```csharp
public class GameManager : SingletonAutoMono<GameManager>
{
    // 自动创建名为 "GameManager" 的 GameObject，场景切换不销毁
}
```

**SingletonAutoMono** 的 GameObject 在创建时会自动调用 `DontDestroyOnLoad`，确保跨场景存在。两个类都已加入重复实例检测 — 如果场景中已存在实例，新的会被自动销毁。

---

### 3. EventCenter — 事件中心

**观察者模式的实现，是整个框架的通信枢纽。** 任何模块都可以通过事件名来监听或触发事件，极大降低模块间的耦合。

```csharp
// === 无参事件 ===
// 监听
EventCenter.GetInstance().AddEventListener("玩家死亡", OnPlayerDeath);
// 触发
EventCenter.GetInstance().EventTrigger("玩家死亡");
// 移除
EventCenter.GetInstance().RemoveEventListener("玩家死亡", OnPlayerDeath);

// === 带参事件 ===
// 监听
EventCenter.GetInstance().AddEventListener<int>("得分变化", OnScoreChanged);
// 触发
EventCenter.GetInstance().EventTrigger("得分变化", 100);
// 移除
EventCenter.GetInstance().RemoveEventListener<int>("得分变化", OnScoreChanged);

// 清空所有事件（切换场景时使用）
EventCenter.GetInstance().Clear();
```

**注意事项：**
- 事件名区分大小写，建议使用常量管理事件名
- 切换场景时记得 `Clear()` 防止遗留引用
- 带参与无参事件使用不同的事件名互不影响
- 所有监听事件移除后，对应 key 会自动从字典中清除

---

### 4. MonoMgr / MonoController — 公共 MonoBehaviour 模块

让不继承 MonoBehaviour 的普通 C# 类也能使用 **Update 帧更新** 和 **协程**。

```csharp
// 添加帧更新
MonoMgr.GetInstance().AddUpdateListener(MyUpdate);
// 移除帧更新
MonoMgr.GetInstance().RemoveUpdateListener(MyUpdate);
// 开启协程
MonoMgr.GetInstance().StartCoroutine(MyCoroutine());
// 停止协程
MonoMgr.GetInstance().StopCoroutine(coroutine);

void MyUpdate()
{
    // 每帧执行
}
```

**原理：** MonoController 是一个挂载在 DontDestroyOnLoad GameObject 上的 MonoBehaviour，通过 event 委托收集外部方法，在自身的 Update 中统一调用。这样就实现了"普通类也能有 Update 方法"。

---

### 5. ResMgr — 资源管理器

封装 `Resources.Load`，提供同步和异步两种加载方式。**GameObject 类型资源会自动 Instantiate。**

```csharp
// 同步加载
GameObject prefab = ResMgr.GetInstance().Load<GameObject>("Prefabs/Player");
AudioClip clip = ResMgr.GetInstance().Load<AudioClip>("Audio/BGM");

// 异步加载
ResMgr.GetInstance().LoadAsync<GameObject>("Prefabs/Enemy", (obj) => {
    // 加载完成后的回调
    obj.transform.position = spawnPoint;
});
```

**注意：** 资源路径相对于 `Resources` 文件夹，不包含文件扩展名。

---

### 6. PoolMgr — 对象池

缓存 GameObject 避免频繁创建/销毁，提升性能。适用于子弹、特效、敌人等频繁生成的对象。

```csharp
// 从池中获取对象（池中没有则自动加载）
PoolMgr.GetInstance().GetObj("Prefabs/Bullet", (obj) => {
    obj.transform.position = firePoint;
});

// 归还对象到池中
PoolMgr.GetInstance().PushObj("Prefabs/Bullet", bulletObj);

// 清空对象池（切换场景时使用）
PoolMgr.GetInstance().Clear();
```

**配合 AutoPush 组件使用：** 将 `AutoPush` 脚本挂到预制体上，对象 `OnEnable` 后 1 秒自动回池。

```csharp
// AutoPush.cs - 挂到预制体即可，无需额外代码
// OnEnable 1秒后自动 PushObj
```

---

### 7. UIManager — UI 管理器

管理 UI 面板的显示、隐藏和层级。

```csharp
// UI层级枚举
public enum E_UI_Layer { Bot, Mid, Top, System }

// 显示面板
UIManager.GetInstance().ShowPanel<LoginPanel>("LoginPanel", E_UI_Layer.Mid, (panel) => {
    panel.SetData(userData);
});

// 隐藏面板
UIManager.GetInstance().HidePanel("LoginPanel");

// 获取已显示的面板
var panel = UIManager.GetInstance().GetPanel<LoginPanel>("LoginPanel");

// 获取层级父物体
Transform layerRoot = UIManager.GetInstance().GetLayerFather(E_UI_Layer.Top);
```

**UI 结构要求：** `Resources/UI/Canvas` 下需要有 Bot、Mid、Top、System 四个子节点，以及 `Resources/UI/EventSystem` 预制体。

---

### 8. BasePanel — UI 面板基类

所有 UI 面板脚本需继承此类。**自动查找当前面板下所有 UI 控件**，通过控件名即可获取引用，省去手动拖拽。

```csharp
public class LoginPanel : BasePanel
{
    protected override void Awake()
    {
        base.Awake(); // 必须先调用基类，完成控件收集
    }

    public override void ShowMe()
    {
        // 面板显示时调用
        var btn = GetControl<Button>("LoginBtn");
        btn.onClick.AddListener(OnLoginClick);
    }

    public override void HideMe()
    {
        // 面板隐藏时调用，清理监听
    }

    // 按钮自动绑定：名为 "LoginBtn" 的按钮被点击时自动调用
    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "LoginBtn":
                DoLogin();
                break;
            case "CloseBtn":
                UIManager.GetInstance().HidePanel("LoginPanel");
                break;
        }
    }
}
```

**核心方法：**
- `GetControl<T>(controlName)` — 通过 GameObject 名称获取组件
- `OnClick(btnName)` — 按钮点击回调，根据按钮名分发

---

### 9. SceneMgr — 场景管理器

同步和异步加载场景，自动清理事件中心和对象池。

```csharp
// 同步加载（带回调）
SceneMgr.GetInstance().LoadScene("GameScene", () => {
    Debug.Log("场景加载完成");
});

// 同步加载（无回调）
SceneMgr.GetInstance().LoadScene("MainMenu");

// 异步加载（支持进度条）
SceneMgr.GetInstance().LoadSceneAsyn("GameScene", () => {
    Debug.Log("异步加载完成");
});
```

**注意：** 切换场景时会自动调用 `EventCenter.Clear()` 和 `PoolMgr.Clear()`。异步加载在 `progress >= 0.9` 后才允许激活场景，确保场景完全就绪。

---

### 10. MusicMgr — 音乐音效管理器

管理背景音乐和音效播放。

```csharp
// 背景音乐
MusicMgr.GetInstance().PlayBKMusic("MainTheme");  // 播放 Resources/Music/bk/ 下的音频
MusicMgr.GetInstance().PauseBKMusic();
MusicMgr.GetInstance().StopBKMusic();
MusicMgr.GetInstance().BkVolume = 0.5f;           // 音量 0~1

// 音效
MusicMgr.GetInstance().PlaySound("Explosion", false, (source) => {
    // 播放完成后的回调
});
MusicMgr.GetInstance().SoundVolume = 0.8f;
MusicMgr.GetInstance().StopAllSound();

// 切换场景时停止所有音效
MusicMgr.GetInstance().StopAllSound();
```

**资源路径：**
- 背景音乐：`Resources/Music/bk/`
- 音效：`Resources/Music/Sounds/`

**特点：** 音效播放完毕后自动销毁 AudioSource，通过 Update 轮询清理。

---

### 11. InputMgr — 键盘输入管理器

基于事件中心的输入检测，将键盘输入转为事件分发。

```csharp
// 开启输入检测
InputMgr.GetInstance().StartOrEndCheck(true);

// 注册要监听的按键
InputMgr.GetInstance().RegisterKey(KeyCode.Space);
InputMgr.GetInstance().RegisterKey(KeyCode.E);
// 或直接注册 WASD
InputMgr.GetInstance().RegisterWASD();

// 监听从事件中心接收
EventCenter.GetInstance().AddEventListener<KeyCode>("KeyDown", OnKeyDown);
EventCenter.GetInstance().AddEventListener<KeyCode>("KeyUp", OnKeyUp);

void OnKeyDown(KeyCode key)
{
    switch (key)
    {
        case KeyCode.W: MoveForward(); break;
        case KeyCode.Space: Jump(); break;
    }
}

// 也可以直接轮询
if (InputMgr.GetInstance().GetKeyDown(KeyCode.E))
    Interact();
```

---

### 12. SteamVRInputMgr — VR 输入管理器

专门为 SteamVR 设计的输入管理器，与 `InputMgr` 同模式。支持左右手柄的按键、触摸板、摇杆、扳机、握持检测。

```csharp
// 绑定默认动作（使用 SteamVR 默认动作集）
SteamVRInputMgr.GetInstance().BindDefaultActions();

// 开启检测
SteamVRInputMgr.GetInstance().StartOrEndCheck(true);

// === 事件方式（通过 EventCenter 接收）===
EventCenter.GetInstance().AddEventListener<VRHand>("VR_TriggerDown", OnTriggerDown);
EventCenter.GetInstance().AddEventListener<VRHand>("VR_GripDown", OnGripDown);

// 也支持分左右手的事件
EventCenter.GetInstance().AddEventListener<VRHand>("VR_TriggerDown_Left", OnLeftTrigger);
EventCenter.GetInstance().AddEventListener<VRHand>("VR_TriggerDown_Right", OnRightTrigger);

void OnTriggerDown(VRHand hand)
{
    if (hand == VRHand.Left)  Debug.Log("左手扳机按下");
    if (hand == VRHand.Right) Debug.Log("右手扳机按下");
}

// === 轮询方式 ===
// 扳机
SteamVRInputMgr.GetInstance().GetTrigger(VRHand.Right);
SteamVRInputMgr.GetInstance().GetTriggerDown(VRHand.Right);
SteamVRInputMgr.GetInstance().GetTriggerPull(VRHand.Right);  // 扳机力度 0~1

// 握持
SteamVRInputMgr.GetInstance().GetGrip(VRHand.Left);
SteamVRInputMgr.GetInstance().GetGripForce(VRHand.Left);      // 握持力度 0~1

// 触摸板
SteamVRInputMgr.GetInstance().GetTouchpad(VRHand.Right);
SteamVRInputMgr.GetInstance().GetTouchpadAxis(VRHand.Right);  // 触摸位置 Vector2

// 摇杆
SteamVRInputMgr.GetInstance().GetThumbstickAxis(VRHand.Left); // 摇杆位置 Vector2

// A/B 按钮
SteamVRInputMgr.GetInstance().GetButtonA(VRHand.Right);
SteamVRInputMgr.GetInstance().GetButtonB(VRHand.Right);

// 手柄运动数据
SteamVRInputMgr.GetInstance().GetVelocity(VRHand.Right);         // 线速度
SteamVRInputMgr.GetInstance().GetAngularVelocity(VRHand.Right);  // 角速度

// 触觉反馈（振动）
SteamVRInputMgr.GetInstance().TriggerHapticShort(VRHand.Right);  // 短振动
SteamVRInputMgr.GetInstance().TriggerHaptic(VRHand.Left, 0.5f, 100f, 0.8f); // 自定义振动

// 检查 SteamVR 就绪状态
if (SteamVRInputMgr.GetInstance().IsReady) { ... }

// 绑定自定义动作
SteamVRInputMgr.GetInstance().BindAction(ref myAction, "MyActionSet", "MyAction");
```

**事件列表：**
| 事件名 | 参数 | 说明 |
|-------|------|------|
| `VR_TriggerDown` / `VR_TriggerUp` | VRHand | 扳机按下/抬起 |
| `VR_GripDown` / `VR_GripUp` | VRHand | 握持按下/抬起 |
| `VR_TouchpadDown` / `VR_TouchpadUp` | VRHand | 触摸板按下/抬起 |
| `VR_ButtonADown` / `VR_ButtonAUp` | VRHand | A 按钮按下/抬起 |

每个事件同时也会触发分左右手的版本（如 `VR_TriggerDown_Left`）。

---

### 13. DelayManager — 延时管理器

执行延时回调，类似 `Invoke` 但支持 Lambda 表达式。

```csharp
// 延时 2 秒执行
DelayManager.GetInstance().ExecuteAfterDelay(2f, () => {
    Debug.Log("2秒后执行");
});

// 每帧执行持续逻辑可用 MonoMgr.AddUpdateListener
```

**特点：** 复用 `MonoMgr` 的协程能力，不额外创建 GameObject。

---

## 快速上手

### 管理器初始化顺序

框架中 Manager 都使用懒加载，无需手动初始化。常见的使用方式是在游戏启动脚本中预热：

```csharp
public class GameBoot : MonoBehaviour
{
    void Awake()
    {
        // 预热常用管理器（可选，首次调用会自动创建）
        UIManager.GetInstance();
        MusicMgr.GetInstance();
        InputMgr.GetInstance();
    }

    void Start()
    {
        InputMgr.GetInstance().RegisterWASD();
        InputMgr.GetInstance().StartOrEndCheck(true);
    }
}
```

### SteamVR 项目初始化

```csharp
void Awake()
{
    var vrInput = SteamVRInputMgr.GetInstance();
    vrInput.BindDefaultActions();
    vrInput.StartOrEndCheck(true);

    // 监听 VR 输入事件
    EventCenter.GetInstance().AddEventListener<VRHand>("VR_TriggerDown", OnTriggerDown);
}
```

### 切换场景

```csharp
// SceneMgr 会自动清理 EventCenter 和 PoolMgr
SceneMgr.GetInstance().LoadScene("NextLevel", () => {
    // 场景加载完成后的初始化
    MusicMgr.GetInstance().PlayBKMusic("LevelBGM");
});
```

---

## 文件列表

| 文件 | 功能 | 依赖 |
|------|------|------|
| `BaseManager.cs` | 普通类单例基类 | 无 |
| `SingletonMono.cs` | MonoBehaviour 单例（手动放置） | 无 |
| `SingletonAutoMono.cs` | MonoBehaviour 单例（自动创建） | 无 |
| `EventCenter.cs` | 事件中心（观察者模式） | BaseManager |
| `MonoController.cs` | MonoBehaviour 帧更新载体 | 无 |
| `MonoMgr.cs` | 公共 Mono 管理器 | BaseManager, MonoController |
| `ResMgr.cs` | Resources 资源加载 | BaseManager, MonoMgr |
| `PoolMgr.cs` | 对象池 | BaseManager, ResMgr |
| `UIManager.cs` | UI 面板管理 | BaseManager, ResMgr |
| `BasePanel.cs` | UI 面板基类 | 无 |
| `SceneMgr.cs` | 场景切换 | BaseManager, EventCenter, PoolMgr, MonoMgr |
| `MusicMgr.cs` | 音乐音效 | BaseManager, ResMgr, MonoMgr |
| `InputMgr.cs` | 键盘输入 | BaseManager, EventCenter, MonoMgr |
| `SteamVRInputMgr.cs` | VR 手柄输入 | BaseManager, EventCenter, MonoMgr, SteamVR |
| `DelayManager.cs` | 延时执行 | BaseManager, MonoMgr |
| `AutoPush.cs` | 自动回池组件 | PoolMgr |

---

## 设计理念

1. **单例无处不在** — 所有 Manager 通过 `BaseManager<T>` 实现懒加载单例，减少依赖传递
2. **事件驱动解耦** — 模块间通过 `EventCenter` 通信，不直接引用
3. **Mono 能力共享** — 通过 `MonoMgr` 让普通类也能使用 Update 和协程
4. **命名约定优于配置** — `BasePanel` 通过 GameObject 名称匹配控件，省去手动拖拽引用
5. **场景切换自动清理** — `SceneMgr` 自动清空事件和对象池，防止内存泄漏
