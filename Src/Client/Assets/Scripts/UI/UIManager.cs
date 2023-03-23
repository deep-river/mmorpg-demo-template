using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    class UIElement
    {
        public string Resources;
        public bool Cache;
        public GameObject Instance;
    }

    private Dictionary<Type, UIElement> UIWindowPool = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        UIWindowPool.Add(typeof(UITest), new UIElement() { Resources = "UI/UITest", Cache = true });
    }

    ~UIManager()
    {

    }

    public T Show<T>()
    {
        // SoundManager.Instance.PlaySound("ui_open");
        Type type = typeof(T);
        if (UIWindowPool.ContainsKey(type))
        {
            UIElement window = UIWindowPool[type];
            if (window.Instance != null)
                window.Instance.SetActive(true);
            else
            {
                UnityEngine.Object prefab = Resources.Load(window.Resources);
                if (prefab == null)
                    return default(T);
                window.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return window.Instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close(Type type)
    {
        // SoundManager.Instance.PlaySound("ui_close");
        if (UIWindowPool.ContainsKey(type))
        {
            UIElement window = UIWindowPool[type];
            if (window.Cache)
                window.Instance.SetActive(false);
            else
            {
                GameObject.Destroy(window.Instance);
                window.Instance = null;
            }
        }
    }
}
