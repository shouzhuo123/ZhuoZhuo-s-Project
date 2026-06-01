using System;
using System.Collections;
using UnityEngine;

public class DelayManager : BaseManager<DelayManager>
{
    public void ExecuteAfterDelay(float delaySeconds, Action callback)
    {
        if (delaySeconds < 0)
        {
            Debug.LogError("延时时间不能为负数！");
            return;
        }
        if (callback == null)
        {
            Debug.LogError("回调方法不能为空！");
            return;
        }
        MonoMgr.GetInstance().StartCoroutine(DelayCoroutine(delaySeconds, callback));
    }

    private IEnumerator DelayCoroutine(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
