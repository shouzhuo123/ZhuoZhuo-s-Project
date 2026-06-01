using System.Collections.Generic;
using UnityEngine;

public class InputMgr : BaseManager<InputMgr>
{
    private bool isStart;
    private HashSet<KeyCode> monitoredKeys = new HashSet<KeyCode>();

    public InputMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(MyUpdate);
    }

    public void StartOrEndCheck(bool isOpen)
    {
        isStart = isOpen;
    }

    public void RegisterKey(KeyCode key)
    {
        monitoredKeys.Add(key);
    }

    public void UnregisterKey(KeyCode key)
    {
        monitoredKeys.Remove(key);
    }

    public void RegisterWASD()
    {
        monitoredKeys.Add(KeyCode.W);
        monitoredKeys.Add(KeyCode.A);
        monitoredKeys.Add(KeyCode.S);
        monitoredKeys.Add(KeyCode.D);
    }

    public float GetAxisRaw(string axisName)
    {
        return Input.GetAxisRaw(axisName);
    }

    public float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }

    public bool GetKey(KeyCode key)
    {
        return Input.GetKey(key);
    }

    public bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    public bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    private void CheckKeyCode(KeyCode key)
    {
        if (Input.GetKeyDown(key))
            EventCenter.GetInstance().EventTrigger("KeyDown", key);
        if (Input.GetKeyUp(key))
            EventCenter.GetInstance().EventTrigger("KeyUp", key);
    }

    private void MyUpdate()
    {
        if (!isStart) return;
        foreach (var key in monitoredKeys)
            CheckKeyCode(key);
    }
}
