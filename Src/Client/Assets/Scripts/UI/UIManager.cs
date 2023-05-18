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
        UIWindowPool.Add(typeof(UIPausePanel), new UIElement() { Resources = "UI/UIPausePanel", Cache = true });
        UIWindowPool.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false });
        UIWindowPool.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
        UIWindowPool.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = false });
        UIWindowPool.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuestSystem", Cache = false });
        UIWindowPool.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cache = false });
        UIWindowPool.Add(typeof(UIFriends), new UIElement() { Resources = "UI/UIFriend", Cache = false });
        UIWindowPool.Add(typeof(UIPopCharMenu), new UIElement() { Resources = "UI/UIPopCharMenu", Cache = false });
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
