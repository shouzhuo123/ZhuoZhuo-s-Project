using UnityEngine;
using UnityEngine.InputSystem;

public class VRShoeUltimateController : MonoBehaviour
{
    [Header("【左右脚开关】")]
    [Tooltip("勾选代表左脚，取消勾选代表右脚")]
    public bool isLeftFoot = true;

    [Header("【骨骼节点】(对照你的Hierarchy拖入)")]
    [Tooltip("左脚拖入 mixamorig5:LeftLeg，右脚拖入 mixamorig5:RightLeg")]
    public Transform legRootBone;

    [Tooltip("左脚拖入 mixamorig5:LeftFoot，右脚拖入 mixamorig5:RightFoot")]
    public Transform footBone;

    [Header("【VR手柄/追踪器】")]
    [Tooltip("可以留空！脚本在游戏运行时会自动寻找。如果你有自定义追踪器也可以手动拖进来。")]
    public Transform targetTracker;

    [Header("【旋转角度微调】")]
    public float normalXRotation = 0f;    // 平时平放的角度
    public float kickXRotation = -50f;    // 按照你的要求：扣死扳机时绷脚尖到 -50 度
    public float smoothSpeed = 18f;       // 绷脚尖的丝滑过渡速度

    [Header("【VR 扳机键输入源】")]
    public InputActionProperty triggerAction;

    private float currentXRotation = 0f;

    void Start()
    {
        // 🔥【黑科技：免拖拽手柄功能】
        // 如果你没有手动拖入 Tracker，脚本在游戏启动时会自动根据名字在场景里帮你抓到它！
        if (targetTracker == null)
        {
            string targetName = isLeftFoot ? "LeftHand Controller" : "RightHand Controller";
            GameObject foundTracker = GameObject.Find(targetName);

            if (foundTracker != null)
            {
                targetTracker = foundTracker.transform;
            }
            else
            {
                // 兼容某些 VR 预设叫 LeftController / RightController
                targetName = isLeftFoot ? "LeftController" : "RightController";
                foundTracker = GameObject.Find(targetName);
                if (foundTracker != null) targetTracker = foundTracker.transform;
            }
        }
    }

    void OnEnable()
    {
        if (triggerAction.action != null) triggerAction.action.Enable();
    }

    void OnDisable()
    {
        if (triggerAction.action != null) triggerAction.action.Disable();
    }

    void Update()
    {
        // 1. 让根骨骼 (mixamorig5:LeftLeg / RightLeg) 实时死死咬住 VR 手柄/追踪器的位置
        if (legRootBone != null && targetTracker != null)
        {
            legRootBone.position = targetTracker.position;
            legRootBone.rotation = targetTracker.rotation;
        }

        // 2. 绷脚尖控制 (mixamorig5:LeftFoot / RightFoot)
        if (footBone == null) return;

        // 读取扳机键压感
        float triggerValue = 0f;
        if (triggerAction.action != null)
        {
            triggerValue = triggerAction.action.ReadValue<float>();
        }

        // 核心旋转逻辑：从 0 度平滑过渡到 -50 度
        float targetXRotation = Mathf.Lerp(normalXRotation, kickXRotation, triggerValue);
        currentXRotation = Mathf.Lerp(currentXRotation, targetXRotation, Time.deltaTime * smoothSpeed);

        // 应用到脚踝骨骼的本地 X 轴
        footBone.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
    }
}