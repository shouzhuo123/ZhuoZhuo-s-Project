using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System
}

public class UIManager : BaseManager<UIManager>
{
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    public RectTransform canvas;

    private Dictionary<E_UI_Layer, Transform> layerMap = new Dictionary<E_UI_Layer, Transform>();

    public UIManager()
    {
        GameObject obj = ResMgr.GetInstance().Load<GameObject>("UI/Canvas");
        canvas = obj.transform as RectTransform;
        GameObject.DontDestroyOnLoad(obj);

        layerMap[E_UI_Layer.Bot] = canvas.Find("Bot");
        layerMap[E_UI_Layer.Mid] = canvas.Find("Mid");
        layerMap[E_UI_Layer.Top] = canvas.Find("Top");
        layerMap[E_UI_Layer.System] = canvas.Find("System");

        obj = ResMgr.GetInstance().Load<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);
    }

    public void ShowPanel<T>(string panelName, E_UI_Layer layer = E_UI_Layer.Top, UnityAction<T> callback = null) where T : BasePanel
    {
        if (panelDic.TryGetValue(panelName, out var existingPanel))
        {
            existingPanel.ShowMe();
            callback?.Invoke(existingPanel as T);
            return;
        }

        ResMgr.GetInstance().LoadAsync<GameObject>("UI/" + panelName, (obj) =>
        {
            Transform father = layerMap.TryGetValue(layer, out var f) ? f : layerMap[E_UI_Layer.Top];
            obj.transform.SetParent(father);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            var rect = obj.transform as RectTransform;
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;

            T panel = obj.GetComponent<T>();
            panel.ShowMe();
            callback?.Invoke(panel);
            panelDic.Add(panelName, panel);
        });
    }

    public Transform GetLayerFather(E_UI_Layer layer)
    {
        layerMap.TryGetValue(layer, out var father);
        return father;
    }

    public void HidePanel(string panelName)
    {
        if (panelDic.TryGetValue(panelName, out var panel))
        {
            panel.HideMe();
            GameObject.Destroy(panel.gameObject);
            panelDic.Remove(panelName);
        }
    }

    public T GetPanel<T>(string name) where T : BasePanel
    {
        panelDic.TryGetValue(name, out var panel);
        return panel as T;
    }

    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callback)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
}
