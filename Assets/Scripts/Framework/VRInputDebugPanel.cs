using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VRInputDebugPanel : MonoBehaviour
{
    [SerializeField] private float panelPixelWidth = 1400f;
    [SerializeField] private float panelPixelHeight = 950f;
    [SerializeField] private float canvasScale = 0.001f;
    [SerializeField] private float fontSize = 32f;
    [SerializeField] private float titleFontSize = 42f;
    [SerializeField] private Vector3 worldPosition = new Vector3(0f, 1.5f, 2.5f);

    private SteamVRInputMgr input;

    private TextMeshProUGUI statusText;
    private TextMeshProUGUI leftTitle, rightTitle;
    private TextMeshProUGUI leftTrigger, leftGrip, leftThumbstick, leftBtnX, leftBtnY, leftVelocity;
    private TextMeshProUGUI rightTrigger, rightGrip, rightThumbstick, rightBtnA, rightBtnB, rightVelocity;
    private TextMeshProUGUI eventLogText;
    private string eventLog = "";
    private float eventLogTimer;

    void Awake()
    {
        input = SteamVRInputMgr.GetInstance();
        CreateUI();
        MonoMgr.GetInstance().AddUpdateListener(OnUpdate);

        // Subscribe to all VR events
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_TriggerDown", OnVREvent);
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_TriggerUp", OnVREvent);
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_GripDown", OnVREvent);
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_GripUp", OnVREvent);
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_PrimaryDown", OnVREvent);
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_PrimaryUp", OnVREvent);
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_SecondaryDown", OnVREvent);
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_SecondaryUp", OnVREvent);
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_StickClickDown", OnVREvent);
        EventCenter.GetInstance().AddEventListener<VRHand>("VR_StickClickUp", OnVREvent);
        EventCenter.GetInstance().AddEventListener("VR_MenuDown", OnMenuEvent);
    }

    void Start()
    {
        input.BindDefaultActions();
        input.StartOrEndCheck(true);
    }

    void OnDestroy()
    {
        MonoMgr.GetInstance().RemoveUpdateListener(OnUpdate);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_TriggerDown", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_TriggerUp", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_GripDown", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_GripUp", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_PrimaryDown", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_PrimaryUp", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_SecondaryDown", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_SecondaryUp", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_StickClickDown", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener<VRHand>("VR_StickClickUp", OnVREvent);
        EventCenter.GetInstance().RemoveEventListener("VR_MenuDown", OnMenuEvent);
    }

    void CreateUI()
    {
        const float margin = 24f;
        const float rowH = 50f;
        const float sepH = 3f;
        const float gap = 8f;

        float y = margin;

        // --- Root canvas: World Space ---
        var canvasGo = new GameObject("VRDebugCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        var canvasRect = canvasGo.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(panelPixelWidth, panelPixelHeight);
        canvasGo.AddComponent<GraphicRaycaster>();

        transform.position = worldPosition;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * canvasScale;

        // --- Panel background ---
        var panelGo = new GameObject("Panel");
        panelGo.transform.SetParent(canvasGo.transform, false);
        var panelRect = panelGo.AddComponent<RectTransform>();
        panelRect.StretchToParent();
        var panelImg = panelGo.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.85f);

        // --- Title ---
        var title = MakeText("Title", panelGo.transform, "QUEST 3 INPUT DEBUG", titleFontSize);
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.3f, 0.8f, 1f);
        AnchorTop(title, ref y, rowH + 8, margin);

        // --- Status ---
        statusText = MakeText("Status", panelGo.transform, "Status: Checking...", fontSize);
        statusText.color = new Color(0.6f, 0.6f, 0.6f);
        AnchorTop(statusText, ref y, rowH - 8, margin);

        y += gap;
        MakeSeparator(panelGo.transform, ref y, sepH, margin);
        y += gap;

        // === LEFT HAND ===
        //  Quest 3 left: trigger, grip, thumbstick + click, X, Y, menu
        leftTitle = MakeText("LeftTitle", panelGo.transform, "LEFT HAND", titleFontSize * 0.85f);
        leftTitle.color = new Color(0.3f, 1f, 0.3f);
        AnchorTop(leftTitle, ref y, rowH, margin);

        leftTrigger    = MakeRow("LeftTrigger",  panelGo.transform, ref y, rowH, margin);
        leftGrip       = MakeRow("LeftGrip",     panelGo.transform, ref y, rowH, margin);
        leftThumbstick = MakeRow("LeftStick",    panelGo.transform, ref y, rowH, margin);
        leftBtnX       = MakeRow("LeftBtnX",     panelGo.transform, ref y, rowH, margin);
        leftBtnY       = MakeRow("LeftBtnY",     panelGo.transform, ref y, rowH, margin);
        leftVelocity   = MakeRow("LeftVelocity", panelGo.transform, ref y, rowH, margin);

        y += gap;
        MakeSeparator(panelGo.transform, ref y, sepH, margin);
        y += gap;

        // === RIGHT HAND ===
        //  Quest 3 right: trigger, grip, thumbstick + click, A, B
        rightTitle = MakeText("RightTitle", panelGo.transform, "RIGHT HAND", titleFontSize * 0.85f);
        rightTitle.color = new Color(0.3f, 1f, 0.3f);
        AnchorTop(rightTitle, ref y, rowH, margin);

        rightTrigger    = MakeRow("RightTrigger",  panelGo.transform, ref y, rowH, margin);
        rightGrip       = MakeRow("RightGrip",     panelGo.transform, ref y, rowH, margin);
        rightThumbstick = MakeRow("RightStick",    panelGo.transform, ref y, rowH, margin);
        rightBtnA       = MakeRow("RightBtnA",     panelGo.transform, ref y, rowH, margin);
        rightBtnB       = MakeRow("RightBtnB",     panelGo.transform, ref y, rowH, margin);
        rightVelocity   = MakeRow("RightVelocity", panelGo.transform, ref y, rowH, margin);

        y += gap;
        MakeSeparator(panelGo.transform, ref y, sepH, margin);
        y += gap;

        // --- Event log ---
        eventLogText = MakeText("EventLog", panelGo.transform, "", fontSize * 0.8f);
        eventLogText.color = new Color(1f, 1f, 0.4f);
        AnchorTop(eventLogText, ref y, rowH - 8, margin);
    }

    // ---- UI builders ----

    TextMeshProUGUI MakeText(string name, Transform parent, string text, float size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = Color.white;
        tmp.overflowMode = TextOverflowModes.Truncate;
        return tmp;
    }

    TextMeshProUGUI MakeRow(string name, Transform parent, ref float y, float height, float margin)
    {
        var txt = MakeText(name, parent, "", fontSize);
        AnchorTop(txt, ref y, height, margin);
        return txt;
    }

    void MakeSeparator(Transform parent, ref float y, float height, float margin)
    {
        var go = new GameObject("Sep");
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        AnchorTopRect(rect, ref y, height, margin);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.3f, 0.6f, 0.3f);
    }

    void AnchorTop(MonoBehaviour mb, ref float y, float height, float margin)
    {
        AnchorTopRect(mb.GetComponent<RectTransform>(), ref y, height, margin);
    }

    void AnchorTopRect(RectTransform rect, ref float y, float height, float margin)
    {
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(-margin * 2, height);
        rect.anchoredPosition = new Vector2(0, -y);
        y += height;
    }

    // ---- Event callbacks ----

    void OnVREvent(VRHand hand)
    {
        eventLog = $"[{Time.time:F1}s] Event from {hand} hand";
        eventLogTimer = 3f;
    }

    void OnMenuEvent()
    {
        eventLog = $"[{Time.time:F1}s] MENU button pressed";
        eventLogTimer = 3f;
    }

    // ---- Per-frame update ----

    void OnUpdate()
    {
        if (input == null || !input.IsReady)
        {
            if (statusText != null)
            {
                statusText.text = "Status: SteamVR NOT READY (start SteamVR first)";
                statusText.color = Color.red;
            }
            return;
        }
        statusText.text = "Status: SteamVR OK (Quest 3)";
        statusText.color = Color.green;

        // --- Left Hand ---
        leftTitle.text      = $"LEFT  [Trig:{B2S(input.GetTrigger(VRHand.Left))}] [Grip:{B2S(input.GetGrip(VRHand.Left))}]";
        leftTrigger.text    = $"Trigger:     val={input.GetTriggerValue(VRHand.Left):F2}  down={B2S(input.GetTriggerDown(VRHand.Left))}  up={B2S(input.GetTriggerUp(VRHand.Left))}";
        leftGrip.text       = $"Grip:        val={input.GetGripValue(VRHand.Left):F2}  down={B2S(input.GetGripDown(VRHand.Left))}  up={B2S(input.GetGripUp(VRHand.Left))}";
        leftThumbstick.text = $"Stick:       pos={V2S(input.GetThumbstick(VRHand.Left))}  click={B2S(input.GetThumbstickClick(VRHand.Left))}";
        leftBtnX.text       = $"Btn X (Pri): hold={B2S(input.GetPrimaryButton(VRHand.Left))}  down={B2S(input.GetPrimaryButtonDown(VRHand.Left))}  up={B2S(input.GetPrimaryButtonUp(VRHand.Left))}";
        leftBtnY.text       = $"Btn Y (Sec): hold={B2S(input.GetSecondaryButton(VRHand.Left))}  down={B2S(input.GetSecondaryButtonDown(VRHand.Left))}  up={B2S(input.GetSecondaryButtonUp(VRHand.Left))}";
        leftVelocity.text   = $"Velocity:    lin={V3S(input.GetVelocity(VRHand.Left))}  ang={V3S(input.GetAngularVelocity(VRHand.Left))}";

        // --- Right Hand ---
        rightTitle.text     = $"RIGHT [Trig:{B2S(input.GetTrigger(VRHand.Right))}] [Grip:{B2S(input.GetGrip(VRHand.Right))}]";
        rightTrigger.text   = $"Trigger:     val={input.GetTriggerValue(VRHand.Right):F2}  down={B2S(input.GetTriggerDown(VRHand.Right))}  up={B2S(input.GetTriggerUp(VRHand.Right))}";
        rightGrip.text      = $"Grip:        val={input.GetGripValue(VRHand.Right):F2}  down={B2S(input.GetGripDown(VRHand.Right))}  up={B2S(input.GetGripUp(VRHand.Right))}";
        rightThumbstick.text = $"Stick:       pos={V2S(input.GetThumbstick(VRHand.Right))}  click={B2S(input.GetThumbstickClick(VRHand.Right))}";
        rightBtnA.text      = $"Btn A (Pri): hold={B2S(input.GetPrimaryButton(VRHand.Right))}  down={B2S(input.GetPrimaryButtonDown(VRHand.Right))}  up={B2S(input.GetPrimaryButtonUp(VRHand.Right))}";
        rightBtnB.text      = $"Btn B (Sec): hold={B2S(input.GetSecondaryButton(VRHand.Right))}  down={B2S(input.GetSecondaryButtonDown(VRHand.Right))}  up={B2S(input.GetSecondaryButtonUp(VRHand.Right))}";
        rightVelocity.text  = $"Velocity:    lin={V3S(input.GetVelocity(VRHand.Right))}  ang={V3S(input.GetAngularVelocity(VRHand.Right))}";

        // Event log
        if (eventLogTimer > 0)
        {
            eventLogTimer -= Time.deltaTime;
            eventLogText.text = eventLog;
        }
        else
        {
            eventLogText.text = "Waiting for input events...";
        }
    }

    static string B2S(bool b) => b ? "ON" : "--";
    static string V2S(Vector2 v) => $"({v.x:+.00}, {v.y:+.00})";
    static string V3S(Vector3 v) => $"({v.x:+.0}, {v.y:+.0}, {v.z:+.0})";
}

public static class RectTransformExtensions
{
    public static void StretchToParent(this RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
