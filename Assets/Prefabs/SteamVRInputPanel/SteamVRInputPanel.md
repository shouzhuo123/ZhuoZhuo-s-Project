SteamVRInputPanel

 已创建的 Package 文件夹

  Assets/Prefabs/SteamVRInputPanel/
  ├── VRInputDebugPanel.prefab           ← 调试面板预制体
  ├── actions.json                        ← SteamVR 动作配置
  ├── bindings_oculus_touch.json          ← Quest 3 手柄绑定

  导出为 Unity Package

  在 Unity 中右键点击 Assets/Prefabs/SteamVRInputPanel 文件夹 → Export Package，Unity 会自动关联依赖脚本：

  ┌──────────────────────┬───────────────────────────┐
  │    自动包含的脚本    │           来自            │
  ├──────────────────────┼───────────────────────────┤
  │ VRInputDebugPanel.cs │ Assets/Scripts/Framework/ │
  ├──────────────────────┼───────────────────────────┤
  │ SteamVRInputMgr.cs   │ Assets/Scripts/Framework/ │
  ├──────────────────────┼───────────────────────────┤
  │ BaseManager.cs       │ Assets/Scripts/Framework/ │
  ├──────────────────────┼───────────────────────────┤
  │ EventCenter.cs       │ Assets/Scripts/Framework/ │
  ├──────────────────────┼───────────────────────────┤
  │ MonoController.cs    │ Assets/Scripts/Framework/ │
  ├──────────────────────┼───────────────────────────┤
  │ MonoMgr.cs           │ Assets/Scripts/Framework/ │
  └──────────────────────┴───────────────────────────┘

  在其他项目中使用

  导入 package 后需要：

  1. 覆盖 SteamVR Input 配置 — 将 actions.json 和 bindings_oculus_touch.json 复制到 Assets/StreamingAssets/SteamVR/（替换原文件）
  2. 拖入预制体 — 将 VRInputDebugPanel.prefab 拖到场景中
  3. 设置 Camera — 确保场景有 MainCamera tagged 的相机
  4. 运行 — 进入 Play Mode，面板出现在 (0, 1.5, 2.5) 世界坐标

  配置参数（Inspector 中调整）

  ┌────────────────────┬───────────────┬────────────────────┐
  │        参数        │    默认值     │        说明        │
  ├────────────────────┼───────────────┼────────────────────┤
  │ Panel Pixel Width  │ 1400          │ 面板宽度（像素）   │
  ├────────────────────┼───────────────┼────────────────────┤
  │ Panel Pixel Height │ 950           │ 面板高度（像素）   │
  ├────────────────────┼───────────────┼────────────────────┤
  │ Canvas Scale       │ 0.001         │ 缩放（保持 0.001） │
  ├────────────────────┼───────────────┼────────────────────┤
  │ Font Size          │ 32            │ 正文字号           │
  ├────────────────────┼───────────────┼────────────────────┤
  │ Title Font Size    │ 42            │ 标题字号           │
  ├────────────────────┼───────────────┼────────────────────┤
  │ World Position     │ (0, 1.5, 2.5) │ 面板世界位置       │
  └────────────────────┴───────────────┴────────────────────┘