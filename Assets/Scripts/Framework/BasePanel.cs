using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    protected virtual void Awake()
    {
        FindAllControls();
    }

    private void FindAllControls()
    {
        var allBehaviours = GetComponentsInChildren<UIBehaviour>(true);
        foreach (var control in allBehaviours)
        {
            string objName = control.gameObject.name;
            if (!controlDic.ContainsKey(objName))
                controlDic.Add(objName, new List<UIBehaviour>());
            controlDic[objName].Add(control);

            if (control is Button btn)
            {
                btn.onClick.AddListener(() => OnClick(objName));
            }
        }
    }

    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if (controlDic.TryGetValue(controlName, out var list))
        {
            foreach (var item in list)
                if (item is T result) return result;
        }
        return null;
    }

    protected virtual void OnClick(string btnName) { }

    public virtual void ShowMe() { }
    public virtual void HideMe() { }
}
