using System.Collections;
using UnityEngine;

public class AutoPush : MonoBehaviour
{
    private Coroutine pushCoroutine;

    void OnEnable()
    {
        pushCoroutine = StartCoroutine(DelayPush());
    }

    void OnDisable()
    {
        if (pushCoroutine != null)
        {
            StopCoroutine(pushCoroutine);
            pushCoroutine = null;
        }
    }

    private IEnumerator DelayPush()
    {
        yield return new WaitForSeconds(1f);
        PoolMgr.GetInstance().PushObj(gameObject.name, gameObject);
    }
}
