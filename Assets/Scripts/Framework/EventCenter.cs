using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo { }

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action) { actions += action; }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public EventInfo(UnityAction action) { actions += action; }
}

public class EventCenter : BaseManager<EventCenter>
{
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            var info = eventDic[name] as EventInfo<T>;
            if (info == null)
            {
                Debug.LogError($"事件 {name} 已注册为无参类型，无法添加带参监听");
                return;
            }
            info.actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            var info = eventDic[name] as EventInfo;
            if (info == null)
            {
                Debug.LogError($"事件 {name} 已注册为带参类型，无法添加无参监听");
                return;
            }
            info.actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo<T>)?.actions?.Invoke(info);
    }

    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo)?.actions?.Invoke();
    }

    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (!eventDic.ContainsKey(name)) return;
        var info = eventDic[name] as EventInfo<T>;
        if (info == null) return;
        info.actions -= action;
        if (info.actions == null)
            eventDic.Remove(name);
    }

    public void RemoveEventListener(string name, UnityAction action)
    {
        if (!eventDic.ContainsKey(name)) return;
        var info = eventDic[name] as EventInfo;
        if (info == null) return;
        info.actions -= action;
        if (info.actions == null)
            eventDic.Remove(name);
    }

    public void Clear()
    {
        eventDic.Clear();
    }
}
