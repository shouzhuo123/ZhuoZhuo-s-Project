using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : BaseManager<SceneMgr>
{
    public void LoadScene(string name, UnityAction func = null)
    {
        EventCenter.GetInstance().Clear();
        PoolMgr.GetInstance().Clear();
        if (func != null)
            MonoMgr.GetInstance().StartCoroutine(LoadSceneCoroutine(name, func));
        else
            SceneManager.LoadScene(name);
    }

    private IEnumerator LoadSceneCoroutine(string name, UnityAction func)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        ao.allowSceneActivation = true;
        while (!ao.isDone)
            yield return null;
        func();
    }

    public void LoadSceneAsyn(string name, UnityAction func = null)
    {
        MonoMgr.GetInstance().StartCoroutine(ReallyLoadSceneAsyn(name, func));
    }

    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction func)
    {
        EventCenter.GetInstance().Clear();
        PoolMgr.GetInstance().Clear();

        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        ao.allowSceneActivation = false;

        while (ao.progress < 0.9f)
            yield return null;

        ao.allowSceneActivation = true;
        while (!ao.isDone)
            yield return null;

        func?.Invoke();
    }
}
