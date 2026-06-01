using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public enum VRHand
{
    Left,
    Right
}

public class SteamVRInputMgr : BaseManager<SteamVRInputMgr>
{
    private bool isStart;

    // Actions from existing SteamVR action sets (always loaded & bound)
    // default: GrabPinch, GrabGrip, Squeeze, Pose, Haptic
    // platformer: Move (thumbstick), Jump (thumbstick click)
    // buggy: Throttle (trigger analog), Brake (X/A), Reset (Y/B)

    private SteamVR_Action_Boolean triggerAction;      // default/in/GrabPinch → trigger click
    private SteamVR_Action_Single triggerValueAction;   // buggy/in/Throttle → trigger pull
    private SteamVR_Action_Boolean gripAction;          // default/in/GrabGrip → grip click
    private SteamVR_Action_Single gripValueAction;      // default/in/Squeeze → grip pull
    private SteamVR_Action_Boolean primaryBtnAction;    // buggy/in/Brake → X (left) / A (right)
    private SteamVR_Action_Boolean secondaryBtnAction;  // buggy/in/Reset → Y (left) / B (right)
    private SteamVR_Action_Boolean stickClickAction;    // platformer/in/Jump → thumbstick click
    private SteamVR_Action_Vector2 stickAction;         // platformer/in/Move → thumbstick position
    private SteamVR_Action_Boolean menuAction;          // default/in/Teleport → repurposed as menu
    private SteamVR_Action_Pose poseAction;             // default/in/Pose
    private SteamVR_Action_Vibration hapticAction;      // default/out/Haptic

    // Previous frame state for edge detection
    private bool[] prevTrigger = new bool[2];
    private bool[] prevGrip = new bool[2];
    private bool[] prevPrimary = new bool[2];
    private bool[] prevSecondary = new bool[2];
    private bool[] prevStickClick = new bool[2];

    private static readonly Dictionary<VRHand, SteamVR_Input_Sources> HandSource =
        new Dictionary<VRHand, SteamVR_Input_Sources>
        {
            { VRHand.Left, SteamVR_Input_Sources.LeftHand },
            { VRHand.Right, SteamVR_Input_Sources.RightHand }
        };

    public SteamVRInputMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(MyUpdate);
    }

    public void StartOrEndCheck(bool isOpen) { isStart = isOpen; }

    public bool IsReady =>
        SteamVR_Input.initialized && OpenVR.System != null && OpenVR.IsHmdPresent();

    private bool actionsBound;

    /// <summary>
    /// Binds Quest 3 controls via existing SteamVR actions.
    /// Safe to call multiple times — auto-retries until SteamVR is ready.
    /// </summary>
    public void BindDefaultActions()
    {
        if (actionsBound) return;
        if (!SteamVR_Input.initialized) return;

        SteamVR_Input.GetActionSet("/actions/platformer")?.Activate();
        SteamVR_Input.GetActionSet("/actions/buggy")?.Activate();

        triggerAction        = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabPinch");
        triggerValueAction   = SteamVR_Input.GetAction<SteamVR_Action_Single>("buggy", "Throttle");
        gripAction           = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabGrip");
        gripValueAction      = SteamVR_Input.GetAction<SteamVR_Action_Single>("default", "Squeeze");
        primaryBtnAction     = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("buggy", "Brake");
        secondaryBtnAction   = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("buggy", "Reset");
        stickClickAction     = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("platformer", "Jump");
        stickAction          = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("platformer", "Move");
        poseAction           = SteamVR_Input.GetAction<SteamVR_Action_Pose>("default", "Pose");
        hapticAction         = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("default", "Haptic");

        actionsBound = true;
        Debug.Log("[SteamVRInputMgr] Quest 3 actions bound successfully");
    }

    private SteamVR_Input_Sources Src(VRHand h) =>
        HandSource.TryGetValue(h, out var s) ? s : SteamVR_Input_Sources.Any;

    // ── Trigger ──────────────────────────────────────────

    public bool GetTrigger(VRHand hand)     => triggerAction != null && triggerAction.GetState(Src(hand));
    public bool GetTriggerDown(VRHand hand) => triggerAction != null && triggerAction.GetStateDown(Src(hand));
    public bool GetTriggerUp(VRHand hand)   => triggerAction != null && triggerAction.GetStateUp(Src(hand));
    public float GetTriggerValue(VRHand hand) => triggerValueAction != null ? triggerValueAction.GetAxis(Src(hand)) : 0f;

    // ── Grip ─────────────────────────────────────────────

    public bool GetGrip(VRHand hand)     => gripAction != null && gripAction.GetState(Src(hand));
    public bool GetGripDown(VRHand hand) => gripAction != null && gripAction.GetStateDown(Src(hand));
    public bool GetGripUp(VRHand hand)   => gripAction != null && gripAction.GetStateUp(Src(hand));
    public float GetGripValue(VRHand hand) => gripValueAction != null ? gripValueAction.GetAxis(Src(hand)) : 0f;

    // ── Primary Button: X (left) / A (right) ─────────────

    public bool GetPrimaryButton(VRHand hand)     => primaryBtnAction != null && primaryBtnAction.GetState(Src(hand));
    public bool GetPrimaryButtonDown(VRHand hand) => primaryBtnAction != null && primaryBtnAction.GetStateDown(Src(hand));
    public bool GetPrimaryButtonUp(VRHand hand)   => primaryBtnAction != null && primaryBtnAction.GetStateUp(Src(hand));

    // ── Secondary Button: Y (left) / B (right) ───────────

    public bool GetSecondaryButton(VRHand hand)     => secondaryBtnAction != null && secondaryBtnAction.GetState(Src(hand));
    public bool GetSecondaryButtonDown(VRHand hand) => secondaryBtnAction != null && secondaryBtnAction.GetStateDown(Src(hand));
    public bool GetSecondaryButtonUp(VRHand hand)   => secondaryBtnAction != null && secondaryBtnAction.GetStateUp(Src(hand));

    // ── Thumbstick ───────────────────────────────────────

    public Vector2 GetThumbstick(VRHand hand) => stickAction != null ? stickAction.GetAxis(Src(hand)) : Vector2.zero;

    public bool GetThumbstickClick(VRHand hand)     => stickClickAction != null && stickClickAction.GetState(Src(hand));
    public bool GetThumbstickClickDown(VRHand hand) => stickClickAction != null && stickClickAction.GetStateDown(Src(hand));
    public bool GetThumbstickClickUp(VRHand hand)   => stickClickAction != null && stickClickAction.GetStateUp(Src(hand));

    // ── Menu ────────────────────────────────────────────

    public bool GetMenuDown() =>
        menuAction != null && menuAction.GetStateDown(SteamVR_Input_Sources.LeftHand);

    // ── Pose ────────────────────────────────────────────

    public Vector3 GetVelocity(VRHand hand)
    {
        if (poseAction == null || !poseAction.active) return Vector3.zero;
        return poseAction[Src(hand)].velocity;
    }

    public Vector3 GetAngularVelocity(VRHand hand)
    {
        if (poseAction == null || !poseAction.active) return Vector3.zero;
        return poseAction[Src(hand)].angularVelocity;
    }

    // ── Haptic ──────────────────────────────────────────

    public void TriggerHaptic(VRHand hand, float duration, float frequency, float amplitude)
    {
        hapticAction?.Execute(0, duration, frequency, amplitude, Src(hand));
    }

    public void TriggerHapticShort(VRHand hand, float duration = 0.1f)
    {
        TriggerHaptic(hand, duration, 80f, 0.5f);
    }

    // ── Per-frame update + event dispatch ───────────────

    private void MyUpdate()
    {
        // Auto-retry binding until SteamVR is ready
        if (!actionsBound)
            BindDefaultActions();

        if (!isStart || !SteamVR_Input.initialized) return;

        for (int i = 0; i < 2; i++)
        {
            var hand = (VRHand)i;
            var src = Src(hand);

            bool trigger  = triggerAction != null && triggerAction.GetState(src);
            bool grip     = gripAction != null && gripAction.GetState(src);
            bool primary  = primaryBtnAction != null && primaryBtnAction.GetState(src);
            bool secondary = secondaryBtnAction != null && secondaryBtnAction.GetState(src);
            bool stickClk = stickClickAction != null && stickClickAction.GetState(src);

            CheckEdge("VR_TriggerDown", "VR_TriggerUp", hand, trigger, prevTrigger[i]);
            CheckEdge("VR_GripDown", "VR_GripUp", hand, grip, prevGrip[i]);
            CheckEdge("VR_PrimaryDown", "VR_PrimaryUp", hand, primary, prevPrimary[i]);
            CheckEdge("VR_SecondaryDown", "VR_SecondaryUp", hand, secondary, prevSecondary[i]);
            CheckEdge("VR_StickClickDown", "VR_StickClickUp", hand, stickClk, prevStickClick[i]);

            prevTrigger[i] = trigger;
            prevGrip[i] = grip;
            prevPrimary[i] = primary;
            prevSecondary[i] = secondary;
            prevStickClick[i] = stickClk;
        }
    }

    private void CheckEdge(string downEv, string upEv, VRHand hand, bool cur, bool prev)
    {
        if (cur && !prev)
        {
            EventCenter.GetInstance().EventTrigger(downEv, hand);
            EventCenter.GetInstance().EventTrigger($"{downEv}_{hand}", hand);
        }
        if (!cur && prev)
        {
            EventCenter.GetInstance().EventTrigger(upEv, hand);
            EventCenter.GetInstance().EventTrigger($"{upEv}_{hand}", hand);
        }
    }
}
