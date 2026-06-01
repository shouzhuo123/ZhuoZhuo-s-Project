using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoolData
{
    public GameObject fatherObj;
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.SetParent(poolObj.transform, false);
        poolList = new List<GameObject>();
        PushObj(obj);
    }

    public void PushObj(GameObject obj)
    {
        poolList.Add(obj);
        obj.transform.SetParent(fatherObj.transform, false);
        obj.SetActive(false);
    }

    public GameObject GetObj()
    {
        GameObject obj = poolList[0];
        poolList.RemoveAt(0);
        obj.SetActive(true);
        obj.transform.SetParent(null, false);
        return obj;
    }
}

public class PoolMgr : BaseManager<PoolMgr>
{
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    private GameObject poolObj;

    public void GetObj(string name, UnityAction<GameObject> callback)
    {
        if (poolDic.TryGetValue(name, out var data) && data.poolList.Count > 0)
        {
            callback(data.GetObj());
        }
        else
        {
            ResMgr.GetInstance().LoadAsync<GameObject>(name, (o) =>
            {
                o.name = name;
                callback(o);
            });
        }
    }

    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
        {
            poolObj = new GameObject("Pool");
            GameObject.DontDestroyOnLoad(poolObj);
        }

        if (poolDic.ContainsKey(name))
            poolDic[name].PushObj(obj);
        else
            poolDic.Add(name, new PoolData(obj, poolObj));
    }

    public void Clear()
    {
        if (poolObj != null)
            GameObject.Destroy(poolObj);
        poolDic.Clear();
        poolObj = null;
    }
}
